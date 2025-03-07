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


        public async Task<IQueryable<T>> GetAllAsync()
        {
            // return (IQueryable<T>)await _context.Set<T>().ToListAsync();
            return _context.Set<T>().AsQueryable();
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
        
        
    }
=======

        public IEnumerable<object> GetQueryable() 
        {
            return _context.Set<T>().AsQueryable().ToList<object>();
        }

 }
>>>>>>> 4fd21ce652db88d6acae8165f1f54dd22ef60408
}
