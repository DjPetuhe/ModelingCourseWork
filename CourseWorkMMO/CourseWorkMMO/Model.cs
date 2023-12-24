using CourseWorkMMO.Elements;

namespace CourseWorkMMO
{
    public class Model
    {
        public Func<List<Element>, bool>? Addition { get; set; } = null;
        private readonly List<Element> _elements;
        private int _additionalEventHappened;
        private double _avarageItemsInModelSum;
        private int _startingItems;
        private int _step;
        private double _currTime;
        private double dif;

        public Model(List<Element> elements) => _elements = elements;

        public void Simulate(double totalTime)
        {
            _step = 0;
            _currTime = 0;
            _startingItems = 0;
            foreach (var el in _elements.OfType<Process>())
            {
                _startingItems += el.Queue.QueueSize;
                _startingItems += el.WorkingProcesses;
            }
            GeneralDispose.Clear();
            List<Element> nextElements = new();
            PrintSteps();
            double nextTime = _elements.Min(el => el.NextTime);
            while (nextTime < totalTime)
            {
                dif = nextTime - _currTime;
                _currTime = nextTime;
                _elements.ForEach(el => el.CurrentTime = _currTime);
                nextElements = _elements.Where(el => el.NextTime == _currTime).ToList();
                nextElements.ForEach(el => el.NextStep());
                PrintSteps(nextElements);
                EvaluateStatistics();
                if (Addition?.Invoke(_elements) == true)
                {
                    _elements.ForEach(el => el.PrintStatistic());
                    _additionalEventHappened++;
                }
                nextTime = _elements.Min(el => el.NextTime);
            }
            PrintResults();
        }

        private void EvaluateStatistics()
        {
            int itemsInModel = _elements.OfType<Create>().Sum(cr => cr.Created) + _startingItems - GeneralDispose.Destroyed;
            _avarageItemsInModelSum += itemsInModel * dif;
        }

        private void PrintSteps() => PrintSteps(new());

        private void PrintSteps(List<Element> nextElements)
        {
            _step++;
            Console.Write($"\n\nStep #{_step}");
            Console.Write($"\nCurrent time: {_currTime}");
            nextElements.ForEach(el => el.PrintEvent());
            _elements.ForEach(el => el.PrintStatistic());
        }

        private void PrintResults()
        {
            Console.Write("\n\n" + new string('=', 30) + "RESULT" + new string('=', 30));
            _elements.ForEach(el => el.PrintResults());
            int totalCreated = _elements.OfType<Create>().Sum(cr => cr.Created);
            Console.Write($"\nAvarage items in model: {_avarageItemsInModelSum / _currTime}");
            //Console.Write($"\nAvarage time for item in model: {GeneralDispose.AvarageLifeTime}");
            //Console.Write($"\nAvarage time between dispose: {_currTime / GeneralDispose.Destroyed}");
            Console.Write($"\nAdditional event happened: {_additionalEventHappened}");
            Console.WriteLine();
        }

        public void PrintHospitalResults()
        {
            Console.Write("\n\n" + new string('=', 30) + "HOSPITAL RESULT BY TYPE" + new string('=', 30));
            Process lab = _elements.OfType<Process>().First(el => el.Name.Equals("LabRegistry")) ?? throw new Exception("No lab exist");
            int totalLab = lab.Queue.QueueSize + lab.WorkingProcesses + lab.CountFinished;
            Console.WriteLine($"\nAvarage time between lab arrival: {_currTime / totalLab}");
            foreach (int type in GeneralDispose.TotalLifeTimesType.Keys)
                Console.WriteLine($"Patient type {type} avarage life time: {GeneralDispose.AvarageLifeTimeType(type)}");
        }
    }
}
