using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ECommerceSharedLibrary.Responses;

namespace ECommerceSharedLibrary.Interface
{
    public interface IGenericCRUDInterface<T>
        where T : class
    {
        Task<Response> CreateAsync(T entity);
        Task<Response> DeleteAsync(T entity);
        Task<Response> UpdateAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<Response> GetByAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllByAsync(Expression<Func<T, bool>> predicate);
    }
}
