using System.Net;

namespace Conway_s_Game_of_Life.Utility
{
    public class JwtConfiguration
    {
        public const string ReadScope = "read";
        public const string WriteScope = "write";
        public static readonly List<string> AllowedScopes = new List<string> { ReadScope, WriteScope };
        public static readonly List<Credentials> AllowedClients = new List<Credentials> { new Credentials("fea3a1c5-1162-4744-aeac-b0dce6fbf49f", "e5778c30-df7e-487c-b61d-725a78649fc6") };

        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationInSeconds { get; set; }
    }
}
