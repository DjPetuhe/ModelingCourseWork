using CourseWorkMMO.Items;

namespace CourseWorkMMO.Elements
{
    public class Dispose
    {
        public static int AllDisposesDestroyed { get; private set; }

        public int Destroyed
        {
            get { return DestroyedType.Values.Sum(); }
        }

        public SortedList<int, int> DestroyedType { get; private set; } = new();

        public  void Destroy(Item item, double currentTime)
        {
            if (!DestroyedType.ContainsKey(item.InitialType))
                DestroyedType.Add(item.InitialType, 0);
            DestroyedType[item.InitialType]++;
            AllDisposesDestroyed++;
            StatsHelper.AddLifeTime(item, currentTime);
        }

        public void Clear()
        {
            AllDisposesDestroyed = 0;
            DestroyedType.Clear();
        }
    }
}
