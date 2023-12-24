using CourseWorkMMO.Items;
using CourseWorkMMO.Queues;
using CourseWorkMMO.Selectors;
using CourseWorkMMO.Generators;

namespace CourseWorkMMO.Elements
{
    public class ComplexProcess : Process
    {
        private readonly List<Process> _subProcesses = new();
        private List<Process> _eventProcesses = new();
        public bool PartlyWorking { get; private set; }

        private double _currentTime;
        public override double CurrentTime
        {
            get { return _currentTime; }
            set
            {
                if (PartlyWorking) WorkingTime += value - _currentTime;
                WorkingSubprocessSum += _subProcesses.Count(p => p.FullWorking) * (value - _currentTime);
                Queue.UpdateQueueSizeSum(_currentTime, value);
                _currentTime = value;
                foreach (Process process in _subProcesses)
                    process.CurrentTime = _currentTime;
            }
        }

        public override int WorkingProcesses
        {
            get { return _subProcesses.Where(p => p.FullWorking).Count(); }
        }

        private Action<Item>? _addition = null;
        public override Action<Item>? Addition 
        { 
            get { return _addition; }
            set 
            {
                _addition = value;
                foreach (Process process in _subProcesses)
                    process.Addition = _addition;
            }
        }

        public double WorkingSubprocessSum { get; private set; }


        public ComplexProcess(string name, IGenerator delayGenerator, Selector selector, Queue queue, int subProcessesCount) 
            : base(name, delayGenerator, selector, queue)
        {
            for (int i = 0; i < subProcessesCount; i++)
                _subProcesses.Add(new($"{i + 1}", delayGenerator, Selector, 0));
        }

        public ComplexProcess(string name, IGenerator delayGenerator, Selector selector, Queue queue, List<Process> subProcess) 
            : base(name, delayGenerator, selector, queue) => _subProcesses = subProcess;

        public ComplexProcess(string name, IGenerator delayGenerator, Selector selector, int queueMaxSize, int subProcessesCount)
            : this(name, delayGenerator, selector, new Queue(queueMaxSize), subProcessesCount) { }

        public ComplexProcess(string name, IGenerator delayGenerator, Queue queue, int subProcessesCount)
            : this(name, delayGenerator, new WeightSelector(), queue, subProcessesCount) { }

        public ComplexProcess(string name, IGenerator delayGenerator, int queueMaxSize, int subProcessesCount)
            : this(name, delayGenerator, new WeightSelector(), new Queue(queueMaxSize), subProcessesCount) { }

        public override void MoveTo(Item item)
        {
            if (Queue.IsFull && FullWorking)
            {
                FailureCount++;
                return;
            }
            if (FullWorking)
            {
                Queue.Enqueue(item);
                return;
            }
            Process subProcess = _subProcesses.Where(p => !p.FullWorking).First();
            subProcess.MoveTo(item);
            CheckWorkingStatus();
            UpdateNextTime();
        }

        public override void NextStep()
        {
            _eventProcesses = _subProcesses.Where(p => p.NextTime == NextTime).ToList();
            foreach (Process process in _eventProcesses)
            {
                CountFinished++;
                process.NextStep();
                if (!Queue.IsEmpty)
                    process.MoveTo(Queue.Dequeue());
            }
            CheckWorkingStatus();
            UpdateNextTime();
        }

        private void CheckWorkingStatus()
        {
            if (_subProcesses.All(p => p.FullWorking))
            {
                FullWorking = true;
                PartlyWorking = true;
                return;
            }
            FullWorking = false;
            PartlyWorking = _subProcesses.Any(p => p.FullWorking);
        }

        public override void UpdateNextTime() => NextTime = _subProcesses.Min(p => p.NextTime);

        public override void UnblockOnStart()
        {
            if (FullWorking) return;
            if (!Queue.IsEmpty)
            {
                List<Process> processes = _subProcesses.Where(p => !p.FullWorking).ToList();
                foreach (var process in processes)
                {
                    process.MoveTo(Queue.Dequeue());
                    if (Queue.IsEmpty) break;
                }
                CheckWorkingStatus();
                UpdateNextTime();
            }
        }

        public override void UnblockOnFinish()
        {
            if (!PartlyWorking) return;
            while (NextTime <= CurrentTime)
                NextStep();
        }

        public override void AddGeneratorForType(int type, IGenerator generator)
        {
            if (TypeGenerator.Any(el => el.type == type))
                throw new ArgumentException("There is already generator for this type");
            TypeGenerator.Add((type, generator));
            foreach (var process in _subProcesses)
                process.AddGeneratorForType(type, generator);
        }

        public override void PrintEvent()
        {
            foreach (var process in _eventProcesses)
                Console.Write($"\nEvent happened in {Name}.{process.Name} Moved to {process.MovedTo}");
        }

        public override void PrintStatistic()
        {
            Console.Write($"\n{Name}");
            Console.Write($", Working processes: {WorkingProcesses}");
            Console.Write($", Queue: {Queue.QueueSize}");
            Console.Write($", Failure: {FailureCount}");
            Console.Write($", Next time: {(NextTime == double.MaxValue ? "-" : NextTime)}");
        }

        public override void PrintResults()
        {
            base.PrintResults();
            Console.Write($", Avarage working processes: {WorkingSubprocessSum / _currentTime}");
        }
    }
}
