
namespace CourseWorkMMO.Generators
{
    public class UniformGenerator : IGenerator
    {
        private double _minimum = Double.Epsilon;
        public double Minimum
        {
            get { return _minimum; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("The minimal delay must be more than 0");
                if (value >= Maximum)
                    throw new ArgumentException("The minimal delay must be less than maximal");
                _minimum = value;
                ChangeDiff();
            }
        }

        private double _maximum = Double.MaxValue;
        public double Maximum
        {
            get { return _maximum; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("The maximal delay must be more than 0");
                if (value <= Minimum)
                    throw new ArgumentException("The maximal delay must be more than minimal");
                _maximum = value;
                ChangeDiff();
            }
        }

        private readonly Random _rand = new();
        private double _diff;

        public UniformGenerator() => ChangeDiff();

        public UniformGenerator(double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        private void ChangeDiff() => _diff = Maximum - Minimum;

        public double NextDelay() => _rand.NextDouble() * _diff + Minimum;
    }
}
