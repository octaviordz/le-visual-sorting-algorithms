module QuicksortTest

open Xunit
open Xunit.Abstractions
open Quicksort

type public QuicksortTest(outputHelper: ITestOutputHelper) =

    [<Fact>]
    member _.``Correct order (ascending)``() =
        let input = [| 5; 3 |]
        outputHelper.WriteLine(sprintf "input = %A" input)
        let result = Quicksort.sort (input)
        let output = result.Items
        Assert.Equal(3, output[0])
        Assert.Equal(5, output[1])
        outputHelper.WriteLine(sprintf "output = %A" output)

    [<Fact>]
    member _.``Validate actions taken (basic)``() =
        let input = [| 5; 3 |]
        let result = Quicksort.sort(input)
        let actions = result.SortActions

        let expectedActions =
            [ (SortAction.Compare [])
              (SortAction.Swap []) ]

        let areSameKind xi xj =
            outputHelper.WriteLine(sprintf "%A %A" (xi.GetType()) (xj.GetType()))
            xi.GetType() = xj.GetType()

        List.zip expectedActions actions
        |> List.iteri (fun i tuple ->
            let xi, xj = tuple
            outputHelper.WriteLine(sprintf "[%d; %A; %A]" i xi xj)
            Assert.True((areSameKind xi xj)))

        outputHelper.WriteLine(sprintf "sortActions = %A" actions)
