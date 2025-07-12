using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.ModelViews.StatisticsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Services.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITestSlotRepository _testSlotRepo;

        public StatisticsService(IUserRepository userRepo, ITestSlotRepository testSlotRepo)
        {
            _userRepo = userRepo;
            _testSlotRepo = testSlotRepo;
        }

        public async Task<ServiceResponse<UserStatisticsResultDto>> CountAllCustomersAsync()
        {
            var count = await _userRepo.CountAllCustomersAsync();

            return new ServiceResponse<UserStatisticsResultDto>
            {
                Data = new UserStatisticsResultDto
                {
                    CustomerCount = count
                },
                Success = true,
                Message = "Customer count retrieved successfully."
            };
        }

        public async Task<ServiceResponse<TestSlotStatisticsResultDto>> CountBookedTestSlotsAsync()
        {
            var count = await _testSlotRepo.CountBookedTestSlotsAsync();
            return new ServiceResponse<TestSlotStatisticsResultDto>
            {
                Data = new TestSlotStatisticsResultDto { BookedTestSlotCount = count },
                Success = true,
                Message = "Booked test slot count retrieved successfully."
            };
        }

        public async Task<ServiceResponse<AvailableSlotStatisticsResultDto>> CountBookedAvailableSlotsAsync()
        {
            var count = await _userRepo.CountBookedAvailableSlotsAsync();
            return new ServiceResponse<AvailableSlotStatisticsResultDto>
            {
                Data = new AvailableSlotStatisticsResultDto { BookedAvailableSlotCount = count },
                Success = true,
                Message = "Booked consultation slot count retrieved successfully."
            };
        }
    }
}
