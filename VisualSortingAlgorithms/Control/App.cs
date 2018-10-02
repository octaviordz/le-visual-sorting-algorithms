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
        private Dictionary<string, IDisposable> _disposableDic;
        private static readonly int XMax = 30;
        public static readonly int YMax = 1000;
        public static readonly int DefaultStepDelay = 300;

        public int[] Data { get; } = new int[XMax];
        private int n = 0;
        private int wn = 0;
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
            _disposableDic = new Dictionary<string, IDisposable>();
            _onStopVisualization = new Subject<Unit>();
            GenerateRandomSeries();
        }
        internal void StopVisualization()
        {
            foreach (var it in _graphControlList)
            {
                it.Stop();
            }
            _onStopVisualization.OnNext(Unit.Default);
        }
        internal void StartVisualization()
        {
            wn = _graphControlList.Count;
            n = 0;
            for (int i = 0; i < _graphControlList.Count; i++)
            {
                var it = _graphControlList[i];
                it.Start();
            }
        }
        internal IGraphControl FindGraph(string name)
        {
            return _graphControlList.Where(it => it.SortAlgorithm.Name == name).FirstOrDefault();
        }
        internal void AddGraph(IGraphControl graphControl)
        {
            graphControl.Data = Data;
            IDisposable d = graphControl.Visualizing.Subscribe(
            v =>
            {
                if (v == false)
                {
                    n += 1;
                }
                if (v == false && n >= wn)
                {
                    _onStopVisualization.OnNext(Unit.Default);
                }
            });
            _graphControlList.Add(graphControl);
            _disposableDic.Add(graphControl.SortAlgorithm.Name, d);
        }
        internal void RemoveGraph(IGraphControl graphControl)
        {
            _graphControlList.Remove(graphControl);
            var d = _disposableDic[graphControl.SortAlgorithm.Name];
            using (d) {}
            _disposableDic.Remove(graphControl.SortAlgorithm.Name);
        }
        public SortAlgorithm CreateSortAlgorithm(string name)
        {
            var n = name.ToUpperInvariant();
            var result = new SortAlgorithm
            {
                Name = name,
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
