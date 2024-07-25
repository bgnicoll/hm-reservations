namespace Services.DTOs
{
    public class GetAvailableAppointmentsRequestDTO : IRequestDTO
    {
        private const int maximumSearchDateRangeDays = 7;
        public DateTime? StartSearchDate { get; set; }
        public DateTime? EndSearchDate { get; set; }
        public string ProviderName { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public IEnumerable<string> Validate()
        {
            var validationErrors = new List<string>();
            if (StartSearchDate.HasValue &&
                EndSearchDate.HasValue &&
                EndSearchDate < StartSearchDate)
            {
                validationErrors.Add("EndSearchDate must be greater than or equal to StartSearchDate.");
            }

            if (StartSearchDate.HasValue &&
                EndSearchDate.HasValue &&
                (EndSearchDate - StartSearchDate).Value.TotalDays > maximumSearchDateRangeDays)
            {
                validationErrors.Add($"Search range cannot be longer than {maximumSearchDateRangeDays} days");
            }

            return validationErrors;
        }
    }
}
