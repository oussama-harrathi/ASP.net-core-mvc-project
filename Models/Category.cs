using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace web_based.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        // Collection navigation property for EF Core relationship
        public ICollection<Contact> Contacts { get; set; }
    }
}
