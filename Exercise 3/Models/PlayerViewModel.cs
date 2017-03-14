using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoApiWeb.Models
{
    public class PlayerViewModel
    {
        public string ChannelId { get; set; }
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string DisplayFormUrl { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}