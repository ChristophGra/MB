module Server.Server

open System
open System.Collections.Generic
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Farmer.KeyVault
open LiteDB.FSharp
open Saturn
open LiteDB
open Server.Helpers
open Shared
module Storage =
    let mapper = FSharpBsonMapper()
    let db = new LiteDatabase ("Filename=./database.db;mode=Exclusive", mapper)
    let mutable abc = 'a'



//Async<Result<'returnValue, string>>
let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    //|> Remoting.fromValue BingoApi
    |> Remoting.buildHttpHandler

let app =
    application {
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
        url "http://0.0.0.0:5000"
    }

[<EntryPoint>]
let main _ =
    run app
    0