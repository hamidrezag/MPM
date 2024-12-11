using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utils
{
    public class PersianDateFormat
    {

        public string Day;
        public string Month;
        public string Year;
        public string Hour;
        public string Minute;
        public string Second;
        public string DayOfWeek;
        public string MonthName;
        public string Time;
        public string Date;
        public string DateTime;
        /// <summary>
        /// برای مثال 961106
        /// </summary>
        public int DateNumber;
        /// <summary>
        /// برای مثال پنجشنبه 1396/05/15
        /// </summary>
        public string DateDayName;
        /// <summary>
        ///YYYYMMDDHHmmss
        /// </summary>
        public long DateNumberWithTime;

        //yyyymmdd
        public long DateNumberWithCompleteYear;

        public string DateDayNameWithoutTime { get; internal set; }
    }
}
