using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace VisualSortingAlgorithms.Boundary
{
    class ZedGraphPresenter
    {
        public static ZedGraphControl CreateZedGraph()
        {
            var z = new ZedGraphControl
            {
                Dock = System.Windows.Forms.DockStyle.Fill
            };
            var p = z.GraphPane;
            var points = new PointPairList();
            Random rand = new Random(DateTime.UtcNow.Millisecond);
            for (int i = 0; i < 100; i++)
            {
                var n = rand.Next(100, 1000);
                points.Add(i, n);
            }
            var ipoints = new PointPairList();
            var ibar = p.AddBar("Invalidate", ipoints, Color.Red);
            ibar.Bar.Fill.Type = FillType.Solid;
            ibar.Bar.Border.IsVisible = false;

            var bar = p.AddBar("Data", points, Color.LightBlue);
            bar.Bar.Fill.Type = FillType.Solid;
            bar.Bar.Border.IsVisible = false;
            void threadStart()
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
                    Thread.CurrentThread.Join(500);
                    z.Invoke(new MethodInvoker(() =>
                    {
                        ibar.Clear();
                    }));
                }
            }

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

            p.Title.IsVisible = false;
            p.Legend.IsVisible = false;
            p.XAxis.Title.IsVisible = false;
            p.XAxis.Type = AxisType.Linear;
            p.YAxis.Title.IsVisible = false;
            p.YAxis.Type = AxisType.Linear;
            
            p.BarSettings.Type = BarType.Overlay;
            
            z.IsEnableVZoom = false;
            z.IsEnableZoom = false;
            
            z.AxisChange();
            return z;
        }

        
    }
}
