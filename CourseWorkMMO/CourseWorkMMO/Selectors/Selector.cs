using CourseWorkMMO.Items;
using CourseWorkMMO.Elements;

namespace CourseWorkMMO.Selectors
{
    public abstract class Selector
    {
        public abstract Element? ChooseNextElement(Item item);
    }
}
