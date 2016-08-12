open System
open System.Configuration
open Akka
open Akka.Actor
open Akka.Configuration.Hocon
open Akka.FSharp
open Akka.Persistence.Query
open Akka.Persistence.Query.Sql
open Akka.Streams
open Akka.Streams.Dsl

open PersistenceQueryTest

let reduce acc (item : string) = 
    String.Concat(acc + Environment.NewLine, item)

let printLine item =
    printfn "%s" item

let MaxResultCount = 16

let getAllPersistenceIds system =
    let queries = PersistenceQuery.Get(system).ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql")
    let mat = ActorMaterializer.Create(system)
    let src : Source<string, NotUsed> = queries.AllPersistenceIds()
    src.Take(int64 MaxResultCount).RunSum(System.Func<string,string,string> reduce, mat) 
    |> Async.AwaitTask 
    |> Async.RunSynchronously

[<EntryPoint>]
let main argv = 
    let akkaConfig = loadAkkaConfiguration ()
    let system = System.create "system" akkaConfig
    let result = getAllPersistenceIds system
    printfn "%s" result
    0
