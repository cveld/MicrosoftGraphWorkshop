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
                var channels = await repo.GetChannels(false);

                return View(channels);
            }
            catch (Exception ex)
            {                        
                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }        
        }
    }
}