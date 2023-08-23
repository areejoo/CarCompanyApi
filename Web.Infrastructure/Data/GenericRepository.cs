using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Core.Entities;
using Web.Core.Interfaces;
using Web.Infrastructure.Data;
using System.Linq;
using System.Data;

namespace Web.Infrastructure.Data
{

    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly MyAppDbContext context;
        private readonly DbSet<T> entities;
        string errorMessage = string.Empty;
        public GenericRepository(MyAppDbContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public IQueryable<T> GetQueryable()
        {
           

            return entities.AsQueryable();

        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await entities.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(T entity)
        {
          
            await entities.AddAsync(entity);
           // context.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            IQueryable inFiles = GetQueryable();
            foreach (Car inFile in inFiles)
                context.Entry<Car>(inFile).State = EntityState.Detached;


            context.Update(entity);
           
        }
        public async Task DeleteAsync(Guid id)
        {

            var entity = await entities.FirstOrDefaultAsync(s => s.Id == id);
            if (entity != null)
            {
                context.Remove(entity);

               // await context.SaveChangesAsync();
            }
            else
            {
                throw new System.Data.Entity.Core.ObjectNotFoundException();
            }
        }
    }
}