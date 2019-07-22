using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWork
{

        public class EFUnitOfWork : IUnitOfWork
        {
            private readonly Context _dbContext;

            public EFUnitOfWork(Context dbContext)
            {
               // Database.SetInitializer<Context>(null);     //Varolan db üzerinde ilk context bağlantısı için kullanılır. 

                if (dbContext == null)
                    throw new ArgumentNullException("DbContext null olamaz.");

                _dbContext = dbContext;
            
            }

            public IRepository<T> GetRepository<T>() where T : class
            {
                return new EFRepository<T>(_dbContext);
            }

            public int SaveChanges()
            {
                try
                {
                    return _dbContext.SaveChanges();
                }
                catch
                {
                    throw;
                }
            }

            private bool disposed = false;
            protected virtual void Dispose(bool disposing)          //Dispose işlemi garbage collector kullanımını yönetir. 
            {
                if (!this.disposed)
                {
                    if (disposing)
                    {
                        _dbContext.Dispose();
                    }
                }

                this.disposed = true;
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }

