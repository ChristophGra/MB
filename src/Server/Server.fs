module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared


let stubApi =
    {
        stub = fun () -> async { () }
    }

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue stubApi
    |> Remoting.buildHttpHandler

let app =
    application {
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
        url "0.0.0.0:5000"
    }

[<EntryPoint>]
let main _ =
    run app
    0