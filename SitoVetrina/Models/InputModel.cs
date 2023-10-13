using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SitoVetrina.Models
{
    public class InputModel
    {
        [Required]
        [StringLength(100)]
        [DataType(DataType.Text)]
        [Display(Name = "NomeProdotto")]
        public string NomeProdotto { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Image")]
        public IFormFile Immagine { set; get; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Prezzo")]
        public string Prezzo { get; set; }
    }
}
