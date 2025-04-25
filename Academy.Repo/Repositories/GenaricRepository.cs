using Academy.Core.Repositories;
using Academy.Repo.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Repo.Repositories
{
    public class GenaricRepository<T> : IGenaricRepository<T> where T : class
    {
        private AcademyContext _context;

        public GenaricRepository(AcademyContext context)
        {
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


        public async Task<IQueryable<T>> GetAllAsync()
        {
            // return (IQueryable<T>)await _context.Set<T>().ToListAsync();
            return _context.Set<T>().AsQueryable();
        }
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }
        public async Task<T> GetAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void Update(T item)
        {
            _context.Update(item);
        }


        public IEnumerable<object> GetQueryable()

        {
            return _context.Set<T>().AsQueryable().ToList<object>();
        }


        // من هنا البوست
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate); // استخدم Where من LINQ
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllIncludingAsync(Expression<Func<T, bool>> predicate,
                    params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(predicate).ToListAsync();
        }
        public async Task<T> GetOneIncludingAsync(
    Expression<Func<T, bool>> predicate,
    params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }


        public async Task<IEnumerable<T>> GetFilteredAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public async Task<T> GetIncludingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllIncludingAsyncc(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            // Apply each Include
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await _context.Set<T>().FindAsync(id);
        }



    }

    }





    







 


