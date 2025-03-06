using Academy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Academy.Core.Repositories
{
    public interface IGenaricRepository<T> where T : class 
    {
        Task<IQueryable<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task AddAsync(T item);
        void Delete(int id);
        void Update(T item);

    }
}
