using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrioQueue
{
    public class PriorityQueue<TElement, TKey>
    {
        private SortedDictionary<TKey, Queue<TElement>> dictionary = new SortedDictionary<TKey, Queue<TElement>>();

        public PriorityQueue()
        {
        }

        public void Enqueue(TElement item, TKey value)
        {
            Queue<TElement> queue;
            if (!dictionary.TryGetValue(value, out queue))
            {
                queue = new Queue<TElement>();
                dictionary.Add(value, queue);
            }

            queue.Enqueue(item);
        }

        public TElement Dequeue()
        {
            if (dictionary.Count == 0)
                throw new Exception("No items to Dequeue:");
            var key = dictionary.Keys.First();

            var queue = dictionary[key];
            var output = queue.Dequeue();
            if (queue.Count == 0)
                dictionary.Remove(key);

            return output;
        }

        public bool IsEmpty()
        {
            return dictionary.Count == 0;
        }
    }
}