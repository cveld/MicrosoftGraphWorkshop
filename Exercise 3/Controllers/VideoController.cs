/* Test */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using VideoApiWeb.Models;
using VideoApiWeb.Utils;

namespace VideoApiWeb.Controllers {
  public class VideoController : Controller {
    [Authorize]
    public async Task<ActionResult> Index(string channelId) {
      var accessTokenSharePoint = await AadHelper.GetAccessTokenForSharePoint();
      var accessTokenMSGraph = await AadHelper.GetAccessTokenForMicrosoftGraph();

      var repoSP = new VideoChannelRepository(accessTokenSharePoint);
      var repoGroups = new GroupsRepository(accessTokenMSGraph);

        var channel = await repoSP.GetChannel(channelId);
        List<Video> videos = new List<Video>(); // replace this empty initialization with actual call to repoSP.GetChannelVideos(channelId)

            var groups = repoGroups.GetGroups();

      var viewModel = new VideoListViewModel
      {
          ChannelId = channelId,
          ChannelTitle = channel.Title,
          Videos = videos,
          Groups = groups
      };

        //// integration of the webhooks example
        //var subscriptionController = new SubscriptionController();
        //ActionResult result = await subscriptionController.CreateSubscription(this);
        //if (result is RedirectToRouteResult)
        //{
        //    // if there is an error, passthrough the redirect to the MVC kernel
        //    return result;
        //}
            

      return View(viewModel);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult> Create(string channelId) {
      var video = new Video {
        ChannelId = channelId
      };

      return View(video);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "ChannelId,Title,Description")] Video video, HttpPostedFileBase upload) {
      var accessToken = await AadHelper.GetAccessTokenForSharePoint();
      var repo = new VideoChannelRepository(accessToken);

      // if a file is uploaded, add to video & upload
      if (upload != null && upload.ContentLength > 0) {
        video.FileName = upload.FileName;
        using (var reader = new System.IO.BinaryReader(upload.InputStream)) {
          video.FileContent = reader.ReadBytes(upload.ContentLength);
        }

        await repo.UploadVideo(video);
      }

      return RedirectToRoute("ChannelVideos", new RouteValueDictionary(new { channelId = video.ChannelId, action = "Index" }));
    }

    [Authorize]
    public async Task<ActionResult> Delete(string channelId, string videoId) {
      var accessToken = await AadHelper.GetAccessTokenForSharePoint();
      var repo = new VideoChannelRepository(accessToken);

      if (channelId != null && videoId != null) {
        await repo.DeleteChannelVideo(channelId, videoId);
      }

      // if channelid provided, use this
      if (channelId != null) {
        return RedirectToRoute("ChannelVideos", new RouteValueDictionary(new { channelId = channelId, action = "Index" }));
      } else {
        return RedirectToRoute("Default", new { controller = "Channel", action = "Index" });
      }
    }
  }
}