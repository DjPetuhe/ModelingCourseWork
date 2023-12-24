using CourseWorkMMO.Items;
using CourseWorkMMO.Selectors;
using CourseWorkMMO.Generators;

namespace CourseWorkMMO.Elements
{
    public class Create : Element
    {
        public int Created { get; protected set; }
        public Create(string name, IGenerator delayGenerator, Selector selector)
            : base(name, delayGenerator, selector) => UpdateNextTime();

        public override void NextStep()
        {
            Item item = new();
            Created++;
            Element? next = Selector.ChooseNextElement(item);
            MovedTo = next != null ? next.Name : "Dispose";
            if (next == null) GeneralDispose.Destroy(item, CurrentTime);
            else next.MoveTo(item);
            UpdateNextTime();
        }

        public override void PrintStatistic()
        {
            Console.Write($"\n{Name}");
            Console.Write($", Created: {Created}");
            Console.Write($", Next time: {NextTime}");
        }

        public override void PrintResults()
        {
            Console.Write($"\n{Name}");
            Console.Write($", Total created: {Created}.");
        }

        public void SetStartingTime(double time) => NextTime = time;
    }

    public class Create<T> : Create where T : Item, new()
    {
        public Create(string name, IGenerator delayGenerator, Selector selector)
            : base(name, delayGenerator, selector) { }

        public override void NextStep()
        {
            T item = new()
            {
                CreatedTime = CurrentTime
            };
            Created++;
            Element? next = Selector.ChooseNextElement(item);
            MovedTo = next != null ? next.Name : "Dispose";
            if (next == null)
            {
                if (PersonalDispose == null) GeneralDispose.Destroy(item, CurrentTime);
                else PersonalDispose.Destroy(item, CurrentTime);
            }
            else next.MoveTo(item);
            UpdateNextTime();
        }
    }
}