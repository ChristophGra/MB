module Client.Murderbingo

open System
open System.Collections.Generic
open Elmish
open Fable.Import.RemoteDev
open Feliz

type Model = {
    Id: Guid
    SeedName: string
    NameEntries: string list
    CurrentName: string
    CurrentUserKey: Guid option
}



type Msg =
    | OpenBingo of Guid
    | OpenedBingo of Guid * string * string list
    | CloseBingo
    | ChangeName of string
    | AddName
    | AddedName
    | LoadNames
    | LoadedNames of string list

let initial userkey =
    {SeedName = ""; NameEntries = List.empty<string>; CurrentName = ""; Id = Guid.Empty; CurrentUserKey = userkey}


let AddName name =
    Cmd.OfAsync.perform Helpers.murderBingoApi.AddName name (fun _ -> AddedName)
let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Msg.OpenBingo guid ->
        let c = Cmd.OfAsync.perform Helpers.murderBingoApi.LoadBingo guid Msg.OpenedBingo
        model,c
    | Msg.OpenedBingo(id, bingoName, nameEntries) ->
        {initial with SeedName = bingoName; NameEntries = nameEntries; Id = id}, Cmd.none
    | Msg.CloseBingo ->
        model, Cmd.none
    | ChangeName name ->
        {model with CurrentName = name}, Cmd.none
    | AddName ->
        match model.CurrentUserKey with
        | Some key -> model, AddName (model.CurrentName,model.Id, key)
        | None -> model, Cmd.none
    | AddedName ->
        model, Cmd.ofMsg LoadNames
    | LoadNames ->
        model, Cmd.OfAsync.perform Helpers.murderBingoApi.LoadNames model.Id LoadedNames
    | LoadedNames nameList->
        {model with NameEntries = nameList}, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) =
    Html.div [
        prop.children [
            Html.text model.SeedName
            Html.button [
                prop.text "Close Bingo"
                prop.onClick ((fun _ -> CloseBingo) >> dispatch)
            ]
            Html.textarea [
                prop.text model.CurrentName
                prop.onChange (ChangeName >> dispatch)
            ]
            Html.button [
                prop.text "Add Name"
                prop.onClick ((fun _ -> Msg.AddName) >> dispatch)
            ]
            Html.unorderedList [
                for name in model.NameEntries do
                    Html.li [
                        prop.text name
                    ]
            ]
        ]
    ]
