
namespace CourseWorkMMO.Generators
{
    public class NormalGenerator : IGenerator
    {
        private double _avarageDelay;
        public double AvarageDelay
        {
            get { return _avarageDelay; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Avarage delay time must be more than 0");
                _avarageDelay = value;
            }
        }

        private double _deviation;
        public double Deviation
        {
            get { return _deviation; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Deviation must be more than 0");
                _deviation = value;
            }
        }

        private readonly Random _rand = new();

        public NormalGenerator(double avarageDelay, double deviation)
        {
            AvarageDelay = avarageDelay;
            Deviation = deviation;
        }

        //Box-Muller transform
        public double NextDelay()
        {
            double u1 = 1.0 - _rand.NextDouble();
            double u2 = 1.0 - _rand.NextDouble();
            double stdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double normal = AvarageDelay + Deviation * stdNormal;
            return normal <= 0 ? Double.Epsilon : normal;
        }
    }
}
