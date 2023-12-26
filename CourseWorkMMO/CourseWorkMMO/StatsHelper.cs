﻿using CourseWorkMMO.Items;
using CourseWorkMMO.Elements;

namespace CourseWorkMMO
{
    public class StatsHelper
    {
        public static int AdditionalEventHappened { get; set; }
        public static double AvarageItemsInModelSum { get; private set; }
        public static int StartingItems { get; private set; }
        
        public static double TotalLifeTime
        {
            get { return TotalLifeTimesType.Values.Sum(); }
        }

        public static SortedList<int, double> TotalLifeTimesType { get; private set; } = new();

        public static SortedList<int, int> ExitedPierType { get; private set; } = new();
        
        private static List<List<double>> _xStatistics123 = new();
        private static List<List<double>> _yStatistics123 = new();
        private static List<List<double>> _xStatisticsAll = new();
        private static List<List<double>> _yStatisticsAll = new();
        private static int _testNum;


        public static void AddLifeTime(Item item, double currentTime)
        {
            if (!TotalLifeTimesType.ContainsKey(item.InitialType))
            {
                TotalLifeTimesType.Add(item.InitialType, 0);
                ExitedPierType.Add(item.InitialType, 0);
            }
            TotalLifeTimesType[item.InitialType] += currentTime - item.CreatedTime;
            ExitedPierType[item.InitialType]++;
        }

        public static void EvaluateStartingItems(List<Element> elements)
        {
            foreach (var el in elements.OfType<Process>())
            {
                StartingItems += el.Queue.QueueSize;
                StartingItems += el.WorkingProcesses;
            }
        }

        public static void EvaluateStepStatistics(List<Element> elements, double dif, double currTime)
        {
            int itemsInModel = elements.OfType<Create>().Sum(cr => cr.Created) + StartingItems - Dispose.AllDisposesDestroyed;
            AvarageItemsInModelSum += itemsInModel * dif;
            _xStatistics123[_testNum].Add(currTime);
            _yStatistics123[_testNum].Add(AvarageLifeTimeType(new List<int>() { 1, 2, 3}));
            _xStatisticsAll[_testNum].Add(currTime);
            _yStatisticsAll[_testNum].Add(AvarageLifeTimeType(new List<int>() { 1, 2, 3, 4 }));
        }

        public static double AvarageLifeTimeType(int type)
        {
            if (!ExitedPierType.ContainsKey(type))
                return 0;
            return TotalLifeTimesType[type] / ExitedPierType[type];
        }

        public static double AvarageLifeTimeType(List<int> types)
        {
            double lifeTimesSum = 0;
            int exitedPierSum = 0;
            foreach (int type in types)
            {
                if (!ExitedPierType.ContainsKey(type))
                    return 0;
                lifeTimesSum += TotalLifeTimesType[type];
                exitedPierSum += ExitedPierType[type];
            }
            return lifeTimesSum / exitedPierSum;
        }

        public static void Clear()
        {
            StartingItems = 0;
            AdditionalEventHappened = 0;
            AvarageItemsInModelSum = 0;
            TotalLifeTimesType.Clear();
            ExitedPierType.Clear();
        }

        public static void NextTest(int index)
        {
            _testNum = index;
            _xStatistics123.Add(new());
            _yStatistics123.Add(new());
            _xStatisticsAll.Add(new());
            _yStatisticsAll.Add(new());
        }

        public static void BuildPlots()
        {
            var plt123 = new ScottPlot.Plot(1400, 800);
            var pltAll = new ScottPlot.Plot(1400, 800);

            for (int i = 0; i < _xStatistics123.Count; i++)
            {
                plt123.AddScatter(_xStatistics123[i].ToArray(), _yStatistics123[i].ToArray());
                pltAll.AddScatter(_xStatisticsAll[i].ToArray(), _yStatisticsAll[i].ToArray());
            }

            plt123.Title("Avarage lifetime of ships type 1, 2 and 3");
            plt123.XLabel("modeling time");
            plt123.YLabel("lifetime");

            pltAll.Title("Avarage lifetime of ships of all time");
            pltAll.XLabel("modeling time");
            pltAll.YLabel("lifetime");

            plt123.SaveFig("ships123Scatter.png");
            pltAll.SaveFig("shipsAllScatter.png");
        }
    }
}