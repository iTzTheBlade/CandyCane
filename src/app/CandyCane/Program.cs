using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CandyCane;

namespace CandyCane
{
    public class Program
    {        
        static void Main(string[] args)
        {
            Console.WriteLine("Projekt Name:");
            Helper._projectName = Console.ReadLine().ToLower();

            Console.WriteLine("Projekt Typ:");
            Console.WriteLine("Für eine Liste von Typen geben sie help ein.");

            string enteredText = null;

            while (Helper._typeSet == false)
            {
                enteredText = Console.ReadLine().ToLower();

                switch (enteredText)
                {
                    case "web":
                        CreateWebProjectFrom webProject = new CreateWebProjectFrom();     
                        Console.WriteLine("Von wo soll das Projekt erstellt werden, candycane oder git?");
                        webProject.createProjectFrom(Console.ReadLine().ToLower());
                        Helper.ProjectCreatedStuff(enteredText);
                        break;

                    case "c#":
                        CreateCsharpProjectFrom cSharpProject = new CreateCsharpProjectFrom();
                        Console.WriteLine("Von wo soll das Projekt erstellt werden, candycane oder git?");
                        cSharpProject.createProjectFrom(Console.ReadLine().ToLower());
                        Helper.ProjectCreatedStuff(enteredText);
                        break;

                    case "help":
                        Console.WriteLine("web");
                        Console.WriteLine("c#");
                        Console.WriteLine("buildscript");
                        break;

                    case "buildscript":
                        Console.WriteLine("Bitte Projektpfad angeben");
                        var path = Console.ReadLine();
                        BuildScript buildscript = new BuildScript(path, Helper._projectName);
                        buildscript.CreateBuildScript();
                        Console.WriteLine("BuildScript wurde erfolgreich erstellt");
                        break;

                    default:
                        Console.WriteLine("Ihre Eingabe war nicht korrekt, bitte nochmals versuchen.");
                        break;
                }
            }
            
        }
    }
}