using CourseWorkMMO.Items;
using CourseWorkMMO.Elements;

namespace CourseWorkMMO.Selectors
{
    public class TypeSelector : Selector
    {
        private readonly List<(int type, Element? element)> _nextElements = new();

        public void AddElementForType(int type, Element? element)
        {
            if (_nextElements.Any(el => el.type == type))
                throw new ArgumentException("There is already transition for this type");
            _nextElements.Add((type, element));
        }

        public override Element? ChooseNextElement(Item item)
        {
            foreach (var (type, element) in _nextElements)
            {
                if (type == item.Type)
                    return element;
            }
            return null;
        }
    }
}
