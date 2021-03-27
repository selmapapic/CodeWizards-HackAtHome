using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeWizards.Models
{
    public class TodayAndGeneralStatistics
    {
        public CovidStatisticsList CovidGeneral { get; set; }
        public TodayCases CovidToday { get; set; }
    }
}
