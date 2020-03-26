using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int UserId { get; set; }
    }
}
