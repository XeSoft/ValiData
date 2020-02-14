namespace ValiData

module Ensure =

    open System

    let nonBlank err (s: string) =
        if String.IsNullOrWhiteSpace s then
            Error (List.singleton err)
        else
            Ok s


    let nonEmptyGuid err (g: Guid) =
        if g = Guid.Empty then
            Error (List.singleton err)
        else
            Ok g


    let nonEmptyList err (l: _ list) =
        match l with
        | [] ->
            Error (List.singleton err)
        | _ ->
            Ok l


    let listOf f (l: _ list) =
        let init = [], []
        let update (oks, errors) item =
            match f item with
            | Ok x ->
                oks @ [x], errors
            | Error errs ->
                oks, errors @ errs
        let oks, errors = List.fold update init l
        match errors with
        | [] -> Ok oks
        | _ -> Error errors


    let isNone err x =
        match x with
        | None ->
            Ok ()
        | Some a ->
            Error (List.singleton (err a))


    let maxLength err i (s: string) =
        if s.Length > i then
            Error (List.singleton (err i))
        else
            Ok s


    let toDecimal err (s: string) =
        match Decimal.TryParse s with
        | false, _ ->
            Error (List.singleton err)
        | true, d ->
            Ok d


    let maxDecimalPlaces err i (d : Decimal) =
        let decimalOnly = d - (Math.Truncate d)
        let scaled = decimalOnly * (pown 10m i)
        if Math.Truncate scaled <> scaled then
            Error (List.singleton err)
        else
            Ok d


    let toInt err (s: string) =
        match Int32.TryParse s with
        | false, _ ->
            Error (List.singleton err)
        | true, i ->
            Ok i


    let isInRange err min max x =
        if min <= x && x <= max then
            Ok x
        else
            Error (List.singleton err)


    let isBelow err max x =
        if x <= max then
            Ok x
        else
            Error (List.singleton err)


    let rec oneOf err list x =
        match list with
        | [] ->
            Error (List.singleton err)
        | item :: rest ->
            if item = x then
                Ok x
            else
                oneOf err rest x


    let allOf ferrs x =
        List.fold (fun state f ->
            match state, f x with
            | Ok _, Ok _ ->
                Ok x
            | Ok _, Error errs ->
                Error errs
            | Error errf, Ok _ ->
                Error errf
            | Error errf, Error errs ->
                Error (List.append errf errs)
        ) (Ok x) ferrs


    let noneOr f x =
        match x with
        | None ->
            Ok None
        | Some v ->
            f v
            |> Result.map Some


    let untrue err f x =
        if f x then
            Error (List.singleton err)
        else
            Ok x


    let istrue err f x =
        untrue err (f >> not) x


    open System.Text.RegularExpressions
    let emailRegex = new Regex(@"^[\w&^*%#~{}=+?`-]+(?:\.[\w&^*%#~{}=+?`-]+)*@(?:[a-zA-Z0-9]+\.)+[a-zA-Z]{2,}$", RegexOptions.IgnoreCase)
    let isEmailLike err x =
        if emailRegex.IsMatch(x) then
            Ok x
        else
            Error (List.singleton err)


    let validDate err (x: string) =
        match System.DateTime.TryParse x with
        | false, _ ->
            Error (List.singleton err)
        | true, d ->
            Ok d


    let validDateString err (x: string) =
        match System.DateTime.TryParse x with
        | false, _ ->
            Error (List.singleton err)
        | true, _ ->
            Ok x


    let isTodayOrAfter (today: DateTime) err date =
        if date >= today then
            Ok date
        else
            Error (List.singleton err)


    let expirationDate err (s: string) (currentDate: DateTime) =
        if s.Length < 5 then
            Error (List.singleton err)
        else
            match System.DateTime.TryParse s with
            | false, _ ->
                Error (List.singleton err)
            | true, d ->
                if currentDate > d then
                    Error (List.singleton err)
                else
                    if d > currentDate.AddYears(5) then
                        Error (List.singleton err)
                    else
                        Ok s