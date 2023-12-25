using CourseWorkMMO.Items;

namespace CourseWorkMMO.Elements
{
    public class Dispose
    {
        public int Destroyed
        {
            get { return DestroyedType.Values.Sum(); }
        }

        public SortedList<int, int> DestroyedType { get; private set; } = new();

        public double TotalLifeTime
        {
            get { return TotalLifeTimesType.Values.Sum(); }
        }

        public double AvarageLifeTime
        {
            get { return TotalLifeTime / Destroyed; }
        }

        public SortedList<int, double> TotalLifeTimesType { get; private set; } = new();

        public  void Destroy(Item item, double currentTime)
        {
            if (!TotalLifeTimesType.ContainsKey(item.InitialType))
            {
                TotalLifeTimesType.Add(item.InitialType, 0);
                DestroyedType.Add(item.InitialType, 0);
            }
            DestroyedType[item.InitialType]++;
            TotalLifeTimesType[item.InitialType] += currentTime - item.CreatedTime;
        }

        public void Clear()
        {
            DestroyedType.Clear();
            TotalLifeTimesType.Clear();
        }

        public double AvarageLifeTimeType(int type)
        {
            if (!DestroyedType.ContainsKey(type))
                return 0;
            return TotalLifeTimesType[type] / DestroyedType[type];
        }
    }
}
