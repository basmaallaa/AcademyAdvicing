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
<<<<<<< HEAD
        void Delete(int id);
       //Task Delete(int id)
=======
        void Delete(T item);
>>>>>>> 57f5f3ac5fdf27a54ae57d64aec420d0d500fc17
        void Update(T item);
        IEnumerable<object> GetQueryable();
    }
}
