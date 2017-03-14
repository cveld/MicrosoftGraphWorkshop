using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VideoApiWeb.Models;
using VideoApiWeb.Utils;

namespace VideoApiWeb.Controllers
{
    public class GroupsController : Controller
    {
        // GET: Groups
        public async Task<ActionResult> Index()
        {
            var accessTokenMSGraph = await AadHelper.GetAccessTokenForMicrosoftGraph();
            var repoMSGraph = new GroupsRepository(accessTokenMSGraph);
            try
            {
                List<Group> groups = repoMSGraph.GetGroups();
                return View(groups);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }
        }
    }
}