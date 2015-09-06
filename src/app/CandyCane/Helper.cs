using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCane
{
    public class Helper
    {
        public static string _projectName;
        public static string _projectType;
        public static bool _typeSet = false;
        internal static string GetProjectPath()
        {
            string projectPath = Environment.CurrentDirectory;
            for (int i = 0; i < 2; i++)
            {
                projectPath = System.IO.Path.GetDirectoryName(projectPath);
            }
            return projectPath + @"\";
        }

        internal static string GetRootPath(string projectName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + projectName;

            return path;
        }

        public static void ProjectCreatedStuff(string projectType)
        {
            _projectType = projectType;
            Console.WriteLine("Ein {0}projekt wurde erstellt. Beliebige Taste drücken um zu Beenden.", projectType);
            _typeSet = true;
            Console.ReadKey();
        }        
    }
}
