[<AutoOpen>]
module Common

[<RequireQualifiedAccess>]
type SortAction =
    | Compare of indices: int list
    | Swap of indices: int list


