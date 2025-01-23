namespace HostelFinder.Domain.Common.Constants
{
    public static class RegexConstants
    {
        public const string PASSWORD = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
    }
}
