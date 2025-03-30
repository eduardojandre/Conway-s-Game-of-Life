using System.Text.Json.Serialization;

namespace Conway_s_Game_of_Life.Utility
{
    public class Token
    {
        public Token(string TokenType, string AccessToken, int ExpiresIn)
        {
            this.TokenType = TokenType;
            this.AccessToken = AccessToken;
            this.ExpiresIn = ExpiresIn;
        }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
