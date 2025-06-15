using GenderHealthCare.Contract.Repositories.Interfaces;
using GenderHealthCare.Repositories.Base;

namespace GenderHealthCare.Repositories.Repositories
{
    public class UnitOfWork(GenderHealthCareDbContext dbContext) : IUnitOfWork
    {
        private bool disposed = false;
        private readonly GenderHealthCareDbContext _dbContext = dbContext;

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            return new GenericRepository<T>(_dbContext);
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void BeginTransaction()
        {
            _dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _dbContext.Database.CommitTransaction();
        }

        public void RollBack()
        {
            _dbContext.Database.RollbackTransaction();
        }
    }
}
