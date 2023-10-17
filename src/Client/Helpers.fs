module Client.Helpers

open Elmish
open Fable.Remoting.Client
open Shared


let createAdapter modelAdapter msgAdapter (model,cmd) =
    modelAdapter model, Cmd.map msgAdapter cmd

let murderBingoApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IMurderBingoApi>