using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMountainTest2
{

    class Settings
    {
        public Settings()
        {
            Number = 1;
            Zip = "zip";
        }
        public int Number { get; set; }
        public string DayOfTheWeek { get; set; }
        public string Zip { get; set; }
        public string ImagePath { get; set; }
        public string Separator { get; set; }


    }
}
