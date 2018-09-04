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
        public static IObservable<ISortAction> Bubble(int[] items)
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
                return Disposable.Create(() => Console.WriteLine($"{nameof(Bubble)}. Observer has unsubscribed"));
            });

        }
        public static IObservable<ISortAction> Merge(int[] items)
        {
            var sorted = new int[items.Length];
            items.CopyTo(sorted, 0);
            return Observable.Create(
            (IObserver<ISortAction> observer) =>
            {
                MergeSort(sorted, 0, sorted.Length - 1, observer);
                observer.OnNext(new CompleteAction
                {
                    Index1 = 0,
                    Index2 = 0,
                    Items = sorted,
                });
                observer.OnCompleted();
                return Disposable.Create(() => Console.WriteLine($"{nameof(Merge)}. Observer has unsubscribed"));
            });
        }
        private static void MergeSort(int[] input, int low, int high, IObserver<ISortAction> observer)
        {
            if (low < high)
            {
                int middle = (low / 2) + (high / 2);
                var indices = new int[(middle - low) + 1];
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i] = low + i;
                }
                observer.OnNext(new SetAction
                {
                    Indices = indices,
                    Items = input,
                });
                MergeSort(input, low, middle, observer);

                indices = new int[(high - (middle + 1)) + 1];
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i] = middle + 1 + i;
                }
                observer.OnNext(new SetAction
                {
                    Indices = indices,
                    Items = input,
                });
                MergeSort(input, middle + 1, high, observer);
                Merge(input, low, middle, high, observer);
            }
        }
        private static void Merge(int[] input, int low, int middle, int high, IObserver<ISortAction> observer)
        {
            List<int> result = new List<int>();

            int left = low;
            int right = middle + 1;
            int[] tmp = new int[(high - low) + 1];
            int tmpIndex = 0;
            while ((left <= middle) && (right <= high))
            {
                //observer.OnNext(new CompareAction
                //{
                //    Index1 = left,
                //    Index2 = right,
                //    Items = tmp,
                //});
                // compare
                if (input[left] < input[right])
                {
                    tmp[tmpIndex] = input[left];
                    left = left + 1;
                }
                else
                {
                    observer.OnNext(new SwapAction
                    {
                        Index1 = left,
                        Index2 = right,
                        Items = tmp,
                    });
                    tmp[tmpIndex] = input[right];
                    right = right + 1;
                }
                tmpIndex = tmpIndex + 1;
            }

            while (left <= middle)
            {
                tmp[tmpIndex] = input[left];
                left = left + 1;
                tmpIndex = tmpIndex + 1;
            }

            while (right <= high)
            {
                tmp[tmpIndex] = input[right];
                right = right + 1;
                tmpIndex = tmpIndex + 1;
            }
            
            for (int i = 0; i < tmp.Length; i++)
            {
                input[low + i] = tmp[i];
            }
            observer.OnNext(new AfterSwapAction
            {
                Index1 = 0,
                Index2 = 0,
                Items = input,
            });
        }
        public static IObservable<ISortAction> Quick(int[] items)
        {
            throw new NotImplementedException();
        }
    }
}
