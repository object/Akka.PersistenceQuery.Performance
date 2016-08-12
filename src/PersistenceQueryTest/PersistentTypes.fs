namespace PersistenceQueryTest

[<RequireQualifiedAccess>]
module PersistentTypes =

    open System

    type PersistentId = 
        | VolumeId of string
        | AkamaiStorageId of string
        | OriginStorageId of string
        | FileIdentity of string
        member this.QualifiedId = 
            this |> function 
                    | VolumeId x -> sprintf "akamai-volume-usage:%s" x
                    | AkamaiStorageId x -> sprintf "akamai-storage-assignment:%s" x
                    | OriginStorageId x -> sprintf "origin-storage-assignment:%s" x
                    | FileIdentity x -> sprintf "file-distribution:%s" x

    type AkamaiDiskUsage() =
        member val TotalBytes : uint64 = 0UL with get, set
        member val AvailableBytes : uint64 = 0UL with get, set
        static member Zero = AkamaiDiskUsage()

    type AkamaiDiskUsageState() =
        member val VolumeId : PersistentId = VolumeId(String.Empty) with get, set
        member val DiskUsage : AkamaiDiskUsage = AkamaiDiskUsage() with get, set

    type AkamaiStorageAssignment() = 
        member val StorageId : string = null with get, set
        member val EdgeChar : string = null with get, set
        static member Zero = AkamaiStorageAssignment(StorageId = String.Empty, EdgeChar = String.Empty)

    type AkamaiStorageAssignmentState() =
        member val StorageId : PersistentId = AkamaiStorageId(String.Empty) with get, set
        member val Storage : AkamaiStorageAssignment = AkamaiStorageAssignment() with get, set

    type OriginStorageAssignment() = 
        member val StorageId : string = null with get, set
        member val ManifestUrl : string = null with get, set
        static member Zero = OriginStorageAssignment(StorageId = String.Empty, ManifestUrl = String.Empty)

    type OriginStorageAssignmentState() =
        member val StorageId : PersistentId = OriginStorageId(String.Empty) with get, set
        member val Storage : OriginStorageAssignment = OriginStorageAssignment() with get, set

    type FileDistribution() =
        member val StorageProvider : string = null with get, set
        member val Locator : string = null with get, set
        member val CdnPath : string = null with get, set
        member val GeoRestriction : string = null with get, set
        member val AccessLevel : string = null with get, set
        member val State : string = null with get, set
        member val Length : uint64 = 0UL with get, set
        member val Timestamp : DateTimeOffset = DateTimeOffset.MinValue with get, set
        static member Zero = FileDistribution(
                                StorageProvider = String.Empty, 
                                Locator = String.Empty, 
                                CdnPath = String.Empty, 
                                GeoRestriction = String.Empty, 
                                AccessLevel = String.Empty, 
                                State = String.Empty)

    type FileDistributionState() =
        member val FileIdentity : PersistentId = FileIdentity(String.Empty) with get, set
        member val Locators : FileDistribution array = Array.empty with get, set
