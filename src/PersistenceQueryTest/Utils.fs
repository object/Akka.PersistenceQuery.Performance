namespace PersistenceQueryTest

[<AutoOpen>]
module Utils = 

    open System
    open System.Configuration
    open System.Linq
    open System.Net
    open Microsoft.FSharp.Reflection
    open Akka.Actor
    open Akka.Configuration.Hocon
    open Akka.FSharp
    open Akka.Persistence.FSharp

    let private loadAkkaConfigurationSection sectionName =
        (ConfigurationManager.GetSection(sectionName) :?> AkkaConfigurationSection).AkkaConfig

    let loadAkkaConfiguration () = 
        (loadAkkaConfigurationSection "akka/akka.actor")
            .WithFallback(loadAkkaConfigurationSection "akka/akka.persistence")
            .WithFallback(loadAkkaConfigurationSection "akka/akka.logging")
            .WithFallback(loadAkkaConfigurationSection "akka/akka.connectionStrings")

    type EventAdapter<'a>(system : Akka.Actor.ExtendedActorSystem) =

        interface Akka.Persistence.Journal.IEventAdapter with

            member this.Manifest(evt : obj) = 
                let manifestType = typeof<Newtonsoft.Json.Linq.JObject>
                sprintf "%s,%s" manifestType.FullName <| manifestType.Assembly.GetName().Name

            member this.ToJournal(evt : obj) : obj = 
                if evt :? 'a then 
                    Newtonsoft.Json.Linq.JObject.FromObject(evt) :> obj
                else 
                    null

            member this.FromJournal(evt : obj, manifest : string) : Akka.Persistence.Journal.IEventSequence =
                if evt :? Newtonsoft.Json.Linq.JObject then
                    let json = evt :?> Newtonsoft.Json.Linq.JObject
                    Akka.Persistence.Journal.EventSequence.Single(json.ToObject<'a>())
                else
                    Akka.Persistence.Journal.EventSequence.Empty;
