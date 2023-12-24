
namespace CourseWorkMMO.Items
{
    public class Ship : Item
    {
        private static readonly Random _rand = new();

        public Ship()
        {
            InitialType = _rand.NextDouble() switch
            {
                <= 0.25 => 1,
                <= 0.80 => 2,
                _ => 3
            };
            Type = InitialType;
        }

        public Ship(int type)
        {
            InitialType = type;
            Type = type;
        }
    }
}
