namespace Ex_api_DTO.Autentication
{
    public class Users
    {
        private static List<User> _users = new List<User>
        {
            new()
            {
                UserName = "Elia",
                Password = "password",
                Role = "admin",
            },

            new()
            {
                UserName = "Mario",
                Password = "password",
                Role = "user",
            }
        };

        static public User GetUser(string Username)
        {
            return _users.FirstOrDefault(ut => ut.UserName == Username);
        }
    }
}
