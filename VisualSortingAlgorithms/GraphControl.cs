using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Forms;
using VisualSortingAlgorithms.Boundary;
using VisualSortingAlgorithms.Control;
using VisualSortingAlgorithms.Entities;
using ZedGraph;

namespace VisualSortingAlgorithms
{
    public partial class GraphControl : ZedGraphControl, IGraphControl
    {
        public SortAlgorithm _sortAlgorithm;
        public SortAlgorithm SortAlgorithm
        {
            get
            {
                return _sortAlgorithm;
            }
            set
            {
                _sortAlgorithm = value;
                GraphPane.Title.Text = _sortAlgorithm.Name;
                if (_sortAlgorithm.BigOFunc != null)
                {
                    var graphPane = MasterPane[1];
                    var l = graphPane.CurveList[0];
                    l.Clear();
                    _sortAlgorithm.BigOFunc().ObserveOn(this).Subscribe(p =>
                    {
                        l.AddPoint(new PointPair(p.X, p.Y));
                    },
                    () =>
                    {
                        AxisChange();
                    });
                }
            }
        }
        public int[] Data
        {
            get
            {
                var bar = GraphPane.CurveList[3];
                var array = new int[bar.Points.Count];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = (int)bar.Points[i].Y;
                }
                return array;
            }
            set
            {
                var array = value;
                var bar = MasterPane[0].CurveList[3];
                bar.Clear();
                for (int i = 0; i < array.Length; i++)
                {
                    bar.AddPoint(new PointPair(i + 1, array[i]));
                }
                AxisChange();
                Invalidate();
            }
        }
        private Subject<bool> _visualizing = new Subject<bool>();
        public IObservable<bool> Visualizing => _visualizing;
        public IObservable<Unit> VisualizationTick { get; set; }
        private IDisposable _unsubscribe = null;

        public GraphControl()
        {
            InitializeComponent();
        }
        
        public void Start()
        {
            if (_unsubscribe != null)
            {
                return;
            }
            var z = this as ZedGraphControl;
            var p = z.GraphPane;

            var cbar = p.CurveList[0];
            var sbar = p.CurveList[1];
            var setbar = p.CurveList[2];
            var bar = p.CurveList[3];

            var a = Data;
            var source = SortAlgorithm.SortFunc(a).SelectMany(it =>
            {
                if (it is SetAction)
                {
                    return new List<Action>
                    {
                        () =>
                        {
                            setbar.Clear();
                            cbar.Clear();
                            sbar.Clear();
                            for (int i = 0; i < it.Indices.Length; i++)
                            {
                                var point = bar.Points[it.Indices[i]];
                                var ipoint = new PointPair(point.X, point.Y);
                                setbar.AddPoint(ipoint);
                            }
                            z.Invalidate();
                        },
                    };
                }
                else if (it is CompareAction)
                {
                    return new List<Action>
                    {
                        () =>
                        {
                            setbar.Clear();
                            cbar.Clear();
                            sbar.Clear();
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
                            setbar.Clear();
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
                            setbar.Clear();
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
                else if (it is SwapShiftAction)
                {
                    return new List<Action>
                    {
                        () =>
                        {
                            setbar.Clear();
                            cbar.Clear();
                            sbar.Clear();
                            var point1 = bar.Points[it.Index1];
                            var point2 = bar.Points[it.Index2];
                            var ipoint1 = new PointPair(point1.X, point1.Y);
                            //shift point
                            cbar.AddPoint(ipoint1);
                            for (int i = it.Index1; i <= it.Index2; i++)
                            {
                                setbar.AddPoint(new PointPair(bar.Points[i].X, bar.Points[i].Y));
                            }
                            var ipoint2 = new PointPair(point2.X, point2.Y);
                            sbar.AddPoint(ipoint2);
                            z.Invalidate();
                        },
                        () =>
                        {
                            setbar.Clear();
                            cbar.Clear();
                            sbar.Clear();
                            var point1 = bar.Points[it.Index1];
                            var y = bar.Points[it.Index2].Y;
                            //shift point
                            sbar.AddPoint(new PointPair(point1.X, y));
                            var barPoints = (IPointListEdit)bar.Points;
                            for (int i = it.Index2; i > it.Index1; i--)
                            {
                                barPoints[i] = new PointPair(barPoints[i].X, barPoints[i - 1].Y);
                            }
                            barPoints[it.Index1].Y = y;
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
                            setbar.Clear();
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
            //var trigger = _stepTrigger.Select(t => Observable.Interval(TimeSpan.FromMilliseconds(t))).
            //    Switch();
            var trigger = VisualizationTick;
            //var trigger = Observable.Timer(
            //    TimeSpan.Zero,
            //    TimeSpan.FromMilliseconds(StepDelay));
            var triggeredSource = source.Zip(trigger, (s, _) => s);
            _unsubscribe = triggeredSource.ObserveOn(this).Subscribe(
                it => it.Invoke(),
                ex =>
                {
                    Debug.WriteLine($"Error {ex}");
                    Stop();
                },
                () =>
                {
                    Debug.WriteLine($"Complete");
                    Stop();
                });
            _visualizing.OnNext(true);
        }
        public void Stop()
        {
            using (_unsubscribe) { }
            _unsubscribe = null;
            _visualizing.OnNext(false);
        }
        public static GraphControl Create()
        {
            GraphControl z = new GraphControl()
            {
                Dock = DockStyle.Fill
            };
            var p = z.GraphPane;
            var po = new GraphPane();
            var c = po.AddCurve(null, new PointPairList(), Color.Black);
            c.Symbol.Type = SymbolType.None;
            po.XAxis.Type = AxisType.Linear;
            po.XAxis.Scale.IsVisible = false;
            po.XAxis.Title.IsVisible = false;
            po.XAxis.Scale.MaxAuto = false;
            po.XAxis.Scale.Max = SortingAlgorithm.BigOXMax;
            po.YAxis.Type = AxisType.Linear;
            po.YAxis.Scale.IsVisible = false;
            po.YAxis.Title.IsVisible = false;
            po.YAxis.Scale.MaxAuto = false;
            po.YAxis.Scale.Max = SortingAlgorithm.BigOYMax;
            z.MasterPane.Add(po);

            var cbar = p.AddBar("compare", new PointPairList(), Color.DarkOrange);
            cbar.Bar.Fill.Type = FillType.Solid;
            cbar.Bar.Border.IsVisible = false;

            var sbar = p.AddBar("swap", new PointPairList(), Color.Red);
            sbar.Bar.Fill.Type = FillType.Solid;
            sbar.Bar.Border.IsVisible = false;

            var setbar = p.AddBar("working-set", new PointPairList(), Color.Blue);
            setbar.Bar.Fill.Type = FillType.Solid;
            setbar.Bar.Border.IsVisible = false;

            var bar = p.AddBar("Data", new PointPairList(), Color.LightBlue);
            bar.Bar.Fill.Type = FillType.Solid;
            bar.Bar.Border.IsVisible = false;

            p.Title.IsVisible = true;
            p.Legend.IsVisible = false;
            p.XAxis.Title.IsVisible = false;
            p.XAxis.Type = AxisType.Linear;
            p.XAxis.Scale.Min = 0;
            p.XAxis.MinorTic.Size = 0;
            p.YAxis.Title.IsVisible = false;
            p.YAxis.Type = AxisType.Linear;
            p.YAxis.Scale.Max = App.YMax;
            p.BarSettings.Type = BarType.Overlay;
            z.IsEnableVZoom = false;
            z.IsEnableZoom = false;
            z.Dock = DockStyle.None;

            // Layout the GraphPanes using a default Pane Layout
            using (Graphics g = z.CreateGraphics())
            {
                z.MasterPane.SetLayout(g, false, new int[] { 1, 1 }, new float[] { 2f, 1f});
            }

            return z;
        }
    }
}
