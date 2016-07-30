using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MakeFavicons;
using MakeFavicons;
using Newtonsoft.Json.Linq;

namespace MakeFaviconConsole
{
    class Program
    {
        internal static int exitCode = 0;
        internal static Dictionary<string, string> options;

        [STAThread]
        static void Main(string[] args)
        {
            options = getArgs(args);

            if (options.ContainsKey("-w"))
            {
                if (!Directory.Exists(options["-w"]))
                {
                    ColoredConsole.WriteLine("directory not found: " + options["-w"], ConsoleColor.Red);
                    Exit(-1);
                }
                Directory.SetCurrentDirectory(options["-w"]);
                if (!options.ContainsKey("-q"))
                {
                    Console.WriteLine("Settings Directory set to: " + Directory.GetCurrentDirectory(), ConsoleColor.Green);
                }
            }

            //load settings
            if (!File.Exists("settings.json"))
            {
                ColoredConsole.WriteLine("settings.json not found", ConsoleColor.Red);
                Exit(-1);
            }
            dynamic setting = JValue.Parse(File.ReadAllText("settings.json"));

            #region OpenFile

            string inputfilename;
            string workingFolder;

            if (!options.ContainsKey("-s"))
            {
                if (options.ContainsKey("-t"))
                {
                    Console.WriteLine("Enter input image file (*.PNG)");
                    inputfilename = Console.ReadLine();
                }
                else
                {
                    var openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "PNG image|*.png";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        inputfilename = openFileDialog.FileName;
                    }
                    else
                    {
                        throw new Exception("Must select a file");
                        Exit(-1);
                    }
                }
            }
            else
            {
                inputfilename = options["-s"];
            }

            if (!options.ContainsKey("-o"))
            {
                if (options.ContainsKey("-t"))
                {
                    Console.WriteLine("Enter path to output directory");
                    workingFolder = Console.ReadLine();
                }
                else
                {
                    var workingFolderDialog = new FolderBrowserDialog();
                    if (workingFolderDialog.ShowDialog() == DialogResult.OK)
                    {
                        workingFolder = workingFolderDialog.SelectedPath;
                    }
                    else
                    {
                        throw new Exception("Must select a folder");
                    }
                }
            }
            else
            {
                workingFolder = options["-o"];
                if (!Directory.Exists(workingFolder))
                {
                    Console.WriteLine("Folder does not exist: " + workingFolder);
                    Directory.CreateDirectory(workingFolder);
                    Console.WriteLine("Created Folder: " + workingFolder);
                }
            }

            #endregion
            #region Validate PNG

            //validate existence
            if (!File.Exists(inputfilename))
            {
                ColoredConsole.WriteLine("File does not exist.", ConsoleColor.Red);
                Exit(-1);
            }

            // validate png
            if (!isPNG(inputfilename))
            {
                ColoredConsole.WriteLine("File is not a valid PNG file.", ConsoleColor.Red);
                Exit(-1);
            }


            // validate square with > mininputfilesize
            Image originalImage = Image.FromFile(inputfilename);
            if (!options.ContainsKey("-q"))
            {
                ColoredConsole.WriteLine("Selected: " + inputfilename, ConsoleColor.Green);
            }
            if (originalImage.Width != originalImage.Height || (setting.MinInputFileSize != null && originalImage.Width < Convert.ToInt32(setting.MinInputFileSize)))
            {
                if (!options.ContainsKey("-q"))
                {
                    ColoredConsole.WriteLine("Dimensions: " + originalImage.Width + " X " + originalImage.Height,
                                            ConsoleColor.Red);

                }
                ColoredConsole.WriteLine("Image must be a square larger than or equal to " + setting.MinInputFileSize + " X " + setting.MinInputFileSize, ConsoleColor.Red);
                Exit(-1);
            }
            else
            {
                if (!options.ContainsKey("-q"))
                {
                    ColoredConsole.WriteLine("Dimensions: " + originalImage.Width + " X " + originalImage.Height,
                        ConsoleColor.Green);
                }
            }

            #endregion

            var MakeFavicon = new MakeFavicons.MakeFavicon();
            var exit = exitCode | MakeFavicon.Make(setting, workingFolder, originalImage,!options.ContainsKey("-q"));
            Exit(exit | exitCode);
        }

        static void Exit(int? exitcodeoverride = null)
        {
            if (!options.ContainsKey("-c"))
            {

                Console.WriteLine("Press any key to exit.");

                while (!Console.KeyAvailable)
                {
                }
            }
            Environment.Exit(exitcodeoverride ?? exitCode);
        }

        static bool isPNG(string filename)
        {
            var header = new byte[4];
            using (var fs = new FileStream(filename, FileMode.Open))
            {
                fs.Read(header, 0, 4);
            }

            var strHeader = Encoding.ASCII.GetString(header);
            return strHeader.ToLower().EndsWith("png");
        }

        static Dictionary<string, string> getArgs(string[] args)
        {
            var result = new Dictionary<string,string>();
            var i = 0;
            while (i < args.Length)
            {
                if (args[i].StartsWith("-"))
                {
                    string val = "true";
                    if (i < args.Length-1 && !args[i+1].StartsWith("-"))
                    {
                        val = args[i + 1];
                        if (val.StartsWith("\"") && val.EndsWith("\""))
                        {
                            val = val.Substring(1, val.Length - 2);
                        }
                    }
                    result.Add(args[i], val);
                }
                i++;
            }
            return result;
        }
    }
}
