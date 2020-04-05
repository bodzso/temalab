using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace temalab.Models
{
    public class TransactionModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public double amount { get; set; }
        public DateTime date { get; set; }
        public string description { get; set; }
    }
}
