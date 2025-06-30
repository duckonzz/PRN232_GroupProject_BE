using GenderHealthCare.Core.Enums;

namespace GenderHealthCare.ModelViews.AuthenticationModels
{
    public class UpdateUserRequest
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public bool? IsCycleTrackingOn { get; set; }
    }
}
