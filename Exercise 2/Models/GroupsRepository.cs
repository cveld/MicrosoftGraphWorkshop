using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace VideoApiWeb.Models
{
    public class GroupsRepository
    {
        string accessToken;

        public GroupsRepository(string accessToken)
        {
            this.accessToken = accessToken;
        }

        public List<Group> GetGroups()
        {
            MediaTypeWithQualityHeaderValue Json = new MediaTypeWithQualityHeaderValue("application/json");
            var groups = new List<Group>();

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, ""))    // <----- ADD YOUR GRAPH QUERY OVER HERE
                {
                    request.Headers.Accept.Add(Json);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


                    using (var response = client.SendAsync(request).Result)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var resultString = response.Content.ReadAsStringAsync().Result;

                            // convert response to object
                            var jsonResponse = JsonConvert.DeserializeObject<JsonHelpers.GroupCollection>(resultString);


                            foreach (var jsonGroup in jsonResponse.value)
                            {
                                var group = new Group
                                {
                                    Id = jsonGroup.id,
                                    DisplayName = jsonGroup.displayName,
                                    Description = jsonGroup.description,
                                    Visibility = jsonGroup.visibility,
                                    Mail = jsonGroup.mail
                                };
                                groups.Add(group);
                            }

                        }
                        else
                        {
                            throw new Exception("Could not retrieve groups");
                        }
                    }
                }
            }
            return groups;
        }

        public async Task<byte[]> GetGroupPhoto(string groupid)
        {
            using (var client = new HttpClient())
            {
                string url = string.Format("https://graph.microsoft.com/v1.0/groups/{0}/photo/$value", groupid);
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    using (var response = client.SendAsync(request).Result)
                    {
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
                } // using HttpRequestMessage

            } // using HttpClient
        } // GetGroupPhoto
    }
}