using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Linq;

namespace BookStore.Utilties.HTMLHelpers
{
    /// <summary>
    /// Provides extension methods for the <see cref="IHtmlHelper"/> interface to help determine active navigation links.
    /// </summary>
    public static class HtmlHelpers
    {
        /// <summary>
        /// Determines whether the current route (including area, controller, and action) matches any of the specified routes.
        /// If a match is found, the method returns the string "active".
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="routes">
        /// A comma-separated list of accepted routes. Each route should be in one of the following formats:
        /// <list type="bullet">
        ///     <item>"Controller/Action" (for routes without an area)</item>
        ///     <item>"Area/Controller/Action" (for routes with an area)</item>
        /// </list>
        /// </param>
        /// <returns>
        /// Returns the string "active" if the current route matches one of the provided routes (ignoring case); otherwise, an empty string.
        /// </returns>
        /// <example>
        /// In a Razor view, you might use this helper as follows:
        /// <code>
        /// &lt;a class="nav-link @(Html.IsActive("Home/Index, Admin/Users/Index"))" href="@Url.Action("Index", "Home")"&gt;
        ///     Home
        /// &lt;/a&gt;
        /// </code>
        /// </example>
        public static string IsActive(this IHtmlHelper htmlHelper, string routes)
        {
            // Get current area, controller, and action from the route data.
            var routeData = htmlHelper.ViewContext.RouteData;
            string currentArea = routeData.Values["area"]?.ToString() ?? "";
            string currentController = routeData.Values["controller"]?.ToString() ?? "";
            string currentAction = routeData.Values["action"]?.ToString() ?? "";

            // Build the current route.
            string currentRoute = string.IsNullOrEmpty(currentArea)
                ? $"{currentController}/{currentAction}"
                : $"{currentArea}/{currentController}/{currentAction}";

            // Split the provided routes, trim whitespace, and remove empty entries.
            var acceptedRoutes = routes.Split(',')
                                       .Select(r => r.Trim())
                                       .Where(r => !string.IsNullOrEmpty(r));

            // Return "active" if the current route matches one of the accepted routes (case-insensitive).
            return acceptedRoutes.Contains(currentRoute, StringComparer.OrdinalIgnoreCase) ? "active" : "";
        }

    }
}
