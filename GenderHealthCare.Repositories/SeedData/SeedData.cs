using Microsoft.EntityFrameworkCore;
using GenderHealthCare.Repositories.Base;

namespace GenderHealthCare.Repositories.SeedData
{
    public class SeedData
    {
        private readonly GenderHealthCareDbContext _context;

        public SeedData(GenderHealthCareDbContext context)
        {
            _context = context;
        }

        public async Task Initialise()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    bool dbExists = _context.Database.CanConnect();
                    if (!dbExists)
                    {
                        _context.Database.Migrate();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _context.Dispose();
            }
        }
    }
}
