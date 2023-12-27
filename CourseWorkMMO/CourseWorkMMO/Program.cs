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
            PortModel(5, false, false);
        }

        public static void PortModel(int amountOfTests, bool printSteps = true, bool printingResults = true)
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
            ComplexProcess england = new("England", englandGenerator, englandSelector, 0, 5);
            Process storm = new("Storm", stormGenerator, int.MaxValue);

            Dispose stormDispose = new();

            List<Element> elements = new() { createShip, createStorm, tugboat, pier, england, storm };

            Model mod = new(elements)
            {
                PrintingSteps = printSteps,
                PrintingResults = printingResults
            };

            pier.AddGeneratorForType(1, pierType1Generator);
            pier.AddGeneratorForType(2, pierType2Generator);
            pier.AddGeneratorForType(3, pierType3Generator);
            pier.AddGeneratorForType(4, pierType4Generator);

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

            pier.Addition = (Item item, double _) => item.Type = item.Type == 4 ? 6 : 5;
            england.Addition = (Item item, double currentTime) =>
            {
                item.Type = 4;
                item.CreatedTime = currentTime;
            };
            tugboat.Addition = (Item item, double currentTime) =>
            {
                if (item.Type == 6) StatsHelper.AddLifeTime(item, currentTime);
            };

            StatsHelper.TransitionPeriod = 60000;
            for (int i = 0; i < amountOfTests; i++)
            {
                StatsHelper.NextTest(i);
                elements.ForEach(el => el.Clear());

                for (int j = 0; j < 4; j++)
                    tugboatQueue.Enqueue(new Ship(4) { CreatedTime = 0 });

                tugboat.SetStartingWorkingOn(new Ship(4), tugboatGenerator.NextDelay());

                mod.Simulate(500000);
            }
            StatsHelper.PrintAvarageStatsAfterTranPeriod();
        }
    }
}