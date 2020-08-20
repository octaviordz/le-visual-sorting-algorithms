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
using System.Reactive;
using System.Reactive.Subjects;

namespace VisualSortingAlgorithms.Control
{
    public class App
    {
        private List<IGraphControl> _graphControlList;
        private static int XMax = 30;
        public static int YMax = 1000;
        public static int DefaultStepDelay = 300;

        public int[] Data { get; } = new int[XMax];
        private Subject<Unit> _onStopVisualization;
        public IObservable<Unit> OnStopVisualization
        {
            get
            {
                return _onStopVisualization;
            }
        }

        public App()
        {
            _graphControlList = new List<IGraphControl>();
            _onStopVisualization = new Subject<Unit>();
            GenerateRandomSeries();
        }
        public void StopVisualization()
        {
            foreach (var it in _graphControlList)
            {
                it.Stop();
            }
            _onStopVisualization.OnNext(Unit.Default);
        }
        public void StartVisualization()
        {
            var wn = _graphControlList.Count;
            int n = 0;
            for (int i = 0; i < _graphControlList.Count; i++)
            {
                var it = _graphControlList[i];
                it.Start();
                it.Visualizing.Subscribe(
                    v =>
                    {
                        if (v == false)
                        {
                            n += 1;
                        }
                        if (n >= wn)
                        {
                            _onStopVisualization.OnNext(Unit.Default);
                        }
                    });
            }
        }
        public IGraphControl FindGraph(string name)
        {
            return _graphControlList.Where(it => it.SortAlgorithm.Name == name.ToUpperInvariant()).FirstOrDefault();
        }
        public void AddGraph(IGraphControl graphControl)
        {
            _graphControlList.Add(graphControl);
            graphControl.Data = Data;
        }
        public void RemoveGraph(IGraphControl graphControl)
        {
            _graphControlList.Remove(graphControl);
        }
        public SortAlgorithm CreateSortAlgorithm(string name)
        {
            var n = name.ToUpperInvariant();
            var result = new SortAlgorithm
            {
                Name = n,
            };
            switch (n)
            {
                case "MERGE":
                    result.SortFunc = SortingAlgorithm.Merge;
                    result.BigOFunc = SortingAlgorithm.MergeBigO;
                    break;
                case "QUICK":
                    result.SortFunc = SortingAlgorithm.Quick;
                    result.BigOFunc = SortingAlgorithm.MergeBigO;
                    break;
                case "BUBBLE":
                    result.SortFunc = SortingAlgorithm.Bubble;
                    result.BigOFunc = SortingAlgorithm.BubbleBigO;
                    break;
                default:
                    throw new NotImplementedException($"Sorting algorithm '{n}' not implemented");
            }
            return result;
        }

        internal void GenerateRandomSeries()
        {
            //Random rand = new Random(DateTime.UtcNow.Millisecond);
            Random rand = new Random(1);
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
