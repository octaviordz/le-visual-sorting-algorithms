using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace VisualSortingAlgorithms.Entities
{
    class SortingAlgorithm
    {
        public static IObservable<ISortAction> BubbleSort(int[] items)
        {
            var sorted = new int[items.Length];
            items.CopyTo(sorted, 0);
            return Observable.Create(
            (IObserver<ISortAction> observer) =>
            {
                for (int i = 0; i < items.Length; i++)
                {
                    for (int idx = 0; idx < items.Length - 1 - i; idx++)
                    {
                        observer.OnNext(new CompareAction
                        {
                            Index1 = idx,
                            Index2 = idx + 1,
                            Items = sorted,
                        });
                        // compare
                        if (sorted[idx] < sorted[idx + 1])
                        {
                            continue;
                        }
                        observer.OnNext(new SwapAction
                        {
                            Index1 = idx,
                            Index2 = idx + 1,
                            Items = sorted,
                        });
                        // swap
                        var tmp = sorted[idx];
                        sorted[idx] = sorted[idx + 1];
                        sorted[idx + 1] = tmp;
                        observer.OnNext(new AfterSwapAction
                        {
                            Index1 = idx,
                            Index2 = idx + 1,
                            Items = sorted,
                        });
                    }
                }
                observer.OnNext(new CompleteAction
                {
                    Index1 = 0,
                    Index2 = 0,
                    Items = sorted,
                });
                observer.OnCompleted();
                return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
            });

        }
    }
}
