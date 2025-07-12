namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IUserContextService
    {   
        string GetUserId();
        string? GetUserRole();
        Task<string> GetConsultantIdAsync();

    }
}