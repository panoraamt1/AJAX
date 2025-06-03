using System.ComponentModel.DataAnnotations;

namespace AdvancedAJAX.Models
{
    public class Country
    {
        public int Id { get; set; }

        [Required]
        [StringLength(3)]
        public string Code { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [Display(Name = "Currency/Name")]
        [StringLength(75)]
        public string? CurrencyName { get; set; }

        public ICollection<City>? Cities { get; set; }
    }
}