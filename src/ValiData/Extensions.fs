namespace ValiData

module String =

    let trimmed (s : string) =
        s.Trim()


    let lower (s: string) =
        s.ToLower()


module Option =

    open System


    let blankAsNone (sOpt : string option) =
        match sOpt with
        | Some s when String.IsNullOrWhiteSpace s ->
            None
        | x ->
            x


    let trimmed (sOpt : string option) =
        Option.map String.trimmed sOpt


    let zeroAsNone (iOpt : int option) =
        match iOpt with
        | Some 0 ->
            None
        | x ->
            x
