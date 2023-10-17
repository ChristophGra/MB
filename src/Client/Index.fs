module Index

open System
open Client
open Elmish
open Feliz
open Feliz.style
open Fulma

type Page =
    | Home
    | MurderBingoOverview
type PageModel =
    | Home
    | MurderBingoOverview of MurderBingoOverview.Model


type Model =
    {
        CurrentPage: PageModel
        CurrentUserKey: Guid option
    }

type Msg =
    | MurderBingoOverviewMsg of MurderBingoOverview.Msg
    | SwitchPage of Page


let init () : Model * Cmd<Msg> =
    let model = { CurrentPage = PageModel.Home; CurrentUserKey = option.None}

    let cmd = Cmd.none

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    let isSamePage newpage oldpage =
        match newpage, oldpage with
        | Page.Home, Home -> true
        | Page.MurderBingoOverview, MurderBingoOverview _ -> true
        | _ -> false

    let pageManagement newPage currentPage =
        if isSamePage newPage currentPage then
            model, Cmd.none
        else
            match newPage with
            | Page.Home -> {model with CurrentPage = PageModel.Home}, Cmd.none
            | Page.MurderBingoOverview ->
                let adapter = Helpers.createAdapter MurderBingoOverview MurderBingoOverviewMsg
                let newSubModel, newMsg = adapter (MurderBingoOverview.initial model.CurrentUserKey)
                {model with CurrentPage = newSubModel}, newMsg

    let currentPage = model.CurrentPage

    match msg with
    | MurderBingoOverviewMsg mbmsg->
        match currentPage with
        | MurderBingoOverview page ->
            let adapter = Helpers.createAdapter MurderBingoOverview MurderBingoOverviewMsg
            let newSubModel, newMsg = adapter <| MurderBingoOverview.update mbmsg page
            {model with CurrentPage = newSubModel}, newMsg
        | _ ->
            model, Cmd.none
    | SwitchPage newPage ->
        pageManagement newPage currentPage



open Feliz
open Feliz.Bulma
open Feliz.Styles
let debugBorder =
    style.border (2, borderStyle.solid,  "red")
let NavBar (dispatch: Msg -> unit) =
    Html.div [
        prop.className "navbar"
        prop.style [
            display.flex
            flexDirection.row

            style.gap 10
            style.width (length.percent 100)
        ]
        prop.children [
            Html.div [
                prop.onClick (fun _ -> dispatch (Msg.SwitchPage Page.Home))
                prop.children [
                    Html.text "Home"
                ]
            ]
            Html.div [
                prop.onClick (fun _ -> dispatch (Msg.SwitchPage Page.MurderBingoOverview))
                prop.children [
                    Html.text "Murder Bingo"
                ]
            ]
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
            match model.CurrentPage with
            | Home ->
                Html.div [
                    prop.text "abc"
                ]
            | MurderBingoOverview mbm ->
                MurderBingoOverview.view mbm (MurderBingoOverviewMsg >> dispatch)

        ]

    ]