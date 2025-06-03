namespace AdvancedAJAX.Models
{
    public class City
    {
        [Key]

        public int Id { get; set; }

        [Required]
        [MaxLength(3)]

        public string Code { get; set; }

        [Required]
        [MaxLength(75)]

        public string Name { get; set; }

        [ForeignKey("Country")]

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        [NotMapped]
        [MaxLength(75)]
        public string CountryName { get; set; }

    }

}
