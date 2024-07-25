using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Services.DTOs
{
    public class PostSubmitAvailabilityRequestDTO : IRequestDTO
    {
        private const int maximumAvailabilitySubmissionDays = 7;
        private const int advanceAvailabilityHours = 24;

        [Required]
        [JsonProperty("startAvailabilityDate")]
        public DateTime StartAvailabilityDate { get; set; }
        [Required]
        [JsonProperty("endAvailabilityDate")]
        public DateTime EndAvailabilityDate { get; set; }
        public IEnumerable<string> Validate()
        {
            var validationErrors = new List<string>();

            if (EndAvailabilityDate < StartAvailabilityDate)
            {
                validationErrors.Add("EndAvailabilityDate must be greater than or equal to StartAvailabilityDate.");
            }

            if ((EndAvailabilityDate - StartAvailabilityDate).TotalDays > maximumAvailabilitySubmissionDays)
            {
                validationErrors.Add($"Cannot submit more than {maximumAvailabilitySubmissionDays} days of availability in one request");
            }

            if (EndAvailabilityDate < (DateTime.UtcNow.AddHours(advanceAvailabilityHours)))
            {
                validationErrors.Add($"Availability cannot be set for less than {advanceAvailabilityHours} hours in advance");
            }

            return validationErrors;
        }
    }
}
