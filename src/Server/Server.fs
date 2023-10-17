module Server.Server

open System
open System.Collections.Generic
open DevOne.Security.Cryptography.BCrypt
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Farmer.KeyVault
open LiteDB.FSharp
open Saturn
open LiteDB
open Shared

type murderBingo =
    {
        Id: Guid
        BingoName: string
        People: string list
    }
type authenticatedUsers = {
    Id: Guid
    key: Guid
    UserName: string
    passwordHash: string
}
module Storage =
    let mapper = FSharpBsonMapper()
    let db = new LiteDatabase ("Filename=./database.db;mode=Exclusive", mapper)
    let bingos = db.GetCollection<murderBingo> "murderBingo"
    let authenticatedUsers = db.GetCollection<authenticatedUsers> "authenticatedUsers"
    let authenticate username password =
        match Seq.tryHead <| authenticatedUsers.Find (fun x -> x.UserName = username) with
        | Some h ->
            match BCryptHelper.CheckPassword (password,h.passwordHash) with
            | true ->
                Ok h.key
            | false ->
                Error ""
        | None ->
            Error ""
    let mapAuthenticated key func =
        match authenticatedUsers.Exists (fun x -> x.key = key) with
        | true -> Ok (func ())
        | false -> Error "invalid token"
    let addBingo name =
        let bingo = {Id = Guid.NewGuid(); BingoName = name; People = []}
        bingos.Insert bingo |> ignore
        bingo.Id
    let getBingo (id: Guid) =
        let bingo = bingos.FindById id
        id, bingo.BingoName, bingo.People
    let getAllBingos () =
        bingos.FindAll ()
        |> Seq.map (fun x -> x.BingoName, x.Id)
        |> List.ofSeq
    let AddNameToBingo id name =
        let bingo = bingos.FindById (BsonValue.op_Implicit id: Guid)
        let newBingo = {bingo with People = name :: bingo.People}
        bingos.Update newBingo |> ignore
    let GetNames id =
        let bingo = bingos.FindById id
        bingo.People

let (BingoApi: IMurderBingoApi) =
    {
        AddBingo = fun (name,userKey) -> async { return Storage.mapAuthenticated userKey (fun _ -> (Storage.addBingo name))}
        LoadBingo = fun id -> async { return (Storage.getBingo id) }
        LoadBingoList = fun _ -> async {return (Storage.getAllBingos ())}
        AddName = fun (name, id, userkey) -> async { return Storage.mapAuthenticated userkey (fun _ ->  Storage.AddNameToBingo id name)}
        LoadNames = fun (id) -> async {return Storage.GetNames id}
        AuthUser = fun (name, password) -> async { return Storage.authenticate name password }
    }

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue BingoApi
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