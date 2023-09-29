module Index

open System
open Elmish
open Fable.React.Props
open Fable.Remoting.Client
open Feliz
open Feliz.style
open Microsoft.FSharp.Collections
open Shared
open Elmish.Navigation
open Elmish.UrlParser
type Page =
    | Home
    | MurderBingo of Guid
type Model = { CurrentPath: Page }
type Route =
    | Home
    | MurderBingo of Guid

let parseGuid (str:string) =
    match Guid.TryParse str with
    | true, g -> Ok g
    | _ -> Error ()
let route: Parser<Route>=
    oneOf
        [ map MurderBingo (s "bingo" </> (custom "" parseGuid ))
          map Home top ]
type Msg =
    | DoNothing
    | Navigate of string

let urlUpdate (result:Route) model: Model * Cmd<Msg> =
    match result, model.CurrentPath with
    | Home, Page.Home -> model, Cmd.none
    | MurderBingo newId, Page.MurderBingo oldId -> model, Cmd.none
    | _ -> model, Cmd.none

let init () : Model * Cmd<Msg> =
    let model = { CurrentPath = Page.Home}

    let cmd = Cmd.ofMsg DoNothing

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    |DoNothing -> model, Cmd.none
    | Navigate path -> model, Cmd.none
open Feliz
open Feliz.Bulma
open Feliz.Styles
let debugBorder =
    style.border (2, borderStyle.solid,  "red")
let NavBar (dispatch: Msg -> unit) =
    Html.div [
        prop.className "navbar"
        prop.style [
            display.block
            flexDirection.row
            style.width (length.percent 100)
        ]
        prop.children [
            Html.text "navbar"
        ]
    ]
let view (model: Model) (dispatch: Msg -> unit) =
    Html.div [
        prop.style [
            display.flex
            flexDirection.column
        ]
        prop.children [
            NavBar dispatch
            Html.div [
                prop.text "abc"
            ]
        ]

    ]