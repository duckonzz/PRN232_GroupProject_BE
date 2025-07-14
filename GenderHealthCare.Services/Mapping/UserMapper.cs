using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.AuthenticationModels;

namespace GenderHealthCare.Services.Mapping
{
    public static class UserMapper
    {
        public static UserResponseModel ToUserDto(this User userModel)
        {
            return new UserResponseModel
            {
                Id = userModel.Id,
                FullName = userModel.FullName,
                Email = userModel.Email,
                PhoneNumber = userModel.PhoneNumber,
                DateOfBirth = userModel.DateOfBirth,
                Gender = userModel.Gender,
                Role = userModel.Role,
                IsCycleTrackingOn = userModel.IsCycleTrackingOn,
                CreatedTime = userModel.CreatedTime,
                HasConsultantProfile = userModel.ConsultantProfile != null
            };
        }

        public static List<UserResponseModel> ToListUserDto(this IEnumerable<User> userList)
        {
            return userList.Select(u => u.ToUserDto()).ToList();
        }
    }
}
