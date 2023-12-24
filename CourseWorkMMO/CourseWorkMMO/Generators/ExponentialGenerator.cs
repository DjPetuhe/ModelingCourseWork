
namespace CourseWorkMMO.Generators
{
    public class ExponentialGenerator : IGenerator
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

        private readonly Random _rand = new();

        public ExponentialGenerator(double avarageDelay) => AvarageDelay = avarageDelay;

        public double NextDelay()
        {
            double value = _rand.NextDouble();
            value = value != 0 ? value : Double.Epsilon;
            return -AvarageDelay * Math.Log(value);
        }
    }
}
