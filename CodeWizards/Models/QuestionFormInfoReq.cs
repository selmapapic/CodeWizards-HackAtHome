using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeWizards.Models
{
    public class QuestionFormInfoReq
    {
        public int Godine { get; set; }
        public string Spol { get; set; }
        public string Putovanje { get; set; }
        public string Mjesto { get; set; }
        public string Kontakt { get; set; }
        public string Maska { get; set; }
        public string Bolovanje { get; set; }
        public string Vakcina { get; set; }
        public string Hronicne { get; set; }
        public string Terapija { get; set; }
        public string Rizicno { get; set; }
        public int[] Simptomi { get; set; }
    }
}
