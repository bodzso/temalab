using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
