using System;
using VisualSortingAlgorithms.Entities;
using System.Drawing;

namespace VisualSortingAlgorithms.Control
{
    public class SortAlgorithm
    {
        public string Name { get; internal set; }
        public Func<int[], IObservable<ISortAction>> SortFunc { get; internal set; }
        public Func<IObservable<PointF>> BigOFunc { get; internal set; }
    }
}