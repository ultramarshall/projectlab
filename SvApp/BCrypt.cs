using static BCrypt.Net.BCrypt;
namespace SvApp
{
    public class BCrypt
    {
        public static string GetRandomSalt ()
        {
            return GenerateSalt( 12 );
        }

        public static string Hash (string password)
        {
            return HashPassword( password, GetRandomSalt( ) );
        }

        public static bool ValidatePassword (string password, string correctHash)
        {
            return Verify( password, correctHash );
        }
    }
}
