#r "paket:
version 5.247.2
framework: netstandard20
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 4.2.0 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open ``Build-generic``

let product = "Functional Living"
let copyright = "Copyright (c) Cumps Consulting"
let company = "Cumps Consulting"

let dockerRepository = "functional-living"
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

let build = buildSolution assemblyVersionNumber
let setVersions = (setSolutionVersions assemblyVersionNumber product copyright company)
let setVersionsFSharp = (setSolutionVersionsFSharp assemblyVersionNumber product copyright company)
let test = testSolution
let publish = publish assemblyVersionNumber
let pack = pack nugetVersionNumber
let containerize = containerize dockerRepository
let push = push dockerRepository

supportedRuntimeIdentifiers <- [ "linux-x64" ]

Target.create "Restore_Solution" (fun _ -> restore "FunctionalLiving")

Target.create "Build_Solution" (fun _ ->
  setVersions "SolutionInfo.cs"
  setVersionsFSharp "SolutionInfo.fs"
  build "FunctionalLiving")

Target.create "Test_Solution" (fun _ -> test "FunctionalLiving")

Target.create "Publish_Solution" (fun _ ->
  [
    "FunctionalLiving.Api"
    "FunctionalLiving.Knx.Listener"
    "FunctionalLiving.Knx.Sender"
  ] |> List.iter publish)

Target.create "Pack_Solution" (fun _ ->
  [
    "FunctionalLiving.Api"
  ] |> List.iter pack)

Target.create "Containerize_Api" (fun _ -> containerize "FunctionalLiving.Api" "api")
Target.create "PushContainer_Api" (fun _ -> push "api")

Target.create "Containerize_KnxListener" (fun _ -> containerize "FunctionalLiving.Knx.Listener" "knx-listener")
Target.create "PushContainer_KnxListener" (fun _ -> push "knx-listener")

Target.create "Containerize_KnxSender" (fun _ -> containerize "FunctionalLiving.Knx.Sender" "knx-sender")
Target.create "PushContainer_KnxSender" (fun _ -> push "knx-sender")

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
