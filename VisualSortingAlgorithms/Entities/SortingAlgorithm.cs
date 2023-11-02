using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace VisualSortingAlgorithms.Entities
{
    class SortingAlgorithm
    {
        public const int BigOXMax = 500;
        public const int BigOYMax = 10000;
        public static IObservable<ISortAction> Bubble(int[] items)
        {
            var sorted = new int[items.Length];
            items.CopyTo(sorted, 0);
            return Observable.Create((IObserver<ISortAction> observer) =>
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
                    }
                }
                observer.OnNext(new CompleteAction
                {
                    Index1 = 0,
                    Index2 = 0,
                    Items = sorted,
                });
                observer.OnCompleted();
                return Disposable.Create(() => Debug.WriteLine($"{nameof(Bubble)}. Observer has unsubscribed"));
            });

        }
        internal static IObservable<PointF> BubbleBigO()
        {
            // BigO(n^2)
            return Observable.Create((IObserver<PointF> observer) =>
            {
                for (int x = 0; x < BigOXMax; x++)
                {
                    float y = (x * x);
                    observer.OnNext(new PointF(x, y));
                }
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }
        public static IObservable<ISortAction> Merge(int[] items)
        {
            var sorted = new int[items.Length];
            items.CopyTo(sorted, 0);
            return Observable.Create((IObserver<ISortAction> observer) =>
            {
                MergeSort(sorted, 0, sorted.Length - 1, observer);
                observer.OnNext(new CompleteAction
                {
                    Index1 = 0,
                    Index2 = 0,
                    Items = sorted,
                });
                observer.OnCompleted();
                return Disposable.Create(() => Debug.WriteLine($"{nameof(Merge)}. Observer has unsubscribed"));
            });
        }
        private static void MergeSort(int[] items, int low, int high, IObserver<ISortAction> observer)
        {
            if (low < high)
            {
                int middle = (low / 2) + (high / 2);
                var indices = new int[(high - low) + 1];
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i] = low + i;
                }
                observer.OnNext(new SetAction
                {
                    Indices = indices,
                    Items = items,
                });
                MergeSort(items, low, middle, observer);
                MergeSort(items, middle + 1, high, observer);
                indices = new int[(high - low) + 1];
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i] = low + i;
                }
                observer.OnNext(new SetAction
                {
                    Indices = indices,
                    Items = items,
                });
                Merge(items, low, middle, high, observer);
            }
        }
        private static void Merge(int[] items, int low, int middle, int high, IObserver<ISortAction> observer)
        {
            int left = low;
            int right = middle + 1;
            int[] tmp = new int[(high - low) + 1];
            int tmpIndex = 0;
            int shiftOnLeftCount = 0;
            while ((left <= middle) && (right <= high))
            {
                observer.OnNext(new CompareAction
                {
                    Index1 = left,
                    Index2 = right,
                    Items = tmp,
                });
                if (items[left] < items[right])
                {
                    tmp[tmpIndex] = items[left];
                    left = left + 1;
                }
                else
                {
                    observer.OnNext(new SwapShiftAction
                    {
                        Index1 = left + shiftOnLeftCount,
                        Index2 = right,
                        Items = tmp,
                    });
                    shiftOnLeftCount += 1;
                    tmp[tmpIndex] = items[right];
                    right = right + 1;
                }
                tmpIndex = tmpIndex + 1;
            }

            while (left <= middle)
            {
                tmp[tmpIndex] = items[left];
                left = left + 1;
                tmpIndex = tmpIndex + 1;
            }

            while (right <= high)
            {
                tmp[tmpIndex] = items[right];
                right = right + 1;
                tmpIndex = tmpIndex + 1;
            }
            
            for (int i = 0; i < tmp.Length; i++)
            {
                items[low + i] = tmp[i];
            }
        }
        internal static IObservable<PointF> MergeBigO()
        {
            // BigO(n log n)
            return Observable.Create((IObserver<PointF> observer) =>
            {
                for (int x = 0; x < BigOXMax; x++)
                {
                    float y = (float)(x * Math.Log(x));
                    observer.OnNext(new PointF(x, y));
                }
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }
        public static IObservable<ISortAction> Quick(int[] items)
        {
            var sorted = new int[items.Length];
            items.CopyTo(sorted, 0);
            return Observable.Create((IObserver<ISortAction> observer) =>
            {
                QuickSort(sorted, 0, sorted.Length - 1, observer);
                observer.OnNext(new CompleteAction
                {
                    Index1 = 0,
                    Index2 = 0,
                    Items = sorted,
                });
                observer.OnCompleted();
                return Disposable.Create(() => Debug.WriteLine($"{nameof(Quick)}. Observer has unsubscribed"));
            });
        }
        private static void QuickSort(int[] items, int low, int high, IObserver<ISortAction> observer)
        {
            int p;
            if (low < high)
            {
                p = Partition(items, low, high, observer);
                QuickSort(items, low, p, observer);
                QuickSort(items, p + 1, high, observer);
            }
        }
        private static int Partition(int[] items, int low, int high, IObserver<ISortAction> observer)
        {
            int pivot = items[low];
            int pivotIndx = low;
            int i = low - 1;
            int j = high + 1;
            int temp;

            var indices = new int[(high - low) + 1];
            for (int idx = 0; idx < indices.Length; idx++)
            {
                indices[idx] = low + idx;
            }
            observer.OnNext(new SetAction
            {
                Indices = indices,
                Items = items,
            });
            while (true)
            {
                do
                {
                    i = i + 1;
                    observer.OnNext(new CompareAction
                    {
                        Index1 = i,
                        Index2 = pivotIndx,
                        Items = items,
                    });
                } while (items[i] < pivot);
                do
                {
                    j = j - 1;
                    observer.OnNext(new CompareAction
                    {
                        Index1 = j,
                        Index2 = pivotIndx,
                        Items = items,
                    });
                } while (items[j] > pivot);

                if (i >= j)
                {
                    return j;
                }

                observer.OnNext(new SwapAction
                {
                    Index1 = j,
                    Index2 = i,
                    Items = items,
                });
                temp = items[i];
                items[i] = items[j];
                items[j] = temp;
                pivotIndx = j;
            }
        }
    }
}
