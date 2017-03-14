using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VideoApiWeb.Models;
using VideoApiWeb.Utils;

namespace VideoApiWeb.Controllers {
    public class ChannelController : Controller {
        [Authorize]
        public async Task<ActionResult> Index() {
            var accessToken = await AadHelper.GetAccessTokenForSharePoint();
            var repo = new VideoChannelRepository(accessToken);

            try
            {
                List<VideoChannel> channels = new List<VideoChannel>(); // replace this empty initialization with actual call to repo.GetChannels()

                return View(channels);
            }
            catch (Exception ex)
            {                        
                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }        
        }
    }
}