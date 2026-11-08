
namespace WebShop.Services
{
    public class PassOMeterService
    {
        public List<string> Verify(string password)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(password))
            {
                errors.Add("Password må ikke være tomt.");
                return errors;
            }

            if (password.Length < 8)
                errors.Add("Password skal være mindst 8 tegn langt.");

            if (!password.Any(char.IsUpper))
                errors.Add("Password skal indeholde mindst ét stort bogstav.");

            if (!password.Any(char.IsLower))
                errors.Add("Password skal indeholde mindst ét lille bogstav.");

            if (!password.Any(char.IsDigit))
                errors.Add("Password skal indeholde mindst ét tal.");

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                errors.Add("Password skal indeholde mindst ét specialtegn.");

            return errors;
        }
    }
}
