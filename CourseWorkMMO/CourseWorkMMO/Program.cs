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
            CarBankModel();
            //HospitalModel();
        }

        public static void CarBankModel()
        {
            IGenerator generatorCreate = new ExponentialGenerator(0.5);
            IGenerator generatorCashier = new ExponentialGenerator(0.3);
            IGenerator generatorStarting = new NormalGenerator(1, 0.3);

            Queue cashier1Qeueu = new(3);
            cashier1Qeueu.Enqueue(new());
            cashier1Qeueu.Enqueue(new());
            Queue cashier2Qeueu = new(3);
            cashier2Qeueu.Enqueue(new());
            cashier2Qeueu.Enqueue(new());

            Process cashier1 = new("Cashier1", generatorCashier, cashier1Qeueu);
            Process cashier2 = new("Cashier2", generatorCashier, cashier2Qeueu);

            cashier1.SetStartingWorkingOn(new(), generatorStarting.NextDelay());
            cashier2.SetStartingWorkingOn(new(), generatorStarting.NextDelay());

            QueuePrioritySelector createSelector = new();
            createSelector.AddNextProcess(cashier1);
            createSelector.AddNextProcess(cashier2);
            Create<Item> cr = new("Create", generatorCreate, createSelector);

            cr.SetStartingTime(0.1);

            static bool changeQueue(List<Element> elements)
            {
                bool swapped = false;
                List<Process> fullQueue = elements.OfType<Process>().Where(p => p.Queue.QueueSize >= 2).ToList();
                List<Process> emptyQueue = elements.OfType<Process>().Where(p => p.Queue.QueueSize <= 1).ToList();
                foreach (var full in fullQueue)
                {
                    foreach (var empty in emptyQueue)
                    {
                        if (full.Queue.QueueSize - empty.Queue.QueueSize >= 2)
                        {
                            swapped = true;
                            Console.Write($"\n\nCar moved from {full.Name} queue to {empty.Name} queue\n");
                            if (empty.Queue.QueueSize == 1)
                                emptyQueue.Remove(empty);
                            Item car = full.Queue.Dequeue();
                            empty.MoveTo(car);
                            break;
                        }
                    }
                }
                return swapped;
            }

            Model mod = new(new List<Element>() { cr, cashier1, cashier2 })
            {
                Addition = changeQueue
            };
            mod.Simulate(1000);
        }

        public static void HospitalModel()
        {
            IGenerator generatorCreate = new ExponentialGenerator(15);
            IGenerator generatorPathToHospitalRooms = new UniformGenerator(3, 8);
            IGenerator generatorPathReceptionToLab = new UniformGenerator(2, 5);
            IGenerator generatorLabRegistry = new ErlangGenerator(3, 4.5);
            IGenerator generatorLabAnalyse = new ErlangGenerator(2, 4);
            IGenerator generatorReception = new ExponentialGenerator(15); //own choice

            WeightSelector selectorCreate = new();
            TypeSelector selectorReception = new();
            WeightSelector selectorPathReceptionToLab = new();
            WeightSelector selectorLabRegistry = new();
            TypeSelector selectorLabAnalyse = new();
            WeightSelector selectorPathLabToReception = new();

            PriorityQueue queueReception = new(100);

            Create<Ship> create = new("Create", generatorCreate, selectorCreate);
            ComplexProcess Reception = new("Reception", generatorReception, selectorReception, queueReception, 2);
            ComplexProcess PathToHospitalRooms = new("PathHospitalRooms", generatorPathToHospitalRooms, 100, 3);
            ComplexProcess PathReceptionToLab = new("PathReceptionToLab", generatorPathReceptionToLab, selectorPathReceptionToLab, 100, 100);
            Process LabRegistry = new("LabRegistry", generatorLabRegistry, selectorLabRegistry, 100);
            ComplexProcess LabAnalyse = new("LabAnalyse", generatorLabAnalyse, selectorLabAnalyse, 100, 2);
            ComplexProcess PathLabToReception = new("PathLabToReception", generatorPathReceptionToLab, selectorPathLabToReception, 100, 100);

            selectorCreate.AddNextElement(Reception, 1);

            selectorReception.AddElementForType(1, PathToHospitalRooms);
            selectorReception.AddElementForType(2, PathReceptionToLab);
            selectorReception.AddElementForType(3, PathReceptionToLab);

            selectorPathReceptionToLab.AddNextElement(LabRegistry, 1);

            selectorLabRegistry.AddNextElement(LabAnalyse, 1);

            selectorLabAnalyse.AddElementForType(2, PathLabToReception);
            selectorLabAnalyse.AddElementForType(3, null);

            selectorPathLabToReception.AddNextElement(Reception, 1);

            queueReception.AddPriority(1, (Item item) => item.Type == 1);

            PathLabToReception.Addition = (Item item) => item.Type = item.Type == 2 ? 1 : item.Type;

            Model mod = new(new List<Element>() { create, Reception, PathToHospitalRooms, PathReceptionToLab, LabRegistry, LabAnalyse, PathLabToReception });
            mod.Simulate(1000);
            mod.PrintHospitalResults();
        }
    }
}