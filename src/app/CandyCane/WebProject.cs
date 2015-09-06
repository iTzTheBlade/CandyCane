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
    public class CreateWebProjectFrom
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
                        //WebProject web = new WebProject();
                        //web.createProject(Helper._projectName);
                        //originSet = true;
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
                Repository.Clone("https://github.com/libgit2/libgit2sharp.git", Helper.GetRootPath(Helper._projectName));
                return true;
            }
            catch (Exception)
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
