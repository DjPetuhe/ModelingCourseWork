using CourseWorkMMO.Items;
using CourseWorkMMO.Elements;

namespace CourseWorkMMO.Selectors
{
    public class QueuePrioritySelector : Selector
    {
        private readonly List<Process?> _nextProcesses = new();

        public void AddNextProcess(Process? proc) 
            => _nextProcesses.Add(proc);

        public override Element? ChooseNextElement(Item item)
        {
            (int queueSize, Process? pr) minQueueEl = (int.MaxValue, null);
            foreach(var process in _nextProcesses) 
            {
                if (process != null && process.Queue.QueueSize < minQueueEl.queueSize)
                    minQueueEl = (process.Queue.QueueSize, process);
            }
            return minQueueEl.pr;
        }
    }
}
