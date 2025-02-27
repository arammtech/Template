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
        //public void SetInSession<T>(string sessionName, T value)
        //{
        //    if (typeof(T) == typeof(int))
        //    {
        //        _httpContextAccessor.HttpContext?.Session.SetInt32(sessionName, (int)(object)value);
        //    }
        //    else
        //    {
        //        _httpContextAccessor.HttpContext?.Session.SetString(sessionName, (string)(object)value);
        //    }
        //}

        //public T GetFromSession<T>(string sessionName)
        //{
        //    if (typeof(T) == typeof(int))
        //    {
        //        var value = _httpContextAccessor.HttpContext?.Session.GetInt32(sessionName) ?? 0;
        //        return (T)(object)value;
        //    }
        //    else
        //    {
        //        var value = _httpContextAccessor.HttpContext?.Session.GetString(sessionName) ?? "";
        //        return (T)(object)value;
        //    }
        //}

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
