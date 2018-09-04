using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Reactive.Linq;
using System.Linq;
using VisualSortingAlgorithms.Entities;
using ZedGraph;
using VisualSortingAlgorithms.Boundary;
using System.Collections.Generic;

namespace VisualSortingAlgorithms.Control
{
    public class App
    {
        private List<IGraphControl> _graphControlList;
        private int _stepDelay = DefaultStepDelay;
        private static int XMax = 5;
        public static int YMax = 1000;
        public static int DefaultStepDelay = 500;

        public int[] Data { get; } = new int[XMax];
        public event Action OnStopVisualization;

        public App()
        {
            _graphControlList = new List<IGraphControl>();
            GenerateRandomSeries();
        }
        internal void SetStepDelay(int value)
        {
            _stepDelay = value;
            foreach (var it in _graphControlList)
            {
                it.SortAlgorithm.StepDelay = value;
            }
        }
        internal void StopVisualization()
        {
            foreach (var it in _graphControlList)
            {
                it.Stop();
            }
            OnStopVisualization?.Invoke();
        }
        internal void StartVisualization()
        {
            foreach (var it in _graphControlList)
            {
                it.Start();
            }
        }
        internal IGraphControl FindGraph(string name)
        {
            return _graphControlList.Where(it => it.SortAlgorithm.Name == name).FirstOrDefault();
        }
        internal void AddGraph(IGraphControl graphControl)
        {
            _graphControlList.Add(graphControl);
            graphControl.Data = Data;
        }
        internal void RemoveGraph(IGraphControl graphControl)
        {
            _graphControlList.Remove(graphControl);
        }
        public SortAlgorithm CreateSortAlgorithm(string name)
        {
            var n = name.ToUpperInvariant();
            var result = new SortAlgorithm
            {
                Name = name,
                StepDelay = DefaultStepDelay,
            };
            switch (n)
            {
                case "MERGE":
                    result.SortFunc = SortingAlgorithm.Merge;
                    break;
                case "QUICK":
                    result.SortFunc = SortingAlgorithm.Quick;
                    break;
                case "BUBBLE":
                    result.SortFunc = SortingAlgorithm.Bubble;
                    break;
            }
            return result;
        }

        internal void GenerateRandomSeries()
        {
            Random rand = new Random(DateTime.UtcNow.Millisecond);
            var ymin = YMax / 10;
            for (int i = 0; i < XMax; i++)
            {
                var n = rand.Next(ymin, YMax);
                Data[i] = n;
            }

            foreach (var it in _graphControlList)
            {
                it.Data = Data;
            }
        }
    }
}
