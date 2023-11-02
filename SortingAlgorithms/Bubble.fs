module Bubble

open System.Collections

type BubbleResult =
    { Items: int array
      SortActions: SortAction list }

let sort (items: int array) =
    let mutable n = items.Length
    let sortActions = Generic.List<SortAction>()

    while n > 1 do
        let mutable newn = 0

        for i in 1 .. (n - 1) do
            let compareAction = SortAction.Compare [ (i - 1); i ]
            sortActions.Add(compareAction)

            if items[i - 1] > items[i] then
                let swapAction = SortAction.Swap [ (i - 1); i ]
                sortActions.Add(swapAction)
                let tmp = items[i - 1]
                items[i - 1] <- items[i]
                items[i] <- tmp
                newn <- i

        n <- newn

    { Items = items
      SortActions = List.ofSeq sortActions }
