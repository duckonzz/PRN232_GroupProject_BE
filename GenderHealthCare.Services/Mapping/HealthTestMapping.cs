using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.HealthTestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Services.Mapping
{
    public static class HealthTestMapping
    {
        public static HealthTestResponseModel ToHealthTestDto(this HealthTest healthTest)
        {
            if (healthTest == null)
                return null;

            return new HealthTestResponseModel
            {
                Id = healthTest.Id,
                Name = healthTest.Name,
                Description = healthTest.Description,
                Price = healthTest.Price,
                CreatedTime = healthTest.CreatedTime,
            };
        }
    }
}
