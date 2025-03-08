namespace Template.Utilities.Global
{
    public static class GlobalFunctions
    {
        // Get Resend Code Time
        public static int GetResendCodeTime(int time)
        {
            switch (time)
            {
                case 1:
                    return EmailCodeTimes.resendCodeTimeMins1;
                case 2:
                    return EmailCodeTimes.resendCodeTimeMins2;
                case 3:
                    return EmailCodeTimes.resendCodeTimeMins3;
                default:
                    return 15;
            }
        }
        // Helper method to generate a default UserName based on available details
        public static string GenerateDefaultUserName(string FirstName, string LastName)
        {
            // Example: Combine FirstName and LastName with a random number to ensure uniqueness
            return $"{FirstName}.{LastName}{new Random().Next(1000, 9999)}";
        }



        public static int GetRandom(int min, int max) 
        {
            return new Random().Next(min, max);
        }


        // Generic Filter
        public static List<T> _Filter<T>(List<T> collection, string filterProperty, string filterValue)
        {
            var propertyInfo = typeof(T).GetProperty(filterProperty);
            if (propertyInfo != null)
            {
                return collection.Where(item => propertyInfo.GetValue(item)?.ToString() == filterValue)
                    .ToList();
            }

            return collection;
        }

           
            //public static string SetOrderStatus(byte Status)
            //{
            //    switch (Status)
            //    {
            //        case 1:
            //            return GlobalSettings.StatusApproved;
            //        case 2:
            //            return GlobalSettings.StatusInProcess;
            //        case 3:
            //            return GlobalSettings.StatusShipped;
            //        case 4:
            //            return GlobalSettings.StatusCanceled;
            //        default:
            //            return "غير معروف";
            //    }

            //}

            //public static byte SetOrderStatus(string Status)
            //{
            //    switch (Status)
            //    {
            //        case GlobalSettings.StatusApproved:
            //            return 1;
            //        case GlobalSettings.StatusInProcess:
            //            return 2;
            //        case GlobalSettings.StatusShipped:
            //            return 3;
            //        case GlobalSettings.StatusCanceled:
            //            return 4;
            //        default:
            //            return 0;
            //    }

            //}
        }
}
