using CourseWorkMMO.Elements;

namespace CourseWorkMMO
{
    public class Model
    {
        public bool PrintingSteps { get; set; }
        public bool PrintingResults { get; set; }
        public Func<List<Element>, bool>? Addition { get; set; } = null;
        private readonly List<Dispose> _disposes = new();
        private readonly List<Element> _elements;
        private int _step;
        private double _currTime;
        private double _dif;

        public Model(List<Element> elements)
        {
            _elements = elements;
            Dispose GeneralDispose = new();
            foreach (var el in elements)
            {
                el.PersonalDispose ??= GeneralDispose;
                if (!_disposes.Contains(el.PersonalDispose))
                    _disposes.Add(el.PersonalDispose);
            }
        }

        public void Simulate(double totalTime)
        {
            Clear();
            StatsHelper.EvaluateStartingItems(_elements);
            List<Element> nextElements = new();
            if (PrintingSteps) PrintSteps();
            double nextTime = _elements.Min(el => el.NextTime);
            while (nextTime < totalTime)
            {
                _dif = nextTime - _currTime;
                _currTime = nextTime;
                _elements.ForEach(el => el.CurrentTime = _currTime);
                nextElements = _elements.Where(el => el.NextTime == _currTime).ToList();
                nextElements.ForEach(el => el.NextStep());
                if (PrintingSteps) PrintSteps(nextElements);
                StatsHelper.EvaluateStepStatistics(_elements, _dif, _currTime);
                if (Addition?.Invoke(_elements) == true)
                {
                    _elements.ForEach(el => el.PrintStatistic());
                    StatsHelper.AdditionalEventHappened++;
                }
                nextTime = _elements.Min(el => el.NextTime);
            }
            if (PrintingResults) PrintResults();
        }

        private void Clear()
        {
            _step = 0;
            _currTime = 0;
            _disposes.ForEach(d => d.Clear());
            StatsHelper.Clear();
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
            Console.Write($"\nAvarage items in model: {StatsHelper.AvarageItemsInModelSum / _currTime}");
            foreach (int type in StatsHelper.TotalLifeTimesType.Keys)
            {
                if (type == 0) Console.Write($"\nStorm avarage life time: {StatsHelper.AvarageLifeTimeType(type)}");
                else Console.Write($"\nShip type {type} avarage life time: {StatsHelper.AvarageLifeTimeType(type)}");
            }
            Console.Write($"\nShip type 1-3 avarage life time: {StatsHelper.AvarageLifeTimeType(new List<int>() { 1, 2, 3})}");
            Console.Write($"\nShip all types avarage life time: {StatsHelper.AvarageLifeTimeType(new List<int>() { 1, 2, 3, 4 })}");
            Console.Write($"\nAdditional event happened: {StatsHelper.AdditionalEventHappened}");
            Console.WriteLine();
        }
    }
}
