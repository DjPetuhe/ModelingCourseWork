using CourseWorkMMO.Elements;
using CourseWorkMMO.Items;

namespace CourseWorkMMO.Selectors
{
    public class ProbabilitySelector : Selector
    {
        private static readonly Random _rand = new();
        private readonly List<(Element? el, double probability)> _nextElements = new();
        private double _probabilitySum;

        public void AddNextElement(Element? element, double probability)
        {
            if (probability <= 0)
                throw new ArgumentException("Probability must be more than 0");
            if (_probabilitySum + probability > 1)
                throw new ArgumentException("Probability can't be more than 1");
            _nextElements.Add((element, probability));
            _probabilitySum += probability;
        }

        public override Element? ChooseNextElement(Item _)
        {
            double randVal = _rand.NextDouble();
            double currentProbability = 0;
            foreach (var (el, probability) in _nextElements)
            {
                if (randVal <= currentProbability)
                    return el;
                currentProbability += probability;
            }
            return null;
        }
    }
}
