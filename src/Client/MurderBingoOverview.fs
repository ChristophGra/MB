module Client.MurderBingoOverview

open System
open Client.Murderbingo
open Elmish
open Fable.Remoting.Client
open Feliz
open Shared


type Model = {
    CurrentBingo: Murderbingo.Model option
    CurrentText: string
    BingoList: ((string * Guid) list) option
    CurrentUserKey: Guid option
}

type Msg =
    | BingoMsg of Murderbingo.Msg
    | AddBingo
    | OpenBingo of Guid
    | TextChanged of string
    | LoadBingoList
    | LoadedBingoList of (string * Guid) list

let initial userkey =
    {CurrentBingo = Option.None; CurrentText = ""; BingoList = option.None; CurrentUserKey = userkey},Cmd.ofMsg Msg.LoadBingoList
let addBingo str =
    Cmd.OfAsync.perform Helpers.murderBingoApi.AddBingo str OpenBingo
let loadBingoList () =
    Cmd.OfAsync.perform Helpers.murderBingoApi.LoadBingoList () LoadedBingoList

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | BingoMsg p ->
        match p with
        | CloseBingo ->
            initial model.CurrentUserKey
        | _ ->
            let adapter = Helpers.createAdapter id BingoMsg
            let newBingo, newMsg = adapter <| Murderbingo.update p (Option.defaultWith (fun () -> Murderbingo.initial model.CurrentUserKey) model.CurrentBingo)
            {model with CurrentBingo = Some newBingo}, newMsg
    | AddBingo  ->
        match model.CurrentUserKey with
        | Some key -> model, addBingo (model.CurrentText, key)
        | None -> model, Cmd.none
    | OpenBingo guid ->
        let m = BingoMsg <| Murderbingo.OpenBingo guid
        model, Cmd.ofMsg m
    | TextChanged s ->
        {model with CurrentText = s}, Cmd.none
    | LoadBingoList ->
        model, loadBingoList ()
    | LoadedBingoList list ->
        {model with BingoList = Some list}, Cmd.none


let view (model: Model) (dispatch: Msg -> unit) =
    Html.div [
        prop.children [
            match model.CurrentBingo with
            | None ->
                Html.textarea [
                    prop.text model.CurrentText
                    prop.onChange (TextChanged >> dispatch)
                ]
                Html.button [
                    prop.text "Add Bingo"
                    prop.onClick ((fun _ -> AddBingo) >> dispatch)
                ]
                match model.BingoList with
                | Some list ->
                    Html.unorderedList [
                        for (name, id) in list do
                            Html.li [
                                prop.text name
                                prop.onClick ((fun _ -> OpenBingo id) >> dispatch)
                            ]
                    ]
                | None ->
                    Html.none
            | Some bingo ->
                Murderbingo.view bingo (BingoMsg >> dispatch)
        ]
    ]
