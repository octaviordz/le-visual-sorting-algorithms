using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualSortingAlgorithms.Entities
{
    interface ISortAction
    {
        int Index1 { get; }
        int Index2 { get; }
        int[] Items { get; }
    }

    class SortActionList : List<ISortAction>
    {
    }

    class CompareAction: ISortAction
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int[] Items { get; set; }
    }
    class SwapAction : ISortAction
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int[] Items { get; set; }
    }
    class AfterSwapAction : ISortAction
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int[] Items { get; set; }
    }
    class CompleteAction : ISortAction
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int[] Items { get; set; }
    }
    //    class BubbleSort
    //    {
    //        public (int[], SortActionList) Sort(ReadOnlySpan<int> items)
    //        {
    //            var sorted = new int[items.Length];
    //            var actionList = new SortActionList();
    //            for (int i = 0; i < items.Length; i++)
    //            {
    //                for (int idx = i; idx < items.Length; idx++)
    //                {
    //                    // compare
    //                    actionList.Add(new CompareAction
    //                    {
    //                        Index1 = idx,
    //                        Index2 = idx + 1,
    //                    });
    //                    if (items[idx] > items[idx + 1])
    //                    {
    //                        sorted[idx] = items[idx];
    //                        continue;
    //                    }
    //                    actionList.Add(new MustSwapAction
    //                    {
    //                        Index1 = idx,
    //                        Index2 = idx + 1,
    //                    });
    //                    // swap
    //                    actionList.Add(new SwapAction
    //                    {
    //                        Index1 = idx,
    //                        Index2 = idx + 1,
    //                    });
    //                    sorted[idx] = items[idx + 1];
    //                    sorted[idx + 1] = items[idx];
    //                }
    //            }

    //            return (sorted, actionList);
    //        }
    //    }
}
