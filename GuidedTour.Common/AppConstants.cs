namespace GuidedTour.Common
{
    public class AppConstants
    {
        public const string EmailRegex = "^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@"
                                         + "[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$";
        public const string Base64Regex = @"^[a-zA-Z0-9\+/]*={0,3}$";
    }
}
