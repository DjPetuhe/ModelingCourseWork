using CourseWorkMMO.Items;
using CourseWorkMMO.Queues;
using CourseWorkMMO.Selectors;
using CourseWorkMMO.Generators;
using System.Xml.Linq;

namespace CourseWorkMMO.Elements
{
    public class Process : Element
    {
        public int CountFinished { get; protected set; }

        private bool _fullWorking;
        public bool FullWorking
        {
            get { return _fullWorking; }
            protected set
            {
                if (value)
                {
                    BlockingOnFinishWhenWork.ForEach(p => p.BlockingOnFinish++);
                    BlockingOnStartWhenWork.ForEach(p => p.BlockingOnStart++);
                }
                else
                {
                    BlockingOnFinishWhenWork.ForEach(p => p.BlockingOnFinish--);
                    BlockingOnStartWhenWork.ForEach(p => p.BlockingOnStart--);
                }    
                _fullWorking = value;
            }
        }

        public int FailureCount { get; protected set; }
        public double FailurePercent
        {
            get { return CountFinished == 0 ? 0 : Math.Round((double)FailureCount * 100 / (FailureCount + CountFinished), 3); }
        }

        public double WorkingTimePercent
        {
            get { return Math.Round(WorkingTime * 100 / CurrentTime, 3); }
        }

        private double _currentTime;
        public override double CurrentTime
        {
            get { return _currentTime; }
            set
            {
                if (FullWorking) WorkingTime += value - _currentTime;
                Queue.UpdateQueueSizeSum(_currentTime, value);
                _currentTime = value;
            }
        }

        public double WorkingTime { get; protected set; }

        public virtual int WorkingProcesses
        {
            get { return FullWorking ? 1 : 0; }
        }

        private int _blockingOnStart; 
        public int BlockingOnStart
        {
            get { return _blockingOnStart; }
            set
            {
                if (value < 0) throw new ArgumentException("Element already unblocked on start!");
                if (_blockingOnStart > 0 && value == 0) UnblockOnStart();
                _blockingOnStart = value;
            }
        }


        private int _blockingOnFinish;
        public int BlockingOnFinish
        {
            get { return _blockingOnFinish; }
            set
            {
                if (value < 0) throw new ArgumentException("Element already unblocked on finish!");
                if (_blockingOnFinish > 0 && value == 0) UnblockOnFinish();
                _blockingOnFinish = value;
            }
        }

        private double _nextTime;
        public override double NextTime
        {
            get
            {
                if (_blockingOnFinish > 0) return double.MaxValue;
                return _nextTime;
            }
            protected set { _nextTime = value; }
        }

        public Queue Queue { get; }
        protected Item? WorkingOn { get; set; }
        public virtual Action<Item>? Addition { get; set; } = null;
        public readonly List<Process> BlockingOnStartWhenWork = new();
        public readonly List<Process> BlockingOnFinishWhenWork = new();
        protected List<(int type, IGenerator generator)> TypeGenerator { get;} = new();  

        public Process(string name, IGenerator delayGenerator, Selector selector, Queue queue)
            : base(name, delayGenerator, selector) => Queue = queue;

        public Process(string name, IGenerator delayGenerator, Selector selector, int queueMaxSize)
            : this(name, delayGenerator, selector, new Queue(queueMaxSize)) { }

        public Process(string name, IGenerator delayGenerator, Queue queue)
            : this(name, delayGenerator, new WeightSelector(), queue) { }

        public Process(string name, IGenerator delayGenerator, int queueMaxSize)
            : this(name, delayGenerator, new Queue(queueMaxSize)) { }

        public override void MoveTo(Item item)
        {
            if (Queue.IsFull && FullWorking)
            {
                FailureCount++;
                return;
            }
            if (FullWorking || _blockingOnStart > 0)
            {
                Queue.Enqueue(item);
                return;
            }
            FullWorking = true;
            WorkingOn = item;
            UpdateNextTime();
        }

        public override void NextStep()
        {
            CountFinished++;
            Item finishedItem = WorkingOn ?? throw new ArgumentException("Can't finish unexisting item.");
            Addition?.Invoke(finishedItem);
            if (Queue.IsEmpty || _blockingOnStart > 0)
            {
                FullWorking = false;
                NextTime = double.MaxValue;
                WorkingOn = null;
            }
            else
            {
                WorkingOn = Queue.Dequeue();
                UpdateNextTime();
            }
            Element? next = Selector.ChooseNextElement(finishedItem);
            MovedTo = next != null ? next.Name : "Dispose";
            if (next == null)
            {
                if (PersonalDispose == null) GeneralDispose.Destroy(finishedItem, CurrentTime);
                else PersonalDispose.Destroy(finishedItem, CurrentTime);
            }
            else next.MoveTo(finishedItem);
        }

        public virtual void UnblockOnStart()
        {
            if (FullWorking) return;
            if (!Queue.IsEmpty)
            {
                WorkingOn = Queue.Dequeue();
                UpdateNextTime();
            }
        }

        public virtual void UnblockOnFinish()
        {
            if (!FullWorking) return;
            if (NextTime <= CurrentTime) NextStep();
        }

        public override void UpdateNextTime()
        {
            if (WorkingOn == null) throw new ArgumentNullException();
            foreach (var (type, generator) in TypeGenerator)
            {
                if (type == WorkingOn.Type)
                {
                    NextTime = CurrentTime + generator.NextDelay();
                    return;
                }
            }
            base.UpdateNextTime();
        }

        public virtual void AddGeneratorForType(int type, IGenerator generator)
        {
            if (TypeGenerator.Any(el => el.type == type))
                throw new ArgumentException("There is already generator for this type");
            TypeGenerator.Add((type, generator));
        }

        public override void PrintStatistic()
        {
            Console.Write($"\n{Name}");
            Console.Write($", Working: {FullWorking}");
            Console.Write($", Queue: {Queue.QueueSize}");
            Console.Write($", Failure: {FailureCount}");
            Console.Write($", Next time: {(NextTime == double.MaxValue ? "-" : NextTime)}");
            if (_blockingOnStart > 0) Console.Write(", start blocked");
            if (_blockingOnFinish > 0) Console.Write(", finish blocked");
        }

        public override void PrintResults()
        {
            Console.Write($"\n{Name}");
            Console.Write($", total failures: {FailureCount}");
            Console.Write($", total proceed: {CountFinished}");
            Console.Write($", failure percent: {FailurePercent}%");
            Console.Write($", Working time percent: {WorkingTimePercent}%");
            Console.Write($", avarage queue size: {Math.Round(Queue.QueueSizeSum / CurrentTime, 3)}");
        }

        public virtual void SetStartingWorkingOn(Item item, double finishTime)
        {
            WorkingOn = item;
            FullWorking = true;
            NextTime = finishTime;
        }
    }
}
