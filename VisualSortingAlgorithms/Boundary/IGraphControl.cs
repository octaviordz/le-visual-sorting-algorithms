﻿using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Reactive.Linq;
using System.Linq;
using VisualSortingAlgorithms.Entities;
using ZedGraph;
using VisualSortingAlgorithms.Control;
using System.Reactive.Subjects;
using System.Reactive;

namespace VisualSortingAlgorithms.Boundary
{
    public interface IGraphControl
    {
        SortAlgorithm SortAlgorithm { get; set; }
        IObservable<Unit> VisualizationTick { get; set; }
        IObservable<bool> Visualizing { get; }
        int[] Data { get; set; }

        void Start();
        void Stop();
    }
}
