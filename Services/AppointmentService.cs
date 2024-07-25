using Microsoft.EntityFrameworkCore;
using Services.DTOs;
using Services.Helpers;
using Services.Models;

namespace Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ReservationDbContext _reservationDbContext;
        private const int appointmentLengthMinutes = 15;
        private const int advanceAppointmentMinimumHours = 24;
        private const int maxAppointmentPageSize = 200;
        private const int maxUnconfirmedAppointmentReservationLimitMinutes = 30;
        public AppointmentService(ReservationDbContext reservationDbContext)
        {
            _reservationDbContext = reservationDbContext;
        }

        public async Task<PostSubmitAvailabilityResponseDTO> GenerateAppointmentsFromProviderAvailability(User provider, PostSubmitAvailabilityRequestDTO requestDto)
        {
            var roundedStartAvailabilityTime = DateTimeSegmentHelper.RoundUpToNearestSegment(requestDto.StartAvailabilityDate);
            var roundedEndAvailabilityTime = DateTimeSegmentHelper.RoundDownToNearestSegment(requestDto.EndAvailabilityDate);

            var existingAppointmentsInTimeRange = await _reservationDbContext.Appointments.
                Where(x =>
                x.ProviderUserId == provider.Id &&
                x.TimeSlot >= roundedStartAvailabilityTime &&
                x.TimeSlot <= roundedEndAvailabilityTime)
                .ToListAsync();

            var appointments = new List<Appointment>();

            var appointmentStartTime = roundedStartAvailabilityTime;

            while (appointmentStartTime < roundedEndAvailabilityTime)
            {
                var existingAppointment = existingAppointmentsInTimeRange.FirstOrDefault(x => x.TimeSlot == appointmentStartTime);
                if (existingAppointment == null)
                {
                    appointments.Add(new Appointment
                    {
                        Provider = provider,
                        TimeSlot = appointmentStartTime
                    });
                }

                appointmentStartTime = appointmentStartTime.AddMinutes(appointmentLengthMinutes);
            }

            await _reservationDbContext.Appointments.AddRangeAsync(appointments);
            var recordsCreated = await _reservationDbContext.SaveChangesAsync();

            return new PostSubmitAvailabilityResponseDTO { AppointmentSlotsCreated = recordsCreated };
        }

        public async Task<GetAvailableAppointmentsResponseDTO> GetAvailableAppointmentsAsync(GetAvailableAppointmentsRequestDTO requestDto)
        {
            IQueryable<Appointment> availableAppointmentsQuery = _reservationDbContext.Appointments;
            var minimumAppointmentReservationDate = DateTime.UtcNow.AddHours(advanceAppointmentMinimumHours);
            availableAppointmentsQuery = availableAppointmentsQuery.Where(x => x.TimeSlot >= minimumAppointmentReservationDate);

            if (requestDto.StartSearchDate.HasValue)
            {
                availableAppointmentsQuery = availableAppointmentsQuery.Where(x => x.TimeSlot >= requestDto.StartSearchDate.Value);
            }

            if (requestDto.EndSearchDate.HasValue)
            {
                availableAppointmentsQuery = availableAppointmentsQuery.Where(x => x.TimeSlot <= requestDto.EndSearchDate.Value);
            }

            if (!string.IsNullOrEmpty(requestDto.ProviderName))
            {
                availableAppointmentsQuery =
                    availableAppointmentsQuery.Where(x => x.Provider.Name.Contains(requestDto.ProviderName, StringComparison.CurrentCultureIgnoreCase));
            }

            availableAppointmentsQuery = availableAppointmentsQuery.Where(x =>
            !x.ConfirmationDate.HasValue &&
            (!x.ReservedDate.HasValue || x.ReservedDate.Value <= DateTime.UtcNow.AddMinutes(-maxUnconfirmedAppointmentReservationLimitMinutes)));

            availableAppointmentsQuery = availableAppointmentsQuery.Include(x => x.Provider);
            availableAppointmentsQuery = availableAppointmentsQuery.OrderBy(x => x.TimeSlot);

            var skip = requestDto.Page - 1 >= 0 ? requestDto.Page - 1 : 0;
            var take = requestDto.PageSize > maxAppointmentPageSize ? maxAppointmentPageSize : requestDto.PageSize;

            availableAppointmentsQuery = availableAppointmentsQuery.Skip(skip).Take(take);

            var availableAppointments = await availableAppointmentsQuery.ToListAsync();
            return GetAvailableAppointmentsResponseDTO.Map(availableAppointments);
        }

        public async Task<bool> AppointmentIsAvailableForReservationByClient(User client, int appointmentId)
        {
            var appointment = await GetReservableAppointmentById(appointmentId);

            if (appointment == null)
            {
                return false;
            }

            var userIsAlreadyBooked = await UserIsAlreadyBooked(client, appointment);

            if (userIsAlreadyBooked)
            {
                return false;
            }

            return true;
        }

        public async Task<PostBookAppointmentResponseDTO> ReserveAppointment(User client, int appointmentId)
        {
            var appointmentToBook = await GetReservableAppointmentById(appointmentId);

            if (appointmentToBook == null)
            {
                return null;
            }

            appointmentToBook.Client = client;
            appointmentToBook.ReservedDate = DateTime.UtcNow;

            await _reservationDbContext.SaveChangesAsync();

            return PostBookAppointmentResponseDTO.Map(appointmentToBook);
        }

        public async Task<bool> AppointmentIsAvailableToConfirmByClient(User client, int appointmentId)
        {
            var appointment = await GetConfirmableAppointmentById(client, appointmentId);

            if (appointment == null)
            {
                return false;
            }

            var userIsAlreadyBooked = await UserIsAlreadyBookedByADifferentAppointment(client, appointment);

            if (userIsAlreadyBooked)
            {
                return false;
            }

            return true;
        }

        public async Task<PostConfirmAppointmentResponseDTO> ConfirmAppointment(User client, int appointmentId)
        {
            var appointmentToConfirm = await GetConfirmableAppointmentById(client, appointmentId);

            if (appointmentToConfirm == null)
            {
                return null;
            }

            appointmentToConfirm.ConfirmationDate = DateTime.UtcNow;
            await _reservationDbContext.SaveChangesAsync();

            return PostConfirmAppointmentResponseDTO.Map(appointmentToConfirm);
        }

        private async Task<Appointment> GetReservableAppointmentById(int appointmentId)
        {
            return await _reservationDbContext.Appointments
                .Include(x => x.Provider)
                .FirstOrDefaultAsync(x =>
                x.Id == appointmentId &&
                x.TimeSlot > DateTime.UtcNow.AddHours(advanceAppointmentMinimumHours) &&
                (!x.ReservedDate.HasValue || x.ReservedDate.Value <= DateTime.UtcNow.AddMinutes(-maxUnconfirmedAppointmentReservationLimitMinutes)) &&
                !x.ConfirmationDate.HasValue);
        }

        private async Task<Appointment> GetConfirmableAppointmentById(User client, int appointmentId)
        {
            return await _reservationDbContext.Appointments
                .Include(x => x.Provider)
                .FirstOrDefaultAsync(x =>
                x.Id == appointmentId &&
                x.ClientUserId == client.Id &&
                x.ReservedDate.HasValue &&
                x.ReservedDate.Value >= DateTime.UtcNow.AddMinutes(-maxUnconfirmedAppointmentReservationLimitMinutes) &&
                !x.ConfirmationDate.HasValue);
        }

        private async Task<bool> UserIsAlreadyBooked(User client, Appointment appointment)
        {
            return await _reservationDbContext.Appointments.AnyAsync(x =>
                x.TimeSlot == appointment.TimeSlot &&
                !x.ConfirmationDate.HasValue &&
                (x.ReservedDate == null || x.ReservedDate.Value > DateTime.UtcNow.AddMinutes(-maxUnconfirmedAppointmentReservationLimitMinutes)) &&
                (x.ProviderUserId == client.Id ||
                x.ClientUserId == client.Id));
        }

        private async Task<bool> UserIsAlreadyBookedByADifferentAppointment(User client, Appointment appointment)
        {
            return await _reservationDbContext.Appointments.AnyAsync(x =>
                x.TimeSlot == appointment.TimeSlot &&
                x.Id != appointment.Id &&
                !x.ConfirmationDate.HasValue &&
                (x.ReservedDate == null || x.ReservedDate.Value > DateTime.UtcNow.AddMinutes(-maxUnconfirmedAppointmentReservationLimitMinutes)) &&
                (x.ProviderUserId == client.Id ||
                x.ClientUserId == client.Id));
        }
    }
}
