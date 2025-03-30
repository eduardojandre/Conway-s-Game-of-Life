namespace Conway_s_Game_of_Life.Utility
{
    public class Credentials
    {
        public Credentials(String user, String password)
        {
            this.User = user;
            this.Password = password;
        }
        public String User { get; set; }
        public String Password { get; set; }
    }
}
