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
        AddBingo: string -> AuthenticatedAPICall<BingoGuid>
        LoadBingo: BingoGuid -> APICall<MurderBingo>
    }



module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName
