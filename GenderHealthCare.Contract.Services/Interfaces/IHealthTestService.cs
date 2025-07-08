
using GenderHealthCare.ModelViews.HealthTestModels;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IHealthTestService
    {
        // Basic CRUD operations
        Task<HealthTestResponseModel> CreateHealthTestAsync(HealthTestRequestModel model);
        Task<HealthTestResponseModel?> GetHealthTestByIdAsync(string id);
        Task<List<HealthTestResponseModel>> GetAllHealthTestsAsync();
        Task<bool> UpdateHealthTestAsync(string id, HealthTestRequestModel model);
        Task<bool> DeleteHealthTestAsync(string id);

        // Additional business methods
        //Task<List<HealthTestResponseModel>> GetTestsByCategoryAsync(string category);
        Task<List<HealthTestResponseModel>> SearchTestsAsync(string keyword);
    }
}
