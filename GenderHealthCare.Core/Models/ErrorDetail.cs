using System.Text.Json.Serialization;

namespace GenderHealthCare.Core.Models
{
    public class ErrorDetail
    {
        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }

        [JsonPropertyName("errorMessage")]
        public object? ErrorMessage { get; set; }
    }
}
