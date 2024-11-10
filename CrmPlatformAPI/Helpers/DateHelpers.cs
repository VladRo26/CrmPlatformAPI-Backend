using System.Runtime.CompilerServices;

namespace CrmPlatformAPI.Helpers
{
    public static class DateHelpers
    {
        public static int CalculateAge(this DateOnly doh)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var age = today.Year - doh.Year;

            if (doh.AddYears(age) > today)
                age--;
            return age;     
        }
    }
}
