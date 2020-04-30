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

        public User User { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
