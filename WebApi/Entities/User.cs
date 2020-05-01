using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [JsonIgnore]
        public byte[] PasswordHash { get; set; }
        [Required]
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }
        public double Balance { get; set; }
        
        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; }
        [JsonIgnore]
        public ICollection<Category> Categories { get; set; }
    }
}