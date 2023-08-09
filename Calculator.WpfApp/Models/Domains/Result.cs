using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.WpfApp.Models.Domains
{
    public class Result
    {
        public int ResultId { get; set; }
        public string Expression { get; set; }
        public string Value { get; set; }
        public DateTime SaveDate { get; set; }
    }
}
