using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualSortingAlgorithms.Entities
{
    public interface ISortAction
    {
        int Index1 { get; }
        int Index2 { get; }
        int[] Indices { get; }
        int[] Items { get; }
    }

    class CompareAction: ISortAction
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int[] Indices { get; set; }
        public int[] Items { get; set; }
    }
    class SwapAction : ISortAction
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int[] Indices { get; set; }
        public int[] Items { get; set; }
    }
    class AfterSwapAction : ISortAction
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int[] Indices { get; set; }
        public int[] Items { get; set; }
    }
    class CompleteAction : ISortAction
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int[] Indices { get; set; }
        public int[] Items { get; set; }
    }
    class SetAction : ISortAction
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int[] Indices { get; set; }
        public int[] Items { get; set; }
    }
}
