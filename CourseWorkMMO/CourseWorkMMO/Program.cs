using CourseWorkMMO.Items;
using CourseWorkMMO.Queues;
using CourseWorkMMO.Elements;
using CourseWorkMMO.Selectors;
using CourseWorkMMO.Generators;

namespace CourseWorkMMO
{
    public class Programm
    {
        public static void Main(string[] args)
        {
            PortModel();
        }

        public static void PortModel()
        {
            IGenerator createShipGenerator = new UniformGenerator(4, 18);
            IGenerator createStormGenerator = new ExponentialGenerator(48);
            IGenerator tugboatGenerator = new ConstantGenerator(1);
            IGenerator pierDefaultGenerator = new UniformGenerator(16, 20);
            IGenerator pierType1Generator = new UniformGenerator(16, 20);
            IGenerator pierType2Generator = new UniformGenerator(21, 27);
            IGenerator pierType3Generator = new UniformGenerator(32, 40);
            IGenerator pierType4Generator = new UniformGenerator(18, 24);
            IGenerator englandGenerator = new UniformGenerator(216, 264);
            IGenerator stormGenerator = new UniformGenerator(2, 6);

            WeightSelector createShipSelector = new();
            WeightSelector createStormSelector = new();
            TypeSelector tugBoatSelector = new();
            WeightSelector pierSelector = new();
            WeightSelector englandSelector = new();

            PriorityQueue tugboatQueue = new(int.MaxValue);

            Create<Ship> createShip = new("ShipCreator", createShipGenerator, createShipSelector);
            Create<Item> createStorm = new("StormCreator", createStormGenerator, createStormSelector);

            Process tugboat = new("Tugboat", tugboatGenerator, tugBoatSelector, tugboatQueue);
            ComplexProcess pier = new("Pier", pierDefaultGenerator, pierSelector, 0, 3);
            ComplexProcess england = new("England", englandGenerator, englandSelector, int.MaxValue, 5);
            Process storm = new("Storm", stormGenerator, int.MaxValue);

            Dispose stormDispose = new();

            createShipSelector.AddNextElement(tugboat, 1);
            createStormSelector.AddNextElement(storm, 1);
            tugBoatSelector.AddElementForType(1, pier);
            tugBoatSelector.AddElementForType(2, pier);
            tugBoatSelector.AddElementForType(3, pier);
            tugBoatSelector.AddElementForType(4, pier);
            tugBoatSelector.AddElementForType(5, null);
            tugBoatSelector.AddElementForType(6, england);
            pierSelector.AddNextElement(tugboat, 1);
            englandSelector.AddNextElement(tugboat, 1);

            tugboatQueue.AddPriority(1, (Item item) => item.Type == 5 || item.Type == 6);

            tugboat.BlockingOnFinishWhenWork.Add(pier);
            pier.BlockingOnStartWhenWork.Add(tugboat);
            storm.BlockingOnStartWhenWork.Add(tugboat);
            storm.BlockingOnFinishWhenWork.Add(tugboat);

            storm.PersonalDispose = stormDispose;

            pier.Addition = (Item item) => item.Type = item.Type == 4 ? 6 : 5;

            for (int i = 0; i < 4; i++)
                tugboatQueue.Enqueue(new Ship(4));

            tugboat.SetStartingWorkingOn(new Ship(4), tugboatGenerator.NextDelay());


            Model mod = new(new List<Element>() { createShip, createStorm, tugboat, pier, england, storm });
            mod.Simulate(1000);
        }
    }
}