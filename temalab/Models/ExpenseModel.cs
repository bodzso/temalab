using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace temalab.Models
{
    public class ExpenseModel : TransactionModel
    {
        public int? categoryId { get; set; }
        public string categoryName { get; set; }
    }
}
