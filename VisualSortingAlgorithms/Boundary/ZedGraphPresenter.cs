using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Reactive.Linq;
using System.Linq;
using VisualSortingAlgorithms.Entities;
using ZedGraph;
using System.Reactive.Concurrency;
using System.Reactive.PlatformServices;
using System.Windows.Threading;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Collections.Generic;

namespace VisualSortingAlgorithms.Boundary
{
    internal class ZedGraphPresenter
    {
        public GraphView GraphView { get; }
        public PointPairList Points { get; internal set; }
        public Func<int[], IObservable<ISortAction>> SortFunc { get; internal set; }
        public Control ZedGraphControl { get { return GraphView.ZedGraphControl; } }

        private const int YMax = 1000;
        private const int StepDelay = 200;

        public ZedGraphPresenter(GraphView graphView)
        {
            GraphView = graphView;
        }

        public void Start()
        {
            var z = GraphView.ZedGraphControl;
            var p = z.GraphPane;

            var cbar = p.CurveList[0];
            var sbar = p.CurveList[1];
            var bar = p.CurveList[2];

            var a = GraphView.Points.Select(it => (int)it.Y).ToArray();
            //Timer(TimeSpan.FromSeconds(10), Scheduler.DispatcherScheduler()).
            //SortFunc(a).ObserveOn(DispatcherScheduler.Current).Subscribe(it =>
            var source = SortFunc(a).SelectMany(it =>
            {
                //var s = new Subject<Action>();
                if (it is CompareAction)
                {
                    return new List<Action>
                    {
                        () =>
                        {
                            cbar.Clear();
                            var point1 = bar.Points[it.Index1];
                            var point2 = bar.Points[it.Index2];
                            var ipoint1 = new PointPair(point1.X, point1.Y);
                            cbar.AddPoint(ipoint1);
                            var ipoint2 = new PointPair(point2.X, point2.Y);
                            cbar.AddPoint(ipoint2);
                            z.Invalidate();
                        },
                    };
                }
                else if (it is SwapAction)
                {
                    return new List<Action>
                    {
                        () =>
                        {
                            cbar.Clear();
                            sbar.Clear();
                            var point1 = bar.Points[it.Index1];
                            var point2 = bar.Points[it.Index2];
                            var ipoint1 = new PointPair(point1.X, point1.Y);
                            sbar.AddPoint(ipoint1);
                            var ipoint2 = new PointPair(point2.X, point2.Y);
                            sbar.AddPoint(ipoint2);
                            z.Invalidate();
                        },
                        () =>
                        {
                            sbar.Clear();
                            var point1 = bar.Points[it.Index1];
                            var point2 = bar.Points[it.Index2];
                            var ipoint1 = new PointPair(point1.X, point2.Y);
                            sbar.AddPoint(ipoint1);
                            var ipoint2 = new PointPair(point2.X, point1.Y);
                            sbar.AddPoint(ipoint2);

                            var barPoints = (IPointListEdit)bar.Points;
                            barPoints[it.Index1] = new PointPair(point1.X, point2.Y);
                            barPoints[it.Index2] = new PointPair(point2.X, point1.Y);
                            z.Invalidate();
                        },
                    };
                }
                else if (it is AfterSwapAction)
                {
                    return new List<Action>
                    {
                        () =>
                        {
                            cbar.Clear();
                            sbar.Clear();
                            z.Invalidate();
                        },
                    };
                }
                else if (it is CompleteAction)
                {
                    return new List<Action>
                    {
                        () =>
                        {
                            cbar.Clear();
                            sbar.Clear();
                            var barPoints = (IPointListEdit)bar.Points;
                            for (int i = 0; i < barPoints.Count; i++)
                            {
                                barPoints[i] = new PointPair(barPoints[i].X, it.Items[i]);
                            }
                            z.Invalidate();
                        },
                    };
                }
                return new List<Action>();
            });
            var trigger = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(StepDelay));
            var triggeredSource = source.Zip(trigger, (s, _) => s);
            triggeredSource.Subscribe(it =>
            {
                if (z.InvokeRequired)
                {
                    z.Invoke(it);
                }
                else
                {
                    it.Invoke();
                }
            });
        }

        public static ZedGraphControl CreateZedGraph(PointPairList points)
        {
            var z = new ZedGraphControl
            {
                Dock = DockStyle.Fill
            };
            var p = z.GraphPane;
            var cbar = p.AddBar("compare", new PointPairList(), Color.DarkOrange);
            cbar.Bar.Fill.Type = FillType.Solid;
            cbar.Bar.Border.IsVisible = false;

            var sbar = p.AddBar("swap", new PointPairList(), Color.Red);
            sbar.Bar.Fill.Type = FillType.Solid;
            sbar.Bar.Border.IsVisible = false;

            var bar = p.AddBar("Data", points, Color.LightBlue);
            bar.Bar.Fill.Type = FillType.Solid;
            bar.Bar.Border.IsVisible = false;

            p.Title.IsVisible = false;
            p.Legend.IsVisible = false;
            p.XAxis.Title.IsVisible = false;
            p.XAxis.Type = AxisType.Linear;
            p.XAxis.Scale.Min = 0;
            p.XAxis.MinorTic.Size = 0;
            p.YAxis.Title.IsVisible = false;
            p.YAxis.Type = AxisType.Linear;
            p.YAxis.Scale.Max = YMax;
            p.BarSettings.Type = BarType.Overlay;
            z.IsEnableVZoom = false;
            z.IsEnableZoom = false;

            z.AxisChange();
            return z;
        }


    }
}
