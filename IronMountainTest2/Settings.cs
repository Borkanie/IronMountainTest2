using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMountainTest2
{

    class Settings
    {
        //the settigns we use in our application
        public Settings()
        {
            //starting from 0 creates errors in sql i set it up as not null in IronMointain2.2
            Number = 1;
            Zip = "zip";
        }
        #region Properties
        public int Number { get; set; }
        public string DayOfTheWeek { get; set; }
        public string Zip { get; set; }
        public string ImagePath { get; set; }
        public string Separator { get; set; }
        #endregion
    }
}
