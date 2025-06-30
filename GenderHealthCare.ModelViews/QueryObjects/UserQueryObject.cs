using GenderHealthCare.Core.Enums;

namespace GenderHealthCare.ModelViews.QueryObjects
{
    public class UserQueryObject : BaseQueryObject
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsCycleTrackingOn { get; set; }
        public Role? Role { get; set; }
        public ConsultantStatus? ConsultantStatus { get; set; }
    }
}
