using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using ShoppingAPI.Core.Repositories;

namespace ShoppingAPI.Persistence.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T>
       where T : class
    {
        private DbSet<T> set;

        public BaseRepository(DbSet<T> set)
        {
            this.set = set;
        }

        public virtual IQueryable<T> GetAll()
        {
            return set.AsQueryable();
        }

        public virtual T Find(params object[] keys)
        {
            return set.Find(keys);
        }

        public T First(Expression<Func<T, bool>> expression)
        {
            return set.First(expression);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return set.FirstOrDefault(expression);
        }

        public T Single(Expression<Func<T, bool>> expression)
        {
            return set.Single(expression);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> expression)
        {
            return set.SingleOrDefault(expression);
        }

        public virtual void Add(T entity)
        {
            set.Add(entity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            set.AddRange(entities);
        }

        public virtual void Delete(T entity)
        {
            if (entity != null)
            {
                set.Remove(entity);
            }
        }

        public virtual void Delete(Expression<Func<T, bool>> expression)
        {
            set.RemoveRange(
                GetAll()
                    .Where<T>(expression)
                    .AsEnumerable<T>());
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            set.RemoveRange(entities);
        }
    }
}