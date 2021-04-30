using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;

namespace IronMountainTest2
{
    class Program
    {
        //This program contains the code for the first application in the second part of the Test
        #region Fields
        static Settings setari ;
        static List<string> Lines = new List<string>();
        static JObject data;
        static string ZipName="images.zip";
        #endregion
        //Main function we call everything from here
        //the bot has to run once and have the App.config correctly set up
        //it will copy all the files from the ImagePath folder into the images.zip file
        //the name of the zipfile is hardcoded in the ZipName variable but it should be the same in both applications this and the IronMointains2.2-the second app of the second part of the test
        static void Main(string[] args)
        {
            setari = new Settings();
            setari.Separator = ConfigurationManager.AppSettings["Selector"];
            setari.ImagePath = ConfigurationManager.AppSettings["ImagePath"];
            //we check if the zip file already exists and in that case we complete it
            if (File.Exists(ZipName))
            {
                Directory.CreateDirectory(setari.Zip);
                ZipFile.ExtractToDirectory(ZipName, Directory.GetCurrentDirectory()+"\\"+setari.Zip);
                File.Delete(ZipName);
            }
            ReadFromJson();
            CreateDirectories();
            MoveAllPictures();
            WriteInMetaFile();
            
            ZipFile.CreateFromDirectory(setari.Zip, ZipName);
            
            Directory.Delete(setari.Zip,true);
            data["Number"] = setari.Number;
            data["DayOfTheWeek"] = setari.DayOfTheWeek;
            File.WriteAllText(@"data.json", data.ToString());
        }
        #region Methods
        //we read the Current number and day from the data.json file
        //please do no make the file manually it will be automaticly set up if it doesnt exist
        static void ReadFromJson()
        {
            if (File.Exists("data.json"))
            {
                using (StreamReader file = File.OpenText(@"data.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    data = (JObject)JToken.ReadFrom(reader);
                }
                setari.Number = Convert.ToInt32(data["Number"]);
                if (data["DayOfTheWeek"].ToString() != DateTime.Now.DayOfWeek.ToString())
                {
                    data["DayOfTheWeek"] = DateTime.Now.DayOfWeek.ToString();
                    setari.Number = 0;

                }
                setari.DayOfTheWeek = data["DayOfTheWeek"].ToString();
            }
            else
            {
                data = new JObject();
                setari.Number = 0;
                setari.DayOfTheWeek = DateTime.Now.DayOfWeek.ToString();
                string json = "{\n  \"Number\":" + setari.Number.ToString() + ",\n  \"DayOfTheWeek\":\"" + setari.DayOfTheWeek + "\"\n}";
                File.WriteAllText(@"data.json", json);
            }
        }
        //Here we write the paths and all the encodings in the Paths.meta File
        static void WriteInMetaFile()
        {
            if (!File.Exists(setari.Zip + "\\" + "Paths.meta"))
            {
                try
                {
                    var file = File.Open(setari.Zip + "\\" + "Paths.meta", FileMode.CreateNew);
                    file.Close();
                    file.Dispose();
                }
                catch (IOException) { }
            }
            
            using (StreamWriter outputFile = new StreamWriter(Directory.GetCurrentDirectory() + "\\" + setari.Zip + "\\Paths.meta",true))
            {
                foreach (var line in Lines)
                {
                    outputFile.WriteLine(line);
                    Console.WriteLine(line);
                }
            }
        }
        //In these function we copy all the files from the target folder into our zip directory
        //we will in the end convert the directory into a zip file by compressing
        static void MoveAllPictures()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(setari.ImagePath);
            List<String> MyMusicFiles = Directory.GetFiles(setari.ImagePath, "*.*", SearchOption.AllDirectories).ToList();
            DateTime currentDate = DateTime.Now;
            foreach (string file in MyMusicFiles)
            {
                try
                {
                    FileInfo mFile = new FileInfo(file);
                    // to remove name collisions
                    if (new FileInfo(dirInfo + "\\" + mFile.Name.Split("\\")[mFile.Name.Split("\\").Length - 1]).Exists != false)
                    {
                        if (!File.Exists(setari.Zip + "\\" + currentDate.DayOfWeek + "\\" + mFile.Name.Split("\\")[mFile.Name.Split("\\").Length - 1]))
                        {
                            mFile.CopyTo(setari.Zip + "\\" + currentDate.DayOfWeek + "\\" + mFile.Name.Split("\\")[mFile.Name.Split("\\").Length - 1]);
                            Lines.Add(GetFirstPartOfString() + currentDate.DayOfWeek + "\\" + mFile.Name.Split("\\")[mFile.Name.Split("\\").Length - 1]);
                            setari.Number++;
                        }   
                    }
                }
                catch(IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }
              
            }
        }
        //here we generata Directory for the current day if we have to
        static void CreateDirectories()
        {
            System.IO.Directory.CreateDirectory(setari.Zip);
            DateTime currentDate = DateTime.Now;
            System.IO.Directory.CreateDirectory(setari.Zip + "\\"+currentDate.DayOfWeek.ToString()); 
        }
        //here we generate most of the Paths.meta line excluding the relative path to the file itself
        static string GetFirstPartOfString()
        {
            string result;
            DateTime currentDate = DateTime.Now;
            JulianCalendar julianCalendar = new JulianCalendar();
            result = currentDate.Year.ToString().Substring(2)+julianCalendar.GetDayOfYear(currentDate).ToString()+ setari.Number.ToString("00000")+ setari.Separator + currentDate.ToString("yyyy/MM/dd HH:mm:ss")+ setari.Separator;
            return result;
        }
        #endregion
    }
}
