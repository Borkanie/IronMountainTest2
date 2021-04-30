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
        static Settings setari ;
        static List<string> Lines = new List<string>();
        static JObject data;
        static void Main(string[] args)
        {
            setari = new Settings();
            setari.Separator = ConfigurationManager.AppSettings["Selector"];
            setari.ImagePath = ConfigurationManager.AppSettings["ImagePath"];
            if (File.Exists("images.zip"))
            {
                Directory.CreateDirectory(setari.Zip);
                ZipFile.ExtractToDirectory("images.zip", Directory.GetCurrentDirectory()+"\\"+setari.Zip);
                File.Delete("images.zip");
            }
            ReadFromJson();
            CreateDirectories();
            MoveAllPictures();
            WriteInMetaFile();
            
            ZipFile.CreateFromDirectory(setari.Zip, "images.zip");
            
            Directory.Delete(setari.Zip,true);
            data["Number"] = setari.Number;
            data["DayOfTheWeek"] = setari.DayOfTheWeek;
            File.WriteAllText(@"data.json", data.ToString());
        }
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
                catch (IOException ex)
                {

                }
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
        static void CreateDirectories()
        {
            System.IO.Directory.CreateDirectory(setari.Zip);
            DateTime currentDate = DateTime.Now;
            System.IO.Directory.CreateDirectory(setari.Zip + "\\"+currentDate.DayOfWeek.ToString()); 
        }
        static string GetFirstPartOfString()
        {
            
            string result;
            DateTime currentDate = DateTime.Now;
            JulianCalendar julianCalendar = new JulianCalendar();
            result = currentDate.Year.ToString().Substring(2)+julianCalendar.GetDayOfYear(currentDate).ToString()+ setari.Number.ToString("00000")+ setari.Separator + currentDate.ToString("yyyy/MM/dd HH:mm:ss")+ setari.Separator;
            return result;
        }
    }
}
