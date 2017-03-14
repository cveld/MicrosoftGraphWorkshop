using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace VideoApiWeb.Models
{
    public class Group
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Visibility { get; set; }
        public string Mail { get; set; }
        public string ConversationsUrl
        {
            get
            {
                return $"https://outlook.office365.com/owa/#path=/group/{Mail}/mail";
            }
        }
        public string CalendarUrl
        {
            get
            {
                return $"https://outlook.office365.com/owa/#path=/group/{Mail}/calendar";
            }
        }    
    }
}