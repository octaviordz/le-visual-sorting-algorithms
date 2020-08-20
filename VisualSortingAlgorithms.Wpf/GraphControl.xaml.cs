using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisualSortingAlgorithms.Control;
using System.Reactive.Linq;
using System.Diagnostics;
using VisualSortingAlgorithms.Entities;
using InteractiveDataDisplay.WPF;

namespace VisualSortingAlgorithms.Wpf
{
    /// <summary>
    /// Interaction logic for GraphControl.xaml
    /// </summary>
    public partial class GraphControl : UserControl, Boundary.IGraphControl
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
            }
        }
        public int[] Data
        {
            get
            {
                var s0 = mainGraph.Sources[0];
                var s1 = mainGraph.Sources[1];

                return (int[])s1.Data;
            }
            set
            {
                int[] array = value;
                int[] x = new int[array.Length];
                int[] y = array;
                for (int i = 0; i < array.Length; i++)
                {
                    x[i] = i;
                    y[i] = array[i];
                }
                mainGraph.Plot(x, y);
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
            int[] a = Data;
            var source = SortAlgorithm.SortFunc(a).SelectMany(it =>
            {
                int[] data = (int[])mainGraph.Sources[1].Data;
                if (it is SetAction)
                {
                    //return new List<Action>
                    //{
                    //    () =>
                    //    {
                    //        setGraph.Plot(new int[]{ }, new int[]{ });
                    //        comperGraph.Plot(new int[]{ }, new int[]{ });
                    //        swapGraph.Plot(new int[]{ }, new int[]{ });
                    //        int[] x = new int[it.Indices.Length];
                    //        int[] y = new int[it.Indices.Length];
                    //        for (int i = 0; i < it.Indices.Length; i++)
                    //        {
                    //            x[i] = it.Indices[i];
                    //            y[i] = data[it.Indices[i]];
                    //        }
                    //        setGraph.Plot(x, y);
                    //    },
                    //};
                }
                else if (it is CompareAction)
                {
                    return new List<Action>
                    {
                        () =>
                        {
                            setGraph.Plot(new int[]{ }, new int[]{ });
                            comperGraph.Plot(new int[]{ }, new int[]{ });
                            swapGraph.Plot(new int[]{ }, new int[]{ });
                            int y1 = data[it.Index1];
                            int y2= data[it.Index2];
                            int[] x = { it.Index1, it.Index2};
                            int[] y = { y1, y2 };
                            comperGraph.Plot(x, y);
                        },
                    };
                }
                else if (it is SwapAction)
                {
                    return new List<Action>
                    {
                        () =>
                        {
                            setGraph.Plot(new int[]{ }, new int[]{ });
                            comperGraph.Plot(new int[]{ }, new int[]{ });
                            swapGraph.Plot(new int[]{ }, new int[]{ });
                            int y1 = data[it.Index1];
                            int y2= data[it.Index2];
                            int[] x = { it.Index1, it.Index2};
                            int[] y = { y1, y2 };
                            swapGraph.Plot(x, y);
                        },
                        () =>
                        {
                            //setGraph.Plot(new int[]{ }, new int[]{ });
                            //// comperGraph.Plot(new int[]{ }, new int[]{ }); //
                            //swapGraph.Plot(new int[]{ }, new int[]{ });
                            int y1 = data[it.Index1];
                            int y2= data[it.Index2];
                            
                            var _data = (int[])swapGraph.Sources[1].Data;
                            _data[0] = y2;
                            _data[1] = y1;
                            swapGraph.Sources[1].Update();

                            data[it.Index1] = y2;
                            data[it.Index2] = y1;
                            mainGraph.Sources[1].Update();
                        },
                    };
                }
                //else if (it is SwapShiftAction)
                //{
                //    return new List<Action>
                //    {
                //        () =>
                //        {
                //            setGraph.Plot(new int[]{ }, new int[]{ });
                //            //comperGraph.Plot(new int[]{ }, new int[]{ });
                //            //swapGraph.Plot(new int[]{ }, new int[]{ });

                //            //var point1 = bar.Points[it.Index1];
                //            //var point2 = bar.Points[it.Index2];
                //            //var ipoint1 = new PointPair(point1.X, point1.Y);
                //            //shift point
                //            comperGraph.Plot(new int[]{ it.Index1 }, new int[]{ data[it.Index1]});
                //            //cbar.AddPoint(ipoint1);
                //            //for (int i = it.Index1; i <= it.Index2; i++)
                //            //{
                //            //    setbar.AddPoint(new PointPair(bar.Points[i].X, bar.Points[i].Y));
                //            //}
                //            int length = it.Index2 - it.Index1 + 1;
                //            int[] x = new int[length];
                //            int[] y = new int[length];
                //            for (int i = it.Index1, j = 0; i <= it.Index2; i++, j++)
                //            {
                //                x[j] = i;
                //                y[j] = data[i];
                //            }
                //            setGraph.Plot(x, y);

                //            //var ipoint2 = new PointPair(point2.X, point2.Y);
                //            //sbar.AddPoint(ipoint2);
                //            swapGraph.Plot(new int[]{ it.Index2 }, new int[]{ data[it.Index2]});
                //        },
                //        () =>
                //        {
                //            //setGraph.Plot(new int[]{ }, new int[]{ });
                //            comperGraph.Plot(new int[]{ }, new int[]{ });
                //            swapGraph.Plot(new int[]{ }, new int[]{ });
                //            //var point1 = bar.Points[it.Index1];
                //            //var y = bar.Points[it.Index2].Y;
                //            int y = data[it.Index2];
                //            //shift point
                //            //sbar.AddPoint(new PointPair(point1.X, y));
                //            swapGraph.Plot(new int[]{ it.Index1 }, new int[]{ y });
                //            //var barPoints = (IPointListEdit)bar.Points;
                //            for (int i = it.Index2; i > it.Index1; i--)
                //            {
                //                //barPoints[i] = new PointPair(barPoints[i].X, barPoints[i - 1].Y);
                //                data[i] = data[i -1];
                //            }
                //            //barPoints[it.Index1].Y = y;
                //            data[it.Index1] = y;
                //            mainGraph.Sources[1].Update();
                //        },
                //    };
                //}
                else if (it is CompleteAction)
                {
                    return new List<Action>
                    {
                        () =>
                        {
                            setGraph.Plot(new int[]{ }, new int[]{ });
                            comperGraph.Plot(new int[]{ }, new int[]{ });
                            swapGraph.Plot(new int[]{ }, new int[]{ });
                            int[] x = new int[it.Items.Length];
                            int[] y = new int[it.Items.Length];
                            for (int i = 0; i < it.Items.Length; i++)
                            {
                                x[i] = i;
                                y[i] = it.Items[i];
                            }
                            mainGraph.Plot(x, y);
                        },
                    };
                }
                return new List<Action>();
            });
            var trigger = VisualizationTick;
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
    }
}
