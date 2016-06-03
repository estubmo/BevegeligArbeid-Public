// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
using System;

namespace BevegeligArbeid.Collections
{
    using System.Threading;


    using Nito;

    public class BoundedBlockingQueue<T>
    {
        private readonly Deque<T> queue = new Deque<T>();
        private readonly int maxSize;
        private bool closing;

        public BoundedBlockingQueue(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public void Enqueue(T item)
        {
            lock (this.queue)
            {
                if (this.queue.Count == this.maxSize)
                {
					System.Diagnostics.Debug.WriteLine("Bounded queued passed size threshold.");
                    this.ThrowAway(1);
                }
                this.queue.AddToBack(item);
				//System.Diagnostics.Debug.WriteLine(String.Format("Queue size: {0}", this.queue.Count));
                if (this.queue.Count == 1)
                {
                    // wake up any blocked dequeue
                    Monitor.PulseAll(this.queue);
                }
            }
        }

        private void ThrowAway(int n)
        {
            for (int i = 0; i < n; i++)
            {
                this.queue.RemoveFromFront();
            }
			System.Diagnostics.Debug.WriteLine(String.Format("Threw away {0} old items.", n));
        }

        public void Clear()
        {
            lock (this.queue)
            {
                this.queue.Clear();
            }
        }

        public bool TryDequeue(out T value)
        {
			//System.Diagnostics.Debug.WriteLine("D-Q 1");
            lock (this.queue)
            {
			//	System.Diagnostics.Debug.WriteLine("D-Q 2");
                while (this.queue.Count == 0)
                {
					//System.Diagnostics.Debug.WriteLine("D-Q 3");
                    if (this.closing)
                    {
						System.Diagnostics.Debug.WriteLine("D-Q 4");
                        value = default(T);
                        return false;
                    }
                    Monitor.Wait(this.queue);
                }
                value = this.queue.RemoveFromFront();
			//	System.Diagnostics.Debug.WriteLine(String.Format("Queue size: {0}", this.queue.Count));
                if (this.queue.Count == this.maxSize - 1)
                {
					System.Diagnostics.Debug.WriteLine("D-Q 5");
                    // wake up any blocked enqueue
                    Monitor.PulseAll(this.queue);
                }
                return true;
            }
        }

        /*public T Dequeue()
        {
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(queue);
                }
                T item = queue.Dequeue();
                if (queue.Count == maxSize - 1)
                {
                    // wake up any blocked enqueue
                    Monitor.PulseAll(queue);
                }
                return item;
            }
        }*/

        public void Close()
        {
            lock (this.queue)
            {
                this.closing = true;
                Monitor.PulseAll(this.queue);
            }
        }
    }
}