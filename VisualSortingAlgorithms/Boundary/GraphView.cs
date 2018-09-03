using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Reactive.Linq;
using System.Linq;
using VisualSortingAlgorithms.Entities;
using ZedGraph;

namespace VisualSortingAlgorithms.Boundary
{
    internal class GraphView
    {
        public ZedGraphControl ZedGraphControl { get; }
        public PointPairList Points { get; internal set; }
        public Func<int[], IObservable<ISortAction>> SortFunc { get; internal set; }

        private const int YMax = 1000;

        public GraphView(ZedGraphControl zedGraphControl)
        {
            ZedGraphControl = zedGraphControl;
        }

        public void Start_()
        {
            var z = ZedGraphControl;
            var p = z.GraphPane;

            var ibar = p.CurveList[0];
            var bar = p.CurveList[1];

            ThreadStart threadStart = delegate ()
            {
                for (int i = 0; i < bar.Points.Count; i++)
                {
                    z.Invoke(new MethodInvoker(() =>
                    {
                        var point = bar.Points[i];
                        var ipoint = new PointPair(point.X, point.Y);
                        ibar.AddPoint(ipoint);
                        z.Invalidate();
                    }));
                    //Thread.CurrentThread.Join(500);
                    z.Invoke(new MethodInvoker(() =>
                    {
                        ibar.Clear();
                    }));
                }
            };

            var timer = new System.Windows.Forms.Timer
            {
                Interval = 2000
            };
            timer.Tick += delegate (object sender, EventArgs e)
            {
                new Thread(threadStart).Start();
                timer.Stop();
            };
            timer.Start();
        }

        public static ZedGraphControl CreateZedGraph(PointPairList points)
        {
            var z = new ZedGraphControl
            {
                Dock = DockStyle.Fill
            };
            var p = z.GraphPane;
            var ipoints = new PointPairList();
            var ibar = p.AddBar("Invalidate", ipoints, Color.Red);
            ibar.Bar.Fill.Type = FillType.Solid;
            ibar.Bar.Border.IsVisible = false;

            var bar = p.AddBar("Data", points, Color.LightBlue);
            bar.Bar.Fill.Type = FillType.Solid;
            bar.Bar.Border.IsVisible = false;

            p.Title.IsVisible = false;
            p.Legend.IsVisible = false;
            p.XAxis.Title.IsVisible = false;
            p.XAxis.Type = AxisType.Linear;
            p.XAxis.Scale.Min = 0;
            p.YAxis.Title.IsVisible = false;
            p.YAxis.Type = AxisType.Linear;
            p.YAxis.Scale.Max = YMax;
            p.BarSettings.Type = BarType.Overlay;
            z.MasterPane.Margin.All = 0;
            z.IsEnableVZoom = false;
            z.IsEnableZoom = false;

            z.AxisChange();
            return z;
        }


    }
}
