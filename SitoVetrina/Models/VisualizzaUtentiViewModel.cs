using Microsoft.AspNetCore.Identity;
using SitoVetrina.Areas.Identity.Data;

namespace SitoVetrina.Models
{
    public class VisualizzaUtentiViewModel
    {
        public string testoRicerca { get; set; }
        public List<User> ListUsers { get; set; }
        public IList<IdentityRole> ListRoles { get; set; }
        public List<string> ListSelectedRoles { get; set; }
        public int i { get; set; }
        public VisualizzaUtentiViewModel(IList<IdentityRole> roles)
        {
            ListRoles = roles;
        }
    }
}
