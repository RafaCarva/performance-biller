using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceBiller.Models
{
    public class Invoices
    {
        public string customerName { get; set; }
        public List<Performace> performList { get; set; }
    }
}
