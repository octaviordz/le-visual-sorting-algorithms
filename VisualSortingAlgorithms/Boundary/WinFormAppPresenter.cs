using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisualSortingAlgorithms.Entities;
using ZedGraph;

namespace VisualSortingAlgorithms.Boundary
{
    class WinFormAppPresenter
    {
        private ZedGraphPresenter _zedGraphPresenter;
        private PointPairList _points;

        public WinFormAppPresenter(MainForm form)
        {
            System.Threading.Thread.CurrentThread.Name = "UI thread";
            //var source = Observable.Range(1, 5).SelectMany(it =>
            //{
            //    return new List<string>
            //    {
            //        $"#{it}",
            //        $"#{it + 10}"
            //    };
            //});
            //var trigger = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            //var triggeredSource = source.Zip(trigger, (s, _) => s);
            //triggeredSource.Take(10).Timestamp().Subscribe(timer =>
            //{
            //    Console.WriteLine($"Timer:{timer.Timestamp} {timer.Value}");
            //});

            for (int i = 0; i < form.checkedListBox1.Items.Count; i++)
            {
                form.checkedListBox1.SetItemChecked(i, true);
                break;
            }
            _points = new PointPairList();
            Random rand = new Random(DateTime.UtcNow.Millisecond);
            for (int i = 0; i < 20; i++)
            {
                var n = rand.Next(100, 1000);
                _points.Add(i + 1, n);
            }
            
            _zedGraphPresenter = BubbleGraphControlPresenter(_points);
            form.WindowState = FormWindowState.Maximized;
            form.splitContainer.Panel2.Controls.Add(_zedGraphPresenter.ZedGraphControl);
        }

        internal void Start()
        {
            _zedGraphPresenter.Start();
        }

        private static ZedGraphPresenter BubbleGraphControlPresenter(PointPairList points)
        {
            var z = ZedGraphPresenter.CreateZedGraph(points);
            var v = new GraphView(z)
            {
                Points = points
            };
            var presenter = new ZedGraphPresenter(v)
            {
                SortFunc = SortingAlgorithm.BubbleSort,
            };
            return presenter;
        }
    }
}
