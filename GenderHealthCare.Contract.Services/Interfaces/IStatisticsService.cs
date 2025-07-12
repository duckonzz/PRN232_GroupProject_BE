using GenderHealthCare.Core.Helpers;
using GenderHealthCare.ModelViews.StatisticsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<ServiceResponse<UserStatisticsResultDto>> CountAllCustomersAsync();
        Task<ServiceResponse<TestSlotStatisticsResultDto>> CountBookedTestSlotsAsync();
        Task<ServiceResponse<AvailableSlotStatisticsResultDto>> CountBookedAvailableSlotsAsync(); 
    }
}
