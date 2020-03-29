using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        [Required]
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
    }
}
