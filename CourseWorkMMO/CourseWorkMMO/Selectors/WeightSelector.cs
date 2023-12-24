using CourseWorkMMO.Items;
using CourseWorkMMO.Elements;

namespace CourseWorkMMO.Selectors
{
    public class WeightSelector : Selector
    {
        private static readonly Random _rand = new();
        private readonly List<(Element? el, int weight)> _nextElements = new();
        private int _weightSum;

        public void AddNextElement(Element? element, int weight)
        {
            if (weight <= 0)
                throw new ArgumentException("Weight must be more than 0");
            _nextElements.Add((element, weight));
            _weightSum += weight;
        }

        public override Element? ChooseNextElement(Item _)
        {
            int randVal = _rand.Next(_weightSum);
            int currentWeight = 0;
            foreach (var (el, weight) in _nextElements)
            {
                if (randVal <= currentWeight)
                    return el;
                currentWeight += weight;
            }
            return null;
        }
    }
}
