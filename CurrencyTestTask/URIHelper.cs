using System;


namespace CurrencyTestTask
{
    public static class URIHelper
    {
        public static Uri UriCombine(this string hostUri, string endpoint, params object[] formatObjects)
        {
            if (string.IsNullOrEmpty(hostUri))
            {
                throw new Exception("The First part of the URI must exist");
            }

            if (endpoint.Length == 0)
            {
                return new Uri(hostUri);
            }

            hostUri = hostUri.TrimEnd('/');
            endpoint = string.Format(endpoint, formatObjects).TrimStart('/');
            return new Uri($"{hostUri}/{endpoint}");
        }
    }
}