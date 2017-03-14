using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VideoApiWeb.Models;
using VideoApiWeb.Utils;

namespace VideoApiWeb.Controllers
{
    public class ImagesController : ApiController
    {
        [Authorize]
        [Route("images/GetGroupImage")]
        public async Task<HttpResponseMessage> GetGroupImage(string groupid)
        {
            var accessTokenMSGraph = await AadHelper.GetAccessTokenForMicrosoftGraph();
            GroupsRepository gr = new GroupsRepository(accessTokenMSGraph);

            byte[] imgData = await gr.GetGroupPhoto(groupid);
            MemoryStream ms = new MemoryStream(imgData);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            return response;
        }

        [Authorize]
        [Route("images/GetVideoThumbnail")]
        public async Task<HttpResponseMessage> GetVideoThumbnail(string channelid, string videoid, string preferredwidth)
        {
            var accessTokenSharePoint = await AadHelper.GetAccessTokenForSharePoint();
            var repoSP = new VideoChannelRepository(accessTokenSharePoint);
            byte[] imgData = await repoSP.GetVideoThumbnail(channelid, videoid, preferredwidth);
            if (imgData == null) return null;
            MemoryStream ms = new MemoryStream(imgData);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            return response;
        }
    }
}
