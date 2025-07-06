using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.CycleTrackingModels;

namespace GenderHealthCare.Services.Mapping
{
    public static class CycleTrackingMapper
    {
        public static CycleTrackingResponse ToCycleTrackingDto(this ReproductiveCycle cycle)
        {

            var ovulationDate = cycle.StartDate.AddDays(cycle.CycleLength - 14);
            var upcomingPeriod = cycle.StartDate.AddDays(cycle.CycleLength);
            var fertileWindowStart = ovulationDate.AddDays(-3);
            var fertileWindowEnd = ovulationDate.AddDays(1);

            return new CycleTrackingResponse
            {
                Id = cycle.Id,
                StartDate = cycle.StartDate,
                EndDate = cycle.EndDate,
                CycleLength = cycle.CycleLength,
                PeriodLength = cycle.PeriodLength,
                Notes = cycle.Notes,

                OvulationDate = ovulationDate,
                UpcomingPeriod = upcomingPeriod,
                FertileWindowStart = fertileWindowStart,
                FertileWindowEnd = fertileWindowEnd
            };
        }

        public static List<CycleTrackingResponse> ToCycleTrackingDtoList(this IEnumerable<ReproductiveCycle> cycles)
        {
            return cycles.Select(c => c.ToCycleTrackingDto()).ToList();
        }
    }
}
