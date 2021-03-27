using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeWizards.Models
{
    public class CovidStatistics
    {
        public int Confirmed { get; set; }
        public int Deaths { get; set; }
        public int Recovered { get; set; }
        public int Active { get; set; }
        public DateTime Date { get; set; }

    }
}
