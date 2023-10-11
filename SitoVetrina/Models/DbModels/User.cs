namespace SitoVetrina.Models.DbModels
{
    public class User
    {
        public User(string id, string userName)
        {
            Id = id;
            UserName = userName;
        }
        public User()
        {
        }
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
