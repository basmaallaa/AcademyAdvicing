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
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task AddAsync(T item);
        void Delete(T item);
        void Update(T item);
        IEnumerable<object> GetQueryable();
    }
}
