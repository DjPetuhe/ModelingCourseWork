using CourseWorkMMO.Items;

namespace CourseWorkMMO.Elements
{
    public class Dispose
    {
        public static int Destroyed 
        { 
            get { return DestroyedType.Values.Sum(); }
        }

        public static SortedList<int, int> DestroyedType { get; private set; } = new();


        public static double TotalLifeTime
        {
            get { return TotalLifeTimesType.Values.Sum(); }
        }

        public static double AvarageLifeTime
        {
            get { return TotalLifeTime / Destroyed; }
        }

        public static SortedList<int, double> TotalLifeTimesType { get; private set; } = new();

        public static void Destroy(Item item, double currentTime)
        {
            if (!TotalLifeTimesType.ContainsKey(item.InitialType))
            {
                TotalLifeTimesType.Add(item.InitialType, 0);
                DestroyedType.Add(item.InitialType, 0);
            }
            DestroyedType[item.InitialType]++;
            TotalLifeTimesType[item.InitialType] += currentTime - item.CreatedTime;
        }

        public static void Clear()
        {
            DestroyedType.Clear();
            TotalLifeTimesType.Clear();
        }

        public static double AvarageLifeTimeType(int type)
        {
            if (!DestroyedType.ContainsKey(type))
                return 0;
            return TotalLifeTimesType[type] / DestroyedType[type];
        }
    }
}
