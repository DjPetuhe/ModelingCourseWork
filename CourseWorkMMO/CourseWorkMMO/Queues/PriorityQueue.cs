using CourseWorkMMO.Items;

namespace CourseWorkMMO.Queues
{
    public class PriorityQueue : Queue
    {
        private readonly SortedList<int, List<Predicate<Item>>> _priorities = new();

        public PriorityQueue(int queueMaxSize) : base(queueMaxSize) { }

        public void AddPriority(int priority, Predicate<Item> condition)
        {
            if (priority <= 0)
                throw new ArgumentException("priority must be positive number");
            if (!_priorities.ContainsKey(priority))
                _priorities.Add(priority, new List<Predicate<Item>>());
            _priorities[priority].Add(condition);
        }

        public new Item Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Queue is empty.");
            foreach(var priority in _priorities)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    foreach(var condition in priority.Value)
                    {
                        if (condition(Items[i]))
                        {
                            Item next = Items[i];
                            Items.RemoveAt(i);
                            return next;
                        }
                    }
                }
            }
            return base.Dequeue();
        }
    }
}
