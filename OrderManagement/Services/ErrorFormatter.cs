namespace OrderManagement.Services
{
    public class ErrorFormatter
    {
        private static int _errorCounter = 1;
        
        public static string FormatError(string errorCategory, string errorDetail)
        {
            string formattedError = $"{_errorCounter} [{errorCategory}] {errorDetail}";
            _errorCounter++;
            return formattedError;
        }
    }
}