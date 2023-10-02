using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch.Internal;
using SitoVetrina.Areas.Identity.Data;
using SitoVetrina.Context;
using System.ComponentModel;

namespace SitoVetrina.Models.Operazioni
{
    public class OperazioniUsers
    {
        public List<User> VisualizzaUsers(DapperContext context,string parametroRicerca)
        {
            FormattableString formattableQuery;
            formattableQuery = $"SELECT Id,UserName FROM AspNetUsers WHERE UserName LIKE CONCAT('%',@ParametroRicerca,'%');";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "ParametroRicerca", parametroRicerca } };

            string query = formattableQuery.ToString();

            using (var connection = context.CreateConnection())
            {
                IEnumerable<User> users = connection.Query<User>(query, parameters);
                return users.ToList();
            }
        }
    }
}
