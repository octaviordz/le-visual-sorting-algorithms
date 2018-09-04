﻿using System;
using VisualSortingAlgorithms.Boundary;
using VisualSortingAlgorithms.Entities;
using ZedGraph;

namespace VisualSortingAlgorithms.Control
{
    public class SortAlgorithm
    {
        public int StepDelay { get; internal set; }
        public string Name { get; internal set; }
        public Func<int[], IObservable<ISortAction>> SortFunc { get; internal set; }
    }
}