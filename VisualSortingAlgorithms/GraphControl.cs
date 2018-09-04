using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisualSortingAlgorithms.Boundary;
using VisualSortingAlgorithms.Entities;
using System.Reactive.Linq;
using ZedGraph;
using VisualSortingAlgorithms.Control;

namespace VisualSortingAlgorithms
{
    public partial class GraphControl : ZedGraphControl, IGraphControl
    {
        public SortAlgorithm SortAlgorithm { get; set; }
        public int[] Data
        {
            get
            {
                var bar = GraphPane.CurveList[2];
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
                var bar = GraphPane.CurveList[2];
                bar.Clear();
                for (int i = 0; i < array.Length; i++)
                {
                    bar.AddPoint(new PointPair(i + 1, array[i]));
                }
                AxisChange();
            }
        }
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
            var bar = p.CurveList[2];
            
            var a = Data;
            //var a = GraphView.Points.Select(it => (int)it.Y).ToArray();
            //Timer(TimeSpan.FromSeconds(10), Scheduler.DispatcherScheduler()).
            //SortFunc(a).ObserveOn(DispatcherScheduler.Current).Subscribe(it =>
            var source = SortAlgorithm.SortFunc(a).SelectMany(it =>
            {
                //var s = new Subject<Action>();
                if (it is SetAction)
                {
                    return new List<Action>
                    {
                        () =>
                        {
                            cbar.Clear();
                            for (int i = 0; i < it.Indices.Length; i++)
                            {
                                var point = bar.Points[it.Indices[i]];
                                var ipoint = new PointPair(point.X, point.Y);
                                cbar.AddPoint(ipoint);
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
            var trigger = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(SortAlgorithm.StepDelay));
            var triggeredSource = source.Zip(trigger, (s, _) => s);
            triggeredSource.Finally(() =>
            {
                Program._app.StopVisualization();
            });
            source.Finally(() =>
            {
                Program._app.StopVisualization();
            });
            var h = triggeredSource.Subscribe(it =>
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
            _unsubscribe = h;
        }
        public void Stop()
        {
            _unsubscribe.Dispose();
            _unsubscribe = null;
        }
        public static GraphControl Create()
        {
            ZedGraphControl z = new GraphControl()
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
            //TODO: new PointPairList()
            var bar = p.AddBar("Data", new PointPairList(), Color.LightBlue);
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
            p.YAxis.Scale.Max = App.YMax;
            p.BarSettings.Type = BarType.Overlay;
            z.IsEnableVZoom = false;
            z.IsEnableZoom = false;
            z.Dock = DockStyle.None;

            return (GraphControl)z;
        }
    }
}
