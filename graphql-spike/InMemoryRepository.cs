using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Spike
{
    public class InMemoryRepository<K, T> : IRepository<K, T>
    {
        readonly Dictionary<K, T> _repository = new Dictionary<K, T>();

        public void Add(K key, T item) => _repository.Add(key, item);
        public T GetById(K key) => _repository[key];
        public IEnumerable<T> GetAll() => _repository.Values;
        public IEnumerable<T> Get(Func<T, bool> condition) => _repository.Values.Where(condition).ToArray();
        public void Clear() => _repository.Clear();
    }
}
