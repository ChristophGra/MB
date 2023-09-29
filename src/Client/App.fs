module App

open Elmish
open Elmish.React
open Elmish.UrlParser

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif
Program.mkProgram Index.init Index.update Index.view
|> Program.toNavigable (parsePath Index.route) Index.urlUpdate
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run