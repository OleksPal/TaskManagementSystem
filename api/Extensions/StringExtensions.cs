using System.Text.RegularExpressions;

namespace TaskManagementSystem.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidEmail(this string input)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (string.IsNullOrEmpty(input))
                return false;

            Regex regex = new Regex(emailPattern);
            return regex.IsMatch(input);
        }
    }
}
