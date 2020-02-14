namespace ValiData

[<AutoOpen>]
module Verify =

    let verify (x: 'T) (r: 'T -> Result<'U, List<'TError>>) (fr: Result<'U -> 'V, List<'TError>>) =
        match fr, r x with
            | Ok fOk, Ok ok ->
                Ok (fOk ok)

            | Ok _, Error err ->
                Error err

            | Error errf, Ok _ ->
                Error errf

            | Error errf, Error err ->
                // ehh, not very efficient
                Error (List.append errf err)


    let fail err =
        Error (List.singleton err)


    let assert_ (x: 'x) (r: 'x -> Result<_, List<'TError>>) (fr: Result<'T -> 'U, List<'TError>>) =
        match fr, r x with
            | Ok _, Ok _ ->
                fr

            | Ok _, Error err ->
                Error err

            | Error errf, Ok _ ->
                Error errf

            | Error errf, Error err ->
                Error (List.append errf err)


    let (>=>) f g =
        f >> Result.bind g
