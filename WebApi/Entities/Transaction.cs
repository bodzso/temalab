using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
    }
}
