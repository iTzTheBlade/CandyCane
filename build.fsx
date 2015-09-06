#I @"tools\FAKE\tools\"
#r @"tools\FAKE\tools\FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open Fake.Git
open System.IO

let projectName           = "candycane"

//Directories
let buildDir              = @".\build"

let appBuildDir           = buildDir + @"\app"

let deployDir               = @".\Publish"

let packagesDir             = @".\packages\"

let mutable version         = "1.0"
let mutable build           = buildVersion
let mutable nugetVersion    = ""
let mutable asmVersion      = ""
let mutable asmInfoVersion  = ""
let mutable setupVersion    = ""

let gitbranch = Git.Information.getBranchName "."
let sha = Git.Information.getCurrentHash()

Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "RestorePackages" (fun _ ->
   RestorePackages()
)

Target "BuildVersions" (fun _ ->

    let safeBuildNumber = if not isLocalBuild then build else "0"

    asmVersion      <- version + "." + safeBuildNumber
    asmInfoVersion  <- asmVersion + " - " + gitbranch + " - " + sha

    nugetVersion    <- version + "." + safeBuildNumber
    setupVersion    <- version + "." + safeBuildNumber

    match gitbranch with
        | "master" -> ()
        | "develop" -> (nugetVersion <- nugetVersion + " - " + "beta")
        | _ -> (nugetVersion <- nugetVersion + " - " + gitbranch)

    SetBuildNumber nugetVersion
)
Target "AssemblyInfo" (fun _ ->
    BulkReplaceAssemblyInfoVersions "src/" (fun f ->
                                              {f with
                                                  AssemblyVersion = asmVersion
                                                  AssemblyInformationalVersion = asmInfoVersion})
)

Target "Buildapp" (fun _->
    !! @"src\app\*.csproj"
      |> MSBuildRelease appBuildDir "Build"
      |> Log "Build - Output: "
)

Target "Zip" (fun _ ->
    !! (buildDir @@ @"\**\*.* ")
        -- " *.zip"
            |> Zip buildDir (buildDir + projectName + version + ".zip")
)

Target "Publish" (fun _ ->
    CreateDir deployDir

    !! (buildDir @@ @"/**/*.* ")
      -- " *.pdb"
        |> CopyTo deployDir
)

"Clean"
  ==> "RestorePackages"
  ==> "BuildVersions"
  =?> ("AssemblyInfo", not isLocalBuild )
  ==> "Buildapp"
  ==> "Zip"
  ==> "Publish"


RunTargetOrDefault "Publish"