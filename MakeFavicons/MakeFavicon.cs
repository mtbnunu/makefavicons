using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using MakeFavicons;
using Microsoft.CSharp.RuntimeBinder;

namespace MakeFavicons
{
    public class MakeFavicon
    {
        public MakeFavicon()
        {
            this.configFileWarnings = new Dictionary<string, List<string>>();
            this.exitCode = 0;
        }
        private Dictionary<string, List<string>> configFileWarnings;
        private int exitCode;

        public int Make(dynamic setting, string workingFolder, Image originalImage, bool loud = false)
        {
            Directory.CreateDirectory(workingFolder + setting.SiteRelativeImageFolder);
            Directory.CreateDirectory(workingFolder + setting.AppleTouchIconLocation);

            var resizer = new PNGResizer(originalImage.imageToByteArray());
            var paths = new Dictionary<string, string>();

            #region make default output png

            foreach (var s in setting.OutputFileSizes)
            {
                var size = Convert.ToInt32(s);
                var bytes = resizer.ResizeImage(size);
                var path = setting.SiteRelativeImageFolder.ToString() +
                           "/" +
                           String.Format(setting.DefaultOutputFilename.ToString(), size);
                path = Regex.Replace(path, "/+", "/");
                var fullpath = workingFolder + path;
                File.WriteAllBytes(fullpath, bytes);
                paths.Add(size.ToString(), path);
                if (loud)
                {
                    ColoredConsole.WriteLine("Complete: " + path, ConsoleColor.Cyan);
                }
            }

            #endregion

            #region make appletouchicon png

            foreach (var s in setting.AppleTouchIconSizes)
            {
                var size = Convert.ToInt32(s);
                var bytes = resizer.ResizeImage(size);
                foreach (var applefilename in setting.AppleTouchIconFilenames)
                {
                    var path = setting.AppleTouchIconLocation.ToString() +
                               "/" +
                               String.Format(applefilename.Filename.ToString(), size);
                    path = Regex.Replace(path, "/+", "/");
                    var fullpath = workingFolder + path;

                    File.WriteAllBytes(fullpath, bytes);
                    var appleFileNameId = applefilename.Id;
                    if (appleFileNameId != null)
                    {
                        var pathid = appleFileNameId.ToString() + "-" + size.ToString();
                        if (!paths.ContainsKey(pathid))
                            paths.Add(pathid, path);
                    }
                    if (loud)
                    {
                        ColoredConsole.WriteLine("Complete: " + path, ConsoleColor.Cyan);
                    }
                }
            }

            #endregion

            if (Convert.ToBoolean(setting.MakeIco))
            {
                if (paths.ContainsKey("256"))
                {
                    try
                    {
                        var icopath = setting.IcoPathAndFileName.ToString();
                        var fullicopath = workingFolder + icopath;
                        MakeIco.Make(workingFolder + paths["256"], fullicopath);
                        paths.Add("ico", icopath);
                    }
                    catch (Exception ex)
                    {
                        ColoredConsole.WriteLine("failed to create Ico file: " + ex.Message,
                            ConsoleColor.Yellow);
                        exitCode = -1;
                    }
                }
                else
                {

                    ColoredConsole.WriteLine("failed to create Ico file: include 256 to OutputFileSizes",
                        ConsoleColor.Yellow);
                    exitCode = -1;
                }
            }

            #region make configfiles

            foreach (var configFile in setting.MakeConfigFiles)
            {
                var configFileName = configFile.Filename.ToString();
                var templateFileName = configFileName + ".template";
                if (!File.Exists(configFileName + ".template"))
                {
                    ColoredConsole.WriteLine("Failed to create " + configFileName + " : Template Not Present",
                        ConsoleColor.Yellow);
                    ColoredConsole.WriteLine("      Please make a template file named  " + templateFileName,
                        ConsoleColor.Yellow);
                    ColoredConsole.WriteLine("      Refer to documentation for format", ConsoleColor.Yellow);
                    exitCode = -1;
                }
                else
                {
                    var template = File.ReadAllText(templateFileName);
                    try
                    {
                        template = fillTemplateVariable(template, configFileName, setting, paths);

                        var path = configFile.Folder.ToString() +
                                   "/" +
                                   configFileName;
                        path = Regex.Replace(path, "/+", "/");
                        paths.Add(configFileName, path);
                        var fullpath = workingFolder + path;
                        File.WriteAllText(fullpath, template);

                        Directory.CreateDirectory(workingFolder + configFile.Folder.ToString());

                        if (loud)
                        {
                            ColoredConsole.WriteLine("Complete: " + path, ConsoleColor.Cyan);
                        }
                        if (configFileWarnings.ContainsKey(configFileName))
                        {
                            configFileWarnings.Remove(configFileName);
                        }

                    }
                    catch (InvalidDataException exception)
                    {
                        ColoredConsole.WriteLine("Failed to create " + configFileName + " : " + exception.Message,
                            ConsoleColor.Yellow);
                        exitCode = -1;
                    }
                }
            }

            #endregion

            #region validate configfiles

            if (configFileWarnings.Any())
            {
                ColoredConsole.WriteLine("[WARNING] Following files may have invalid paths due to above failure",
                    ConsoleColor.Yellow);
                exitCode = -1;

                foreach (var configFileWarning in configFileWarnings)
                {
                    foreach (var invalidConfigFileName in configFileWarning.Value)
                    {
                        ColoredConsole.WriteLine(
                            "    " + invalidConfigFileName + ": path to " + configFileWarning.Key + " might be invalid",
                            ConsoleColor.Yellow);
                    }
                }
            }

            #endregion


            if (exitCode == 0)
            {
                ColoredConsole.WriteLine("All requested operation completed successfully",
                    ConsoleColor.Green);
            }

            return exitCode;
        }

        private string fillTemplateVariable(string template, string configFileName, dynamic setting, Dictionary<string, string> paths)
        {
            var configFiles = new Dictionary<string, string>();
            foreach (var configFile in setting.MakeConfigFiles)
            {
                var path = configFile.Folder.ToString() +
                           "/" +
                           configFile.Filename.ToString();
                path = Regex.Replace(path, "/+", "/");
                configFiles.Add(configFile.Filename.ToString(), path);
            }

            #region check [[if ...]]...[[endif]]

            Regex regif =
                new Regex(
                    @"\[\[if (?<iftype>true|exists) ((?<prefix>.*?):(?<val>.*?))\]\](?:[^\n]*(\n+))?(?<content>[\S\s]*?)\[\[endif\]\](?:[^\n]*(\n+))?");
            Match mif = regif.Match(template);
            while (mif.Success)
            {
                var write = false;
                if (mif.Groups["iftype"].Value.Equals("true"))
                {
                    try
                    {
                        if (mif.Groups["prefix"].Value.Equals("setting"))
                        {
                            var property = setting[mif.Groups["val"].Value];
                            if (Convert.ToBoolean(property))
                            {
                                write = true;
                            }
                        }
                        else
                        {
                            throw new InvalidDataException("if true statement only supports setting:xxx");
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new InvalidDataException("if true statement tried to access invalid field: " +
                                                       mif.Groups["prefix"] + ":" + mif.Groups["val"]);
                    }
                }
                else if (mif.Groups["iftype"].Value.Equals("exists"))
                {
                    if (mif.Groups["prefix"].Value.Equals("setting"))
                    {
                        try
                        {
                            var property = setting[mif.Groups["val"].Value];
                            if (property != null)
                            {
                                write = true;
                            }
                        }
                        catch (Exception exception)
                        {
                            if (exception is NullReferenceException || exception is RuntimeBinderException)
                            {
                                write = false;
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                    if (mif.Groups["prefix"].Value.Equals("path"))
                    {
                        if (paths.ContainsKey(mif.Groups["val"].Value))
                        {
                            write = true;
                        }
                    }
                }

                template = template.Replace(mif.Value, write
                    ? mif.Groups["content"].Value
                    : "");

                mif = regif.Match(template);
            }

            #endregion

            #region check {{prefix:val}}

            Regex reg = new Regex(@"{{(?<prefix>.*?):(?<val>.*?)}}");
            Match m = reg.Match(template);

            while (m.Success)
            {
                string replacement = null;
                if (m.Groups["prefix"].Value.Equals("path"))
                {
                    var key = m.Groups["val"].Value;

                    if (paths.ContainsKey(key))
                    {
                        replacement = paths[key];
                    }
                    //in case it is a ConfigFile not made yet
                    else if (configFiles.ContainsKey(key))
                    {
                        replacement = configFiles[key];
                        if (!configFileWarnings.ContainsKey(key))
                        {
                            configFileWarnings.Add(key, new List<string>());
                        }

                        configFileWarnings[key].Add(configFileName);
                    }
                    else
                    {
                        throw new InvalidDataException("'path " + key +
                                                       "' was not created. (please add it to settings.json)");
                    }
                }
                else if (m.Groups["prefix"].Value.Equals("setting"))
                {
                    try
                    {
                        var property = setting[m.Groups["val"].Value];
                        replacement = property.ToString();
                    }
                    catch (Exception exception)
                    {
                        if (exception is NullReferenceException || exception is RuntimeBinderException)
                        {
                            throw new InvalidDataException("Template contains unknown setting " + m.Groups["val"].Value);
                        }
                        throw;
                    }
                }
                if (replacement == null)
                {
                    throw new InvalidDataException("Template contains unknown variable " + m.Value);
                }
                template = template.Replace(m.Value, replacement);

                m = m.NextMatch();
            }

            #endregion

            return template;
        }
    }
}
