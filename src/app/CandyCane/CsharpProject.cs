using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CandyCane;
using System.IO;
using LibGit2Sharp;

namespace CandyCane
{
    public class CreateCsharpProjectFrom
    {
        public void createProjectFrom(string projectOrigin)
        {
            bool originSet = false;

            while (originSet == false)
            {
                switch (projectOrigin)
                {
                    case "git":
                        Console.WriteLine("Das kann eine Weile dauern...");
                        originSet = createProjectGit(projectOrigin);
                        Console.WriteLine("Repository erfolgreich geclont.");
                        break;

                    case "candycane":
                        break;

                    default:
                        Console.WriteLine("Ihre Eingabe war nicht korrekt, bitte nochmals versuchen.");
                        break;
                }
            }
        }

        public bool createProjectGit(string from)
        { 
            try
            {
                Repository.Clone("https://github.com/iTzTheBlade/CandyCane_CsharpProject", Helper.GetRootPath(Helper._projectName));
                Console.WriteLine("Repository cloned");
                Console.WriteLine("Try to build the project");

                BuildScript buildScript = new BuildScript(Helper.GetRootPath(Helper._projectName), Helper._projectName);
                buildScript.CreateBuildScript();

                BootAndBuildBat batActions = new BootAndBuildBat(Helper.GetRootPath(Helper._projectName));
                batActions.RunBootAndBuiltBat();

                Console.WriteLine("Built project");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool createProjectCandyCane(string from)
        {

            return false;
        }
    }  
}