using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Helpers;

namespace GenderHealthCare.Services.Infrastructure
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCryptHelper.HashPassword(password);
        }

        public bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            return BCryptHelper.VerifyPassword(inputPassword, hashedPassword);
        }
    }
}
