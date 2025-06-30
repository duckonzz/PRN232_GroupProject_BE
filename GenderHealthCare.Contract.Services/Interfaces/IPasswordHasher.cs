namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string inputPassword);
    }
}
