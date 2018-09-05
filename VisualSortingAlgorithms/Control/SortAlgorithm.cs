using System;
using VisualSortingAlgorithms.Boundary;
using VisualSortingAlgorithms.Entities;
using ZedGraph;

namespace VisualSortingAlgorithms.Control
{
    public class SortAlgorithm
    {
        public string Name { get; internal set; }
        public Func<int[], IObservable<ISortAction>> SortFunc { get; internal set; }
    }
}