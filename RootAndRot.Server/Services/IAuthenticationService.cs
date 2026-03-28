using RootAndRot.Server.Data;
namespace RootAndRot.Server.Services 
{
    public interface IAuthenticationService
    {
        Task<bool> Register(string name, string password);
        Task<User?> LogIn(string name, string password);
        Task<bool> LogOut(Guid id);
        public Task<(string accessToken, string refreshToken)> IssueTokenPair(User user, string key, string issuer, string audience);
        public Task<RefreshToken?> GetRefreshToken(string token);
        public Task SaveRefreshToken(RefreshToken token);
    }
}
