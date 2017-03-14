using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using VideoApiWeb.Models.JsonHelpers;
using VideoApiWeb.Utils;
using Newtonsoft.Json.Linq;

namespace VideoApiWeb.Models {
    public class VideoChannelRepository
    {
        private HttpClient _client = null;
        string accessToken;

        public VideoChannelRepository(string accessToken)
        {
            this.accessToken = accessToken;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        }

        public async Task<List<VideoChannel>> GetChannels()
        {
            // https://mod421769.sharepoint.com/portals/hub/_api/videoservice/Channels?$top=2000&$skip=0&$select=id,title,tilehtmlcolor&$orderby=title
            // "https://mod421769.sharepoint.com/portals/hub"
            var url = SpHelper.GetVideoPortalRootUrl();
            var query = ""; // PASTE YOUR VIDEO PORTAL QUERY HERE <--------
              

            // create request for channels
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, query);

            // issue request & get response 
            var response = await _client.SendAsync(request);
            string responseString = await response.Content.ReadAsStringAsync();
            // convert response to object
            var jsonResponse = JsonConvert.DeserializeObject<JsonHelpers.VideoChannelCollection>(responseString);

            // convert to model object
            var channels = new List<VideoChannel>();

            foreach (var videoChannel in jsonResponse.Data.Results)
            {
                var channel = new VideoChannel
                {
                    Id = videoChannel.Id,
                    HtmlColor = videoChannel.TileHtmlColor,
                    Title = videoChannel.Title,
                    Description = videoChannel.Description,
                    ServerRelativeUrl = videoChannel.ServerRelativeUrl
                };
                channels.Add(channel);
            }

            return channels.OrderBy(vc => vc.Title).ToList();
        }

        public async Task<byte[]> GetVideoThumbnail(string channelid, string videoid, string preferredwidth)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            var url = await SpHelper.GetVideoPortalRootUrl() + $"/_api/VideoService/Channels('{channelid}')/Videos('{videoid}')/ThumbnailStream('{preferredwidth}')";
            var response = await client.GetAsync(url);
            // https://techcommunity.microsoft.com/t5/Office-365-Video/Video-thumbnail-URL-broken-when-using-OAuth/td-p/7956

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //var resultString = response.Content.ReadAsStringAsync().Result;
                Stream imageStream = await response.Content.ReadAsStreamAsync();
                byte[] bytes = new byte[imageStream.Length];
                imageStream.Read(bytes, 0, (int)imageStream.Length);
                return bytes;
            }
            return null;
        }

        public async Task<string> GetEmbedUrl(string channelid, string videoid)
        {
            /*
            factory.getPlaybackUrl = function(channelId, videoId) {
			$http.defaults.useXDomain = true;
                delete $http.defaults.headers.common['X-Requested-With'];
                return $http.get(baseUrl + 'channels(guid\'' + channelId + '\')/videos(guid\'' + videoId + '\')/getPlaybackUrl(1)');
            };
            factory.getStreamingToken = function(channelId, videoId) {
			$http.defaults.useXDomain = true;
                delete $http.defaults.headers.common['X-Requested-With'];
                return $http.get(baseUrl + 'channels(guid\'' + channelId + '\')/videos(guid\'' + videoId + '\')/getStreamingKeyAccessToken');
            };
            
            <iframe ng-src="{{videoEmbedUrl}}" name="azuremediaplayer" scrolling="no" frameborder="no" align="center" height="100%" width="100%" allowfullscreen></iframe>
            
    function constructEmbedUrl() {
                var videoEmbedUrl = "//aka.ms/azuremediaplayeriframe?url=" + encodeURIComponent($scope.playbackUrl) + "&protection=aes&token=" + encodeURIComponent($scope.accessToken) + "&autoplay=false";
		$scope.videoEmbedUrl = $sce.trustAsResourceUrl(videoEmbedUrl);
            }

    	videosFactory.getStreamingToken($routeParams.channelId, $routeParams.videoId).success(function (results) {
		console.log("Playback token returned");
		$scope.accessToken = results.value;
		if ($scope.playbackUrl) {
			constructEmbedUrl();
			
		}
	});
	videosFactory.getPlaybackUrl($routeParams.channelId, $routeParams.videoId).success(function (results) {
		console.log("Playback URL returned");
		$scope.playbackUrl = results.value;
		if ($scope.accessToken) {
			constructEmbedUrl();
			
		}        
	});
    */
            return string.Empty;
        }

        public async Task<VideoChannel> GetChannel(string channelId)
        {
            var query = string.Format("{0}/_api/VideoService/Channels('{1}')", await SpHelper.GetVideoPortalRootUrl(), channelId);

            // issue request & get response 
            var response = await _client.GetAsync(query);
            string responseString = await response.Content.ReadAsStringAsync();
            // convert response to object
            var jsonResponse = JsonConvert.DeserializeObject<JsonHelpers.VideoChannelSingle>(responseString);

            var channel = new VideoChannel
            {
                Id = jsonResponse.Data.Id,
                HtmlColor = jsonResponse.Data.TileHtmlColor,
                Title = jsonResponse.Data.Title,
                Description = jsonResponse.Data.Description,
                ServerRelativeUrl = jsonResponse.Data.ServerRelativeUrl
            };

            return channel;
        }

        public async Task<Video> GetVideo(string channelId, string videoId)
        {
            // create request for video
            var query = string.Format("{0}/_api/VideoService/Channels('{1}')/Videos('{2}')",
            await SpHelper.GetVideoPortalRootUrl(), channelId, videoId);

            // issue request & get response 
            var response = await _client.GetAsync(query);
            string responseString = await response.Content.ReadAsStringAsync();
            // convert response to object
            var jsonResponse = JsonConvert.DeserializeObject<JsonHelpers.ChannelVideosSingle>(responseString);

            var channelVideo = jsonResponse.Data;            
            var video = new Video
            {
                ChannelId = channelId,
                VideoId = channelVideo.ID,
                Title = channelVideo.Title,
                DisplayFormUrl = channelVideo.DisplayFormUrl,
                DurationInSeconds = channelVideo.VideoDurationInSeconds,
                ThumbnailUrl = channelVideo.ThumbnailUrl
            };

            return video;
        }

        public async Task<List<Video>> GetChannelVideos(string channelId)
        {
            // create request for videos
            var query = ""; // PASTE YOUR VIDEO PORTAL QUERY HERE <--------
            

            // issue request & get response 
            var response = await _client.GetAsync(query);
            string responseString = await response.Content.ReadAsStringAsync();
            // convert response to object
            var jsonResponse = JsonConvert.DeserializeObject<JsonHelpers.ChannelVideosCollection>(responseString);

            // convert to model object
            var videos = new List<Video>();

            foreach (var channelVideo in jsonResponse.Data.Results)
            {                
                var video = new Video
                {
                    ChannelId = channelId,
                    VideoId = channelVideo.ID,
                    Title = channelVideo.Title,
                    DisplayFormUrl = channelVideo.DisplayFormUrl,
                    DurationInSeconds = channelVideo.VideoDurationInSeconds,
                    ViewCount = channelVideo.ViewCount,
                    ThumbnailUrl = channelVideo.ThumbnailUrl,
                    Description = channelVideo.Description
                };
                videos.Add(video);
            }

            return videos.OrderBy(v => v.Title).ToList();
        }

        public async Task UploadVideo(Video video)
        {
            var videoServiceUrl = await SpHelper.GetVideoPortalRootUrl();

            // create new video object
            var newVideo = new JsonHelpers.NewVideoPayload
            {
                Title = video.Title,
                Description = video.Description,
                FileName = video.FileName,
                Metadata = new NewVideoPayloadMetadata { Type = "SP.Publishing.VideoItem" }
            };
            var newVideoJson = JsonConvert.SerializeObject(newVideo, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // create video placeholder
            var placeholderRequestQuery = string.Format("{0}/_api/VideoService/Channels('{1}')/Videos", videoServiceUrl, video.ChannelId);
            var placeholderRequestBody = new StringContent(newVideoJson);
            placeholderRequestBody.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json;odata=verbose");

            // issue request & get response 
            var createPlaceholderResponse = await _client.PostAsync(placeholderRequestQuery, placeholderRequestBody);
            string createPlaceholderResponseString = await createPlaceholderResponse.Content.ReadAsStringAsync();
            // convert response to object
            var jsonResponse = JsonConvert.DeserializeObject<JsonHelpers.ChannelVideosSingle>(createPlaceholderResponseString);


            // upload video
            const int fileUploadChunkSize = 2 * 1024 * 1024; // upload 2MB chunks
            long fileBytesUploaded = 0;
            bool canContinue = true;
            var fileUploadSessionId = Guid.NewGuid().ToString();

            string uploadVideoEndpoint = string.Format("{0}/_api/VideoService/Channels('{1}')/Videos('{2}')/GetFile()/StartUpload(uploadId=guid'{3}')",
                                            videoServiceUrl,
                                            video.ChannelId,
                                            jsonResponse.Data.ID,
                                            fileUploadSessionId);

            using (HttpResponseMessage startResponseMessage = await _client.PostAsync(uploadVideoEndpoint, null))
            {
                canContinue = startResponseMessage.IsSuccessStatusCode;
            }

            // upload all but the last chunk
            var totalChunks = Math.Ceiling(video.FileContent.Length / (double)fileUploadChunkSize);
            while (fileBytesUploaded < fileUploadChunkSize * (totalChunks - 1))
            {
                if (!canContinue) { break; }

                // read file in
                using (var videoFileReader = new BinaryReader(new MemoryStream(video.FileContent)))
                {
                    // advance to the part of the video to show
                    videoFileReader.BaseStream.Seek(fileBytesUploaded, SeekOrigin.Begin);

                    // get a slice of the file to upload
                    var videoSlice = videoFileReader.ReadBytes(Convert.ToInt32(fileUploadChunkSize));

                    // upload slice
                    string chunkUploadUrl = string.Format("{0}/_api/VideoService/Channels('{1}')/Videos('{2}')/GetFile()/ContinueUpload(uploadId=guid'{3}',fileOffset='{4}')",
                                              videoServiceUrl,
                                              video.ChannelId,
                                              jsonResponse.Data.ID,
                                              fileUploadSessionId, fileBytesUploaded);
                    using (var fileContent = new StreamContent(new MemoryStream(videoSlice)))
                    {
                        using (HttpResponseMessage uploadResponseMessage = await _client.PostAsync(chunkUploadUrl, fileContent))
                        {
                            canContinue = uploadResponseMessage.IsSuccessStatusCode;
                            fileBytesUploaded += fileUploadChunkSize;
                        }
                    }
                }
            }

            // upload last chunk
            if (canContinue)
            {
                var lastBytesToUpload = video.FileContent.Length - fileBytesUploaded;
                using (var videoFileReader = new BinaryReader(new MemoryStream(video.FileContent)))
                {
                    // jump to the part of the file to upload
                    videoFileReader.BaseStream.Seek(fileBytesUploaded, SeekOrigin.Begin);

                    // get the last slice of file to upload
                    var videoSlice = videoFileReader.ReadBytes(Convert.ToInt32(lastBytesToUpload));
                    string chunkUploadUrl = string.Format("{0}/_api/VideoService/Channels('{1}')/Videos('{2}')/GetFile()/FinishUpload(uploadId=guid'{3}',fileOffset='{4}')",
                                              videoServiceUrl,
                                              video.ChannelId,
                                              jsonResponse.Data.ID,
                                              fileUploadSessionId, fileBytesUploaded);
                    using (var fileContent = new StreamContent(new MemoryStream(videoSlice)))
                    {
                        using (HttpResponseMessage uploadResponseMessage = await _client.PostAsync(chunkUploadUrl, fileContent))
                        {
                            canContinue = uploadResponseMessage.IsSuccessStatusCode;
                            fileBytesUploaded += fileUploadChunkSize;
                        }
                    }
                }
            }
        }

        public async Task DeleteChannelVideo(string channelId, string videoId)
        {
            var videoServiceUrl = await SpHelper.GetVideoPortalRootUrl();

            // create request for videos
            var query = string.Format("{0}/_api/VideoService/Channels('{1}')/Videos('{2}')", await SpHelper.GetVideoPortalRootUrl(), channelId, videoId);

            // set request header method
            _client.DefaultRequestHeaders.Add("X-HTTP-Method", "DELETE");

            // issue request
            await _client.PostAsync(query, null);
        }

        public async Task<List<Group>> GetGroups()
        {
           
            List<Group> groups = new List<Group>();
            // create request for videos
            var query = string.Format("https://graph.microsoft.com/v1.0/me/memberOf/$/microsoft.graph.group");
            query = string.Format("https://graph.microsoft.com/v1.0/me/");
            // issue request & get response 
            var response = await _client.GetAsync(query);
            string responseString = await response.Content.ReadAsStringAsync();


            //using (var request = new HttpRequestMessage(HttpMethod.Get, @"https://graph.microsoft.com/v1.0/me/memberOf/$/microsoft.graph.group?$filter=groupTypes/any(a:a%20eq%20'unified')"))
            //    {
            //        request.Headers.Accept.Add(Json);
            //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //        using (var response = client.SendAsync(request).Result)
            //        {
            //            if (response.StatusCode == HttpStatusCode.OK)
            //            {
            //                var json = JObject.Parse(response.Content.ReadAsStringAsync().Result);

            //                foreach (JToken user in json.SelectToken("value").Children())
            //                {
            //                    groups.Add(new GroupModel(user));
            //                }
            //            }
            //        }
                
            
            return groups;
        }

        


    }
}