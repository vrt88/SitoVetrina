namespace SitoVetrina.Models
{
    public class User
    {
        public User(string id, string userName)
        {
            this.Id = id;
            this.UserName = userName;
        }
        public User()
        {
        }
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
