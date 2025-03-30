using System.Text;

namespace Conway_s_Game_of_Life.Utility
{
    public class TokenUtils
    {
        public static Credentials ExtractFromBasic(String authorization)
        {
            if (!authorization.StartsWith("basic ", StringComparison.InvariantCultureIgnoreCase))
                throw new Exception("Requires basic authorization");
            authorization = authorization.Replace("basic ", "", StringComparison.InvariantCultureIgnoreCase);
            var bytes = Convert.FromBase64String(authorization);
            authorization = Encoding.UTF8.GetString(bytes);
            String[] basicSplit = authorization.Split(":");
            return new Credentials(basicSplit[0], basicSplit[1]);
        }
    }
}
