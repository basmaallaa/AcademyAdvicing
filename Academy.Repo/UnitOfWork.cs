using Academy.Core;
using Academy.Core.Repositories;
using Academy.Repo.Data;
using Academy.Repo.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Repo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AcademyContext _context;
        private Hashtable _repositories;
        public UnitOfWork( AcademyContext context) 
        {
            _context = context;
            _repositories = new Hashtable();
        }
       

       
        public async Task<int> CompleteAsync()
        => await _context.SaveChangesAsync();
        

        public IGenaricRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T).Name;

            // لو الريبو مش موجود ضيفه 
            if(!_repositories.ContainsKey(type))
            {
                var repository = new GenaricRepository<T>(_context);
                _repositories.Add(type, repository);
            }
            // بعد ما ضيفته رجعهولي بقي اشتغل عليه 
            return _repositories[type] as IGenaricRepository<T>;
        }
    
    }
}
