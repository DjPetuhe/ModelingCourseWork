
namespace CourseWorkMMO.Generators
{
    public class ConstantGenerator : IGenerator
    {
        private double _delayTime;
        public double DelayTime
        {
            get { return _delayTime; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("The delay must be more than 0");
                _delayTime = value;
            }
        }

        public ConstantGenerator(double delayTime) => DelayTime = delayTime;

        public double NextDelay() => DelayTime;
    }
}
