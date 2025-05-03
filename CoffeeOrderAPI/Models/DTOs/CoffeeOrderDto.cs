using System.ComponentModel.DataAnnotations;

namespace CoffeeOrderAPI.Models.DTOs
{
    public class CoffeeOrderDto
    {
        [Required]
        [RegularExpression("^(Small|Medium|Large)$", ErrorMessage = "Size must be Small, Medium, or Large.")]
        public required string Size { get; set; }
        
        [Required]
        [RegularExpression("^(Latte|Espresso|Cappuccino)$", ErrorMessage = "Invalid coffee type.")]
        public required string CoffeeType { get; set; }

        [Required]
        [RegularExpression("^(Whole|Skim|Oat|Almond|Soy|None)$", ErrorMessage = "Invalid milk type.")]
        public required string MilkType { get; set; }

        [Range(0, 10)]
        public int SugarCount { get; set; }

        public bool ExtraShot { get; set; }

        [RegularExpression("^(Hot|Iced)$", ErrorMessage = "Temperature must be Hot or Iced.")]
        public required string Temperature { get; set; }

        public required string Notes { get; set; }
    }
}