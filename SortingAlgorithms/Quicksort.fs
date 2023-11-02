module Quicksort

open System
open System.Collections
open Microsoft.FSharp.Core

type QuicksortResult =
    { Items: int array
      SortActions: SortAction list }


// Divides array into two partitions
let partition (items: int array) (lo: int) (hi: int) (sortActions: Generic.List<SortAction>) =
    let middle = (float (hi + lo) / 2.0) |> Math.Floor |> Convert.ToInt32
    let pivot = items[middle]
    let mutable leftIndex = lo - 1
    let mutable rightIndex = hi + 1

    let leftIndexStepRight () = leftIndex <- leftIndex + 1
    let rightIndexStepLeft () = rightIndex <- rightIndex - 1
    let isLeftLessThanPivot () =
        let compareAction = SortAction.Compare [ leftIndex; middle ]
        sortActions.Add(compareAction)
        items[leftIndex] < pivot
    let isRightGreaterThanPivot ()=
        let compareAction = SortAction.Compare [ rightIndex; middle ]
        sortActions.Add(compareAction)
        items[rightIndex] > pivot

    let rec _partition () =
        leftIndexStepRight ()
        while isLeftLessThanPivot() do 
            leftIndexStepRight()

        rightIndexStepLeft ()
        while isRightGreaterThanPivot() do 
            rightIndexStepLeft()

        if leftIndex >= rightIndex then
            leftIndex, rightIndex // stop
        else
            let swapAction = SortAction.Swap [ leftIndex; rightIndex; ]
            sortActions.Add(swapAction)
            // swap
            let tmp = items[leftIndex]
            items[leftIndex] <- items[rightIndex]
            items[rightIndex] <- tmp
            _partition ()
    
    _partition ()


let rec quickSort (items: int array) lo hi (sortActions: Generic.List<SortAction>)=

    if lo < hi then
        let _, right = partition items lo hi sortActions
        quickSort items lo right sortActions
        quickSort items (right + 1) hi sortActions


let sort (items: int array) =
    let sortActions = Generic.List<SortAction>()
    quickSort items 0 (items.Length - 1) sortActions
    
    { Items = items
      SortActions = [] }

