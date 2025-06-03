using System.ComponentModel.DataAnnotations;

namespace AjaxCustomerCRUD.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(3)]
        public string Code { get; set; }

        [Required]
        [MaxLength(75)]
        public string Name { get; set; }

        [MaxLength(75)]
        public string CurrencyName { get; set; } = "";
    }
}
