namespace GenderHealthCare.ModelViews.AuthenticationModels
{
    public class UserResponseModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Role { get; set; }
        public bool IsCycleTrackingOn { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
    }
}
