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

        
       //Task Delete(int id)

        void Delete(T item);

        void Update(T item);
       
    }
}
