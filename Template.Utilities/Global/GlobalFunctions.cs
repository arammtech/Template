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
                    return GlobalSettings.resendCodeTimeMins1;
                case 2:
                    return GlobalSettings.resendCodeTimeMins2;
                case 3:
                    return GlobalSettings.resendCodeTimeMins3;
                default:
                    return 15;
            }
        }

        //    private List<BookListViewModel> _FilterBooks(List<BookListViewModel> books, string filterProperty, string filterValue)
        //{
        //    int? cartQuantityFromSession = _sessionService.GetCartQuantity();

        //    var propertyInfo = typeof(BookListViewModel).GetProperty(filterProperty);
        //    if (propertyInfo != null)
        //    {
        //        return books
        //            .Where(b => propertyInfo.GetValue(b)?.ToString() == filterValue)
        //            .ToList();
        //    }

        //    return books;
        //}
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
