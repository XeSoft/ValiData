# ValiData

A small extensible validation library for F#.

Compatible with Fable. You can define a validation once and use it pretty much anywhere, including the front-end.

## Usage

Here is a basic example.

```fsharp
open System
open ValiData

type User = {
    UserId: Guid
    Email: string
    IsActive: bool
}
​
module User =
​
    type Error =
        | UserIdEmpty
        | EmailBlank
        | EmailLooksInvalid
​
    let create userId email isActive : User =
        {
            UserId = userId
            Email = email
            IsActive = isActive
        }
​
    let validate (x: User) =
        Ok create
        |> verify x.UserId (Ensure.nonEmptyGuid UserIdEmpty)
        |> verify x.Email (
            String.trimmed
            >> String.lower
            >> Ensure.nonBlank EmailBlank
            >=> Ensure.isEmailLike EmailLooksInvalid
        )
        // cannot be created as invalid data (won't deserialize)
        |> verify x.IsActive Ok

        
User.validate {
    UserId = Guid.Empty
    Email = "do not contact me"
    IsActive = true
}
// = Error [ UserIdEmpty; EmailLooksInvalid ]
```

### Validating lists

Here is an example that validates a list of data.

```fsharp
type AttachFiles = {
    EntityId: Guid
    FileIds: Guid list
}

module AttachFiles =

    type Error =
        | EntityIdEmpty
        | FileIdEmpty

    let create entityId fileIds : AttachFiles =
        {
            EntityId = entityId
            FileIds = fileIds
        }

    let validate (x : AttachFiles) =
        Ok create
        |> verify x.EntityId (Ensure.nonEmptyGuid EntityIdEmpty)
        |> verify x.FileIds (Ensure.listOf (Ensure.nonEmptyGuid FileIdEmpty))
        
        
AttachFiles.validate {
    EntityId = Guid.Empty
    FileIds = [ Guid.Empty; Guid.Empty ]
}
// = Error [ EntityIdEmpty; FileIdEmpty; FileIdEmpty ]
```

### Assert

You can assert that something is true without using the data.

```fsharp
Ok ...
|> assert_ someData (Ensure.nonBlank SomeDataBlank)
|> verify ...
```

Here, the `assert_` line will give an error if the data fails to validate, but even if the validation passes, `someData` will not be passed into the create function.

> `assert` is already a keyword in F#, which is why I had to use `assert_`.

## Extending

Simply define your own Ensure module in your project. Any functions you put there will be added to ValiData's Ensure functions. Make sure that the result of your Ensure function's error case returns a list of Errors so it can be easily combined with other errors.

```fsharp
module Ensure =

    let minLength err i (s: string) =
        if s.Length < i then
            Error [err i]
        else
            Ok s
            
open ValiData

Ok ...
|> verify ... (
    Ensure.minLength TooShort 5 // your function
    >=> Ensure.maxLength TooLong 20 // built-in to ValiData
)
```

> This also works for built-in modules such as String. This is how ValiData provides `String.trimmed` for example.

## Installing

Currently, I recommend that you clone the repo and simply copy the project into your own solution. The code is small and entirely contained in 3 files. In this way you can also just add your own Ensure functions and other module extensions directly into your copy of ValiData.

## TODO

* Tests
* Nuget package
* More validations

## FAQ

Yes, the name is cheesy.