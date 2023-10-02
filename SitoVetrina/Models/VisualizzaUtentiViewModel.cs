using Microsoft.AspNetCore.Identity;
using SitoVetrina.Areas.Identity.Data;

namespace SitoVetrina.Models
{
    public class VisualizzaUtentiViewModel
    {
        public string testoRicerca { get; set; }
        public List<User> ListUsers { get; set; }
        public List<string> ListRoles { get; set; }
        public int i { get; set; }
    }
}
