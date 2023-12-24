
namespace CourseWorkMMO.Generators
{
    public class ErlangGenerator : IGenerator
    {
        private readonly static Random s_rand = new();
        private readonly int _k;
        private readonly double _averageDelay;

        public ErlangGenerator(int k, double averageDelay)
        {
            _k = k;
            _averageDelay = averageDelay;
        }

        public double NextDelay()
        {
            double erlangMulti = 1;
            for (int i = 0; i < _k; i++)
                erlangMulti *= 1 - s_rand.NextDouble();
            return -Math.Log(erlangMulti) / (_k * _averageDelay);
        }
    }
}
