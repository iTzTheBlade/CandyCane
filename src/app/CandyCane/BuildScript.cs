using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCane
{
    public class BuildScript
    {
        private string _path;
        private string _name;

        public BuildScript(string path, string name)
        {
            this._name = name;
            this._path = path;
        }

        public List<string> folders()
        {
            var folders = new List<string>();

            foreach (string folder in Directory.GetDirectories(_path + "\\src"))
                folders.Add(Path.GetFileName(folder));

            return folders;
        }

        public void CreateBuildScript()
        {
            List<string> buildScript = new List<string>();

            buildScript.Add("#I @\"tools\\FAKE\\tools\\\"\r\n");
            buildScript.Add("#r @\"tools\\FAKE\\tools\\FakeLib.dll\"\r\n");
            buildScript.Add("\r\n");

            buildScript.Add("open Fake\r\n");
            buildScript.Add("open Fake.AssemblyInfoFile\r\n");
            buildScript.Add("open Fake.Git\r\n");
            buildScript.Add("open System.IO\r\n");
            buildScript.Add("\r\n");

            buildScript.Add("let projectName           = " + "\"" + _name + "\"\r\n");
            buildScript.Add("\r\n");

            buildScript.Add("//Directories\r\n");
            buildScript.Add("let buildDir              = @\".\\build\"\r\n");
            buildScript.Add("\r\n");
            foreach (var folder in folders())
            {
                if (folder == "js")
                {
                    continue;
                }

                if (folder.ToLower() == "test")
                {
                    buildScript.Add("let testDir              = buildDir + @\".\\test\"\r\n");
                }
                else
                {
                    buildScript.Add("let " + folder + "BuildDir           = buildDir + @\"\\" + folder + "\"\r\n");
                }
            }

            buildScript.Add("\r\n");
            buildScript.Add("let deployDir               = @\".\\Publish\"\r\n");
            buildScript.Add("\r\n");
            buildScript.Add("let packagesDir             = @\".\\packages\\\"\r\n");
            buildScript.Add("\r\n");

            buildScript.Add("let mutable version         = \"1.0\"\r\n");
            buildScript.Add("let mutable build           = buildVersion\r\n");
            buildScript.Add("let mutable nugetVersion    = \"\"\r\n");
            buildScript.Add("let mutable asmVersion      = \"\"\r\n");
            buildScript.Add("let mutable asmInfoVersion  = \"\"\r\n");
            buildScript.Add("let mutable setupVersion    = \"\"\r\n");
            buildScript.Add("\r\n");

            buildScript.Add("let gitbranch = Git.Information.getBranchName \".\"\r\n");
            buildScript.Add("let sha = Git.Information.getCurrentHash()\r\n");
            buildScript.Add("\r\n");

            buildScript.Add("Target \"Clean\" (fun _ ->\r\n");
            if (folders().Contains("test"))
            {
                buildScript.Add("    CleanDirs [buildDir; deployDir; testDir]\r\n");
            }
            else
            {
                buildScript.Add("    CleanDirs [buildDir; deployDir]\r\n");
            }
            buildScript.Add(")\r\n");
            buildScript.Add("\r\n");

            buildScript.Add("Target \"RestorePackages\" (fun _ ->\r\n");
            buildScript.Add("   RestorePackages()\r\n");
            buildScript.Add(")\r\n");
            buildScript.Add("\r\n");

            buildScript.Add("Target \"BuildVersions\" (fun _ ->\r\n");
            buildScript.Add("\r\n");
            buildScript.Add("    let safeBuildNumber = if not isLocalBuild then build else \"0\"\r\n");
            buildScript.Add("\r\n");
            buildScript.Add("    asmVersion      <- version + \".\" + safeBuildNumber\r\n");
            buildScript.Add("    asmInfoVersion  <- asmVersion + \" - \" + gitbranch + \" - \" + sha\r\n");
            buildScript.Add("\r\n");
            buildScript.Add("    nugetVersion    <- version + \".\" + safeBuildNumber\r\n");
            buildScript.Add("    setupVersion    <- version + \".\" + safeBuildNumber\r\n");
            buildScript.Add("\r\n");
            buildScript.Add("    match gitbranch with\r\n");
            buildScript.Add("        | \"master\" -> ()\r\n");
            buildScript.Add("        | \"develop\" -> (nugetVersion <- nugetVersion + \" - \" + \"beta\")\r\n");
            buildScript.Add("        | _ -> (nugetVersion <- nugetVersion + \" - \" + gitbranch)\r\n");
            buildScript.Add("\r\n");
            buildScript.Add("    SetBuildNumber nugetVersion\r\n");
            buildScript.Add(")");
            buildScript.Add("\r\n");

            buildScript.Add("Target \"AssemblyInfo\" (fun _ ->\r\n");
            buildScript.Add("    BulkReplaceAssemblyInfoVersions \"src/\" (fun f ->\r\n");
            buildScript.Add("                                              {f with\r\n");
            buildScript.Add("                                                  AssemblyVersion = asmVersion\r\n");
            buildScript.Add("                                                  AssemblyInformationalVersion = asmInfoVersion})\r\n");
            buildScript.Add(")\r\n");
            buildScript.Add("\r\n");

            foreach (var folder in folders())
            {
                if (folder == "js")
                {
                    continue;
                }

                if (folder.ToLower() == "test")
                {
                    buildScript.Add("Target \"BuildTest\" (fun _->\r\n");
                    buildScript.Add("    !! @\"src\\" + folder + "\\*.csproj\"\r\n");
                    buildScript.Add("      |> MSBuildRelease " + folder + "Dir \"Build\"\r\n");
                    buildScript.Add("      |> Log \"Build - Output: \"\r\n");
                    buildScript.Add(")\r\n");
                    buildScript.Add("\r\n");
                }
                else
                {
                    buildScript.Add("Target \"Build" + folder + "\" (fun _->\r\n");
                    buildScript.Add("    !! @\"src\\" + folder + "\\*.csproj\"\r\n");
                    buildScript.Add("      |> MSBuildRelease " + folder + "BuildDir \"Build\"\r\n");
                    buildScript.Add("      |> Log \"Build - Output: \"\r\n");
                    buildScript.Add(")\r\n");
                    buildScript.Add("\r\n");
                }

            }

            if (folders().Contains("test"))
            {
                buildScript.Add("Target \"NUnitTest\" (fun _ ->\r\n");
                buildScript.Add("    if (Directory.GetFiles(testDir).Length <> 0) then\r\n");
                buildScript.Add("        !! (testDir + @\"\\*.Tests.dll\")\r\n");
                buildScript.Add("            |> NUnit (fun p ->\r\n");
                buildScript.Add("                {p with\r\n");
                buildScript.Add("                    ToolPath = @\".\\tools\\NUnit.Runners\\tools\\\";\r\n");
                buildScript.Add("                    Framework = \"net - 4.0\";\r\n");
                buildScript.Add("                    DisableShadowCopy = true;\r\n");
                buildScript.Add("                    OutputFile = testDir + @\"\\TestResults.xml\"})\r\n");
                buildScript.Add(")\r\n");
                buildScript.Add("\r\n");
            }
            buildScript.Add("Target \"Zip\" (fun _ ->\r\n");
            buildScript.Add("    !! (buildDir @@ @\"\\**\\*.* \")\r\n");
            buildScript.Add("        -- \" *.zip\"\r\n");
            buildScript.Add("            |> Zip buildDir (buildDir + projectName + version + \".zip\")\r\n");
            buildScript.Add(")\r\n");
            buildScript.Add("\r\n");

            buildScript.Add("Target \"Publish\" (fun _ ->\r\n");
            buildScript.Add("    CreateDir deployDir\r\n");
            buildScript.Add("\r\n");
            buildScript.Add("    !! (buildDir @@ @\"/**/*.* \")\r\n");
            buildScript.Add("      -- \" *.pdb\"\r\n");
            buildScript.Add("        |> CopyTo deployDir\r\n");
            buildScript.Add(")\r\n");
            buildScript.Add("\r\n");

            buildScript.Add("\"Clean\"\r\n");
            buildScript.Add("  ==> \"RestorePackages\"\r\n");
            buildScript.Add("  ==> \"BuildVersions\"\r\n");
            buildScript.Add("  =?> (\"AssemblyInfo\", not isLocalBuild )\r\n");
            foreach (var folder in folders())
            {
                if (folder == "js")
                {
                    continue;
                }

                buildScript.Add("  ==> \"Build" + folder + "\"\r\n");
            }
            if (folders().Contains("test"))
            {
                buildScript.Add("  ==> \"NUnitTest\"\r\n");
            }
            buildScript.Add("  ==> \"Zip\"\r\n");
            buildScript.Add("  ==> \"Publish\"\r\n");
            buildScript.Add("\r\n");

            buildScript.Add("\r\n");
            buildScript.Add("RunTargetOrDefault \"Publish\"");

            string tempBuildScript = "";

            for (int i = 0; i < buildScript.Count; i++)
            {
                tempBuildScript += buildScript[i];
            }

            if (!File.Exists(_path + @"\build.fsx"))
            {
                File.WriteAllText(_path + @"\build.fsx", tempBuildScript);
            }
            else
            {
                Console.WriteLine("Datei existiert bereits, wenn die Datei überschrieben werden soll, antworten Sie bitte mit \"Ja\"");
                var input = Console.ReadLine().ToLower();

                if (input == "ja")
                {
                    File.Delete(_path + @"\build.fsx");
                    File.WriteAllText(_path + @"\build.fsx", tempBuildScript);
                }
                else
                {
                    Console.WriteLine("Programm wird beendet");
                }
            }
        }
    }
}
