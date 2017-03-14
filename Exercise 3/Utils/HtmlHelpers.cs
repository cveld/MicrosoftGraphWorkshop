using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VideoApiWeb.Utils
{
    // source: http://stackoverflow.com/questions/20410623/how-to-add-active-class-to-html-actionlink-in-asp-net-mvc
    static public class HtmlHelpers
    {
        public static string IsActive(this HtmlHelper html,
                                      string control,
                                      string action)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            // both must match
            var returnActive = control == routeControl &&
                               action == routeAction;

            return returnActive ? "is-selected" : "";
        }

        public static string ShowHHMMSS(int seconds)
        {
            TimeSpan timespan = TimeSpan.FromSeconds(seconds);
            if (seconds >= 3600)
            {
                return timespan.ToString(@"hh\:mm\:ss");
            }
            else return timespan.ToString(@"mm\:ss");
        }
    }
}