namespace RestaurantApp.GoogleAuth
{
    public interface IGoogleAuthService
    {
        // Define the methods for authentication, getting user info, and logout
        public Task<GoogleUserDTO> AuthenticateAsync();
        public Task<GoogleUserDTO> GetCurrentUserAsync();
        public Task LogoutAsync();
    }
}
