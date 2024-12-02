using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebAppRazorPages.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Catagory Name is required")]
        [MaxLength(100, ErrorMessage = "Max Lenth of the Name should be under 100 words")]
        [MinLength(3, ErrorMessage = "Min Length of the Name should be atleast 3 words")]
        [DisplayName("Catgory Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a valid Display Order")]
        [DisplayName("Display Order")]
        [Range(1, 200, ErrorMessage = "The range of order must be between 1 to 200")]
        public int DisplayOrder { get; set; }
    }
}
