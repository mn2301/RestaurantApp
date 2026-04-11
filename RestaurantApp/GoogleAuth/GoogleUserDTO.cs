namespace RestaurantApp.GoogleAuth
{
    public class GoogleUserDTO
    {
        // Data transfer object for Google user information
        public string TokenId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }   
        public string ImageURL { get; set; }
    }
}
