using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShoppingAPI.Core.Repositories
{
    public interface IBaseRepository<T>
       where T : class
    {
        IQueryable<T> GetAll();
        T Find(params object[] keys);
        T First();
        T First(Expression<Func<T, bool>> expression);
        T FirstOrDefault(Expression<Func<T, bool>> expression);
        T Single(Expression<Func<T, bool>> expression);
        T SingleOrDefault(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> expression);
        void DeleteRange(IEnumerable<T> entities);
    }
}