using System.Collections;
using System.Collections.Generic;

namespace AppName.Web.DataStructures
{
    public class MaxCapacityCollection<TCapacity, TItem> : IEnumerable<TItem> where TCapacity: CapacityDefinition, new()
    {
        private Queue<TItem> _items;

        public MaxCapacityCollection()
        {
            var capacity = new TCapacity();
            _items = new Queue<TItem>(capacity.Amount);
        }
        
        public void Add(TItem item)
        {
            if (_items.Count == 10)
            {
                _items.Dequeue();
            }

            _items.Enqueue(item);
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}