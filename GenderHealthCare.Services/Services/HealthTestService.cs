using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.HealthTestModels;
using GenderHealthCare.Services.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GenderHealthCare.Services.Services
{
    public class HealthTestService : IHealthTestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HealthTestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<HealthTestResponseModel> CreateHealthTestAsync(HealthTestRequestModel model)
        {
            var healthTest = new HealthTest
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                CreatedTime = DateTimeOffset.UtcNow
            };

            await _unitOfWork.GetRepository<HealthTest>().InsertAsync(healthTest);
            await _unitOfWork.SaveAsync();

            return healthTest.ToHealthTestDto();
        }

        public async Task<HealthTestResponseModel?> GetHealthTestByIdAsync(string id)
        {
            var healthTest = await _unitOfWork.GetRepository<HealthTest>()
                .Entities
                .FirstOrDefaultAsync(ht => ht.Id == id);

            return healthTest?.ToHealthTestDto();
        }

        public async Task<List<HealthTestResponseModel>> GetAllHealthTestsAsync()
        {
            var healthTests = await _unitOfWork.GetRepository<HealthTest>()
                .Entities
                .ToListAsync();

            return healthTests.Select(ht => ht.ToHealthTestDto())
                            .OrderByDescending(ht => ht.CreatedTime)
                            .ToList();
        }

        //public async Task<List<HealthTestResponseModel>> GetTestsByCategoryAsync(string category)
        //{
        //    var healthTests = await _unitOfWork.GetRepository<HealthTest>()
        //        .Entities
        //        .Where(ht => ht.Name.Contains(category) ||
        //                   (ht.Description != null && ht.Description.Contains(category)))
        //        .ToListAsync();

        //    return healthTests.Select(ht => ht.ToHealthTestDto()).ToList();
        //}

        public async Task<List<HealthTestResponseModel>> SearchTestsAsync(string keyword)
        {
            var healthTests = await _unitOfWork.GetRepository<HealthTest>()
                .Entities
                .Where(ht => ht.Name.Contains(keyword) ||
                           (ht.Description != null && ht.Description.Contains(keyword)))
                .ToListAsync();

            return healthTests.Select(ht => ht.ToHealthTestDto()).ToList();
        }

        public async Task<bool> UpdateHealthTestAsync(string id, HealthTestRequestModel model)
        {
            var healthTestRepo = _unitOfWork.GetRepository<HealthTest>();
            var healthTest = await healthTestRepo.GetByIdAsync(id);

            if (healthTest == null) return false;

            healthTest.Name = model.Name;
            healthTest.Description = model.Description;
            healthTest.Price = model.Price;

            healthTestRepo.Update(healthTest);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteHealthTestAsync(string id)
        {
            var healthTestRepo = _unitOfWork.GetRepository<HealthTest>();
            var healthTest = await healthTestRepo.GetByIdAsync(id);

            if (healthTest == null) return false;

            healthTestRepo.Delete(id);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}