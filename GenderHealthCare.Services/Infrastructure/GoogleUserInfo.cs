using FirebaseAdmin.Auth;

namespace GenderHealthCare.Services.Infrastructure
{
    public class GoogleUserInfo
    {
        public string Uid { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }

        public static GoogleUserInfo FromDecodedToken(FirebaseToken token)
        {
            return new GoogleUserInfo
            {
                Uid = token.Uid,
                Email = token.Claims["email"]?.ToString() ?? string.Empty,
                Name = token.Claims["name"]?.ToString() ?? string.Empty,
                Picture = token.Claims["picture"]?.ToString() ?? string.Empty
            };
        }
    }
}
