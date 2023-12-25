using CourseWorkMMO.Items;

namespace CourseWorkMMO.Queues
{
    public class Queue
    {
        protected List<Item> Items { get; } = new();
        public readonly int QueueMaxSize;
        public int QueueSize => Items.Count;
        public double QueueSizeSum { get; protected set; }
        public bool IsFull => QueueSize >= QueueMaxSize;
        public bool IsEmpty => QueueSize == 0;

        public Queue(int queueMaxSize)
            => QueueMaxSize = queueMaxSize;

        public virtual Item Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Queue is empty.");
            Item next = Items[0];
            Items.RemoveAt(0);
            return next;
        }

        public void Enqueue(Item item)
        {
            if (IsFull)
                throw new ArgumentOutOfRangeException(nameof(item), "Queue is full.");
            Items.Add(item);
        }

        public void UpdateQueueSizeSum(double oldTime, double newTime)
            => QueueSizeSum += (newTime - oldTime) * QueueSize;
    }
}
