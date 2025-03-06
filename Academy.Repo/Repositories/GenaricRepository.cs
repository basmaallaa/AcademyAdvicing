using Academy.Core.Repositories;
using Academy.Repo.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Repo.Repositories
{
    public class GenaricRepository<T> : IGenaricRepository<T> where T : class
    {
        private AcademyContext _context;

        public GenaricRepository(AcademyContext context) {
            _context = context; 
        
        }
        public async Task AddAsync(T item)
        {
            await _context.AddAsync(item);
        }

        public void Delete(T item)
        {
            _context.Remove(item);
        }

        /*public async Task Delete(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id); // 🔍 البحث عن الكيان أولاً

            if (entity == null)
            {
                throw new InvalidOperationException($"Entity of type {typeof(T).Name} with ID {id} was not found.");
            }

            _context.Remove(entity); // ✅ الآن يمكن الحذف بأمان
        }*/

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void Update(T item)
        {
            _context.Update(item);
        }
<<<<<<< HEAD

       
=======
        public IEnumerable<object> GetQueryable() 
        {
            return _context.Set<T>().AsQueryable().ToList<object>();
        }
>>>>>>> 57f5f3ac5fdf27a54ae57d64aec420d0d500fc17
    }
}
