using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace temalab.Models
{
    public class TransactionModel
    {
        public int? id { get; set; } = null;
        public virtual string name { get; set; }
        public double amount { get; set; }
        public DateTime date { get; set; }
        public virtual string description { get; set; }
        public virtual int? categoryId { get; set; } = null;
    }
}
