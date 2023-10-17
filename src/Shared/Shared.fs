namespace Shared

open System

type BingoGuid = | Id of Guid
type MurderBingo = {
    Id: BingoGuid
    Name: string
}
type SessionToken = {
    Id: Guid
    Expires: DateTime
}

type APICall<'returnValue> = Async<Result<'returnValue, string>>
type AuthenticatedAPICall<'returnValue> = SessionToken -> APICall<'returnValue>



type IMurderBingoApi =
    {
        AddBingo: string * Guid -> Async<Result<Guid,string>>
        LoadBingo: Guid -> Async<Guid * string * string list>
        LoadBingoList: unit -> Async<(string * Guid) list>
        AddName: string * Guid * Guid-> Async<Result<unit, string>>
        LoadNames: Guid -> Async<string list>
        AuthUser: string * string -> Async<Result<Guid, string>>
    }



module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName
