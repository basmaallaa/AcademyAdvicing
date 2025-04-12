using Academy.Core.Repositories;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core
{
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync();

        IGenaricRepository<T> Repository<T>() where T : class;

        
    }
}
