using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Reactive.Linq;
using System.Linq;
using VisualSortingAlgorithms.Entities;
using ZedGraph;
using VisualSortingAlgorithms.Control;

namespace VisualSortingAlgorithms.Boundary
{
    public interface IGraphControl
    {
        SortAlgorithm SortAlgorithm { get; set; }
        int[] Data { get; set; }

        void Start();
        void Stop();
    }
}
