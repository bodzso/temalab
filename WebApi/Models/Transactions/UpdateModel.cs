using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Transactions
{
    public class UpdateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
    }
}
