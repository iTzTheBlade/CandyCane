using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CandyCane
{
    public class BootAndBuildBat
    {
        private static string _path;
        public BootAndBuildBat(string path)
        {
            _path = path;
        }

        public void CreateBootBat()
        {
            string content = ("@echo off\r\ncls\r\n\"tools\nuget\nuget.exe\" \"install\" \"FAKE\" \"-OutputDirectory\" \"tools\" \"-ExcludeVersion\"\r\n\"tools\nuget\nuget.exe\" \"install\" \"NUnit.Runners\" \"-OutputDirectory\" \"tools\" \"-ExcludeVersion\"");          
            string tempPath = _path + @"/00_boot.bat";                 
            
            File.WriteAllText(tempPath, content);
        }

        public void CreateBootAndBuildBat()
        {
            string content = ("@echo off\r\ncls\r\n\"tools\nuget\nuget.exe\" \"install\" \"FAKE\" \"-OutputDirectory\" \"tools\" \"-ExcludeVersion\"\r\n\"tools\nuget\nuget.exe\" \"install\" \"NUnit.Runners\" \"-OutputDirectory\" \"tools\" \"-ExcludeVersion\"\r\ncall 02_build.bat %1");
            string tempPath = _path + @"/01_boot_and_build.bat";

            File.WriteAllText(tempPath, content);            
        }

        public void RunBootAndBuiltBat()
        {
            string targetDir = string.Format(_path);

            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.WorkingDirectory = targetDir;
                proc.StartInfo.FileName = "01_boot_and_build.bat";
                proc.StartInfo.CreateNoWindow = false;
                var process = proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }

        public void CreateBuildBat()
        {
            string content = ("@echo off\r\ncls\r\n\"tools\\FAKE\\tools\\Fake.exe\" build.fsx \"%1\"\r\nexit /b %errorlevel%");
            string tempPath = _path + @"/02_build.bat";

            File.WriteAllText(tempPath, content);
        }
    }
}