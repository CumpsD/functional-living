#r "paket:
version 6.0.0-beta8
framework: netstandard20
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 5.0.1 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO.FileSystemOperators
open ``Build-generic``

let product = "Functional Living"
let copyright = "Copyright (c) Cumps Consulting"
let company = "Cumps Consulting"

let dockerRepository = "cumpsd"
let assemblyVersionNumber = (sprintf "2.%s")
let nugetVersionNumber = (sprintf "2.%s")

let setSolutionVersionsFSharp formatAssemblyVersion product copyright company x =
  AssemblyInfoFile.createFSharp x
      [AssemblyInfo.Version (formatAssemblyVersion buildNumber)
       AssemblyInfo.FileVersion (formatAssemblyVersion buildNumber)
       AssemblyInfo.InformationalVersion gitHash
       AssemblyInfo.Product product
       AssemblyInfo.Copyright copyright
       AssemblyInfo.Company company]

let buildTestFSharp' formatAssemblyVersion project =
  buildNeutral formatAssemblyVersion ("test" @@ project @@ (sprintf "%s.fsproj" project))

let buildSource = build assemblyVersionNumber
let buildTest = buildTest assemblyVersionNumber
let buildTestFSharp = buildTestFSharp' assemblyVersionNumber
let setVersions = (setSolutionVersions assemblyVersionNumber product copyright company)
let setVersionsFSharp = (setSolutionVersionsFSharp assemblyVersionNumber product copyright company)
let publish = publish assemblyVersionNumber
let pack = pack nugetVersionNumber
let containerize = containerize dockerRepository
let push = push dockerRepository

supportedRuntimeIdentifiers <- [ "linux-x64" ]

Target.create "Restore_Solution" (fun _ ->
  restore "FunctionalLiving"
)

Target.create "Build_Solution" (fun _ ->
  setVersions "SolutionInfo.cs"
  setVersionsFSharp "SolutionInfo.fs"

  buildSource "FunctionalLiving.Api"
  buildSource "FunctionalLiving.Knx.Listener"
  buildSource "FunctionalLiving.Knx.Sender"

  buildTestFSharp "FunctionalLiving.Knx.Parser.Tests"
  buildTest "FunctionalLiving.Knx.Tests"
  buildTest "FunctionalLiving.Tests"
)

Target.create "Test_Solution" (fun _ ->
    [
        "test" @@ "FunctionalLiving.Knx.Parser.Tests"
        "test" @@ "FunctionalLiving.Knx.Tests"
        "test" @@ "FunctionalLiving.Tests"
    ] |> List.iter testWithDotNet
)

Target.create "Publish_Solution" (fun _ ->
  [
    "FunctionalLiving.Api"
    "FunctionalLiving.Knx.Listener"
    "FunctionalLiving.Knx.Sender"
  ] |> List.iter publish
)

Target.create "Pack_Solution" (fun _ ->
  [
    "FunctionalLiving.Api"
  ] |> List.iter pack
)

Target.create "Containerize_Api" (fun _ -> containerize "FunctionalLiving.Api" "functional-living-api")
Target.create "PushContainer_Api" (fun _ -> push "functional-living-api")

Target.create "Containerize_KnxListener" (fun _ -> containerize "FunctionalLiving.Knx.Listener" "functional-living-knx-listener")
Target.create "PushContainer_KnxListener" (fun _ -> push "functional-living-knx-listener")

Target.create "Containerize_KnxSender" (fun _ -> containerize "FunctionalLiving.Knx.Sender" "functional-living-knx-sender")
Target.create "PushContainer_KnxSender" (fun _ -> push "functional-living-knx-sender")

Target.create "Build" ignore
Target.create "Test" ignore
Target.create "Publish" ignore
Target.create "Pack" ignore
Target.create "Containerize" ignore
Target.create "Push" ignore

"NpmInstall"
  ==> "DotNetCli"
  ==> "Clean"
  ==> "Restore_Solution"
  ==> "Build_Solution"
  ==> "Build"

"Build"
  ==> "Test_Solution"
  ==> "Test"

"Test"
  ==> "Publish_Solution"
  ==> "Publish"

"Publish"
  ==> "Pack_Solution"
  ==> "Pack"

"Pack"
  ==> "Containerize_Api"
  ==> "Containerize_KnxListener"
  ==> "Containerize_KnxSender"
  ==> "Containerize"
// Possibly add more projects to containerize here

"Containerize"
  ==> "DockerLogin"
  ==> "PushContainer_Api"
  ==> "PushContainer_KnxListener"
  ==> "PushContainer_KnxSender"
  ==> "Push"
// Possibly add more projects to push here

// By default we build & test
Target.runOrDefault "Test"
