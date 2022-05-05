using System;
using System.Collections.Generic;

namespace GraphQL.Spike
{
    public interface IRepository<K, T>
    {
        void Add(K id, T person);
        T GetById(K id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Get(Func<T, bool> condition);
    }
}
