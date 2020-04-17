using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Transactions
{
    public class CreateModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public double? Amount { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
    }
}
