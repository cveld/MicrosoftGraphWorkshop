﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using VideoApiWeb.Models;
using VideoApiWeb.SignalR;
using VideoApiWeb.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace VideoApiWeb.Controllers
/*
 *  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
 *  See LICENSE in the source repository root for complete license information.
 */
{
    public class NotificationController : Controller
    {
        public ActionResult LoadView()
        {
            return View("Notification");
        }

        // The notificationUrl endpoint that's registered with the webhook subscription.
        [HttpPost]
        public async Task<ActionResult> Listen()
        {

            // Validate the new subscription by sending the token back to Microsoft Graph.
            // This response is required for each subscription.
            if (Request.QueryString["validationToken"] != null)
            {
                var token = Request.QueryString["validationToken"];
                return Content(token, "plain/text");
            }

            // Parse the received notifications.
            else
            {
                try
                {
                    var notifications = new Dictionary<string, Notification>();
                    using (var inputStream = new System.IO.StreamReader(Request.InputStream))
                    {
                        JObject jsonObject = JObject.Parse(inputStream.ReadToEnd());
                        if (jsonObject != null)
                        {

                            // Notifications are sent in a 'value' array.
                            JArray value = JArray.Parse(jsonObject["value"].ToString());
                            foreach (var notification in value)
                            {
                                Notification current = JsonConvert.DeserializeObject<Notification>(notification.ToString());

                                // Check client state to verify the message is from Microsoft Graph. 
                                var subscriptionParams = (SubscriptionData)HttpRuntime.Cache.Get("subscriptionId_" + current.SubscriptionId);
                                if (subscriptionParams != null)
                                {
                                    if (current.ClientState == subscriptionParams.ClientState)
                                    {
                                        // Just keep the latest notification for each resource.
                                        // No point pulling data more than once.
                                        notifications[current.Resource] = current;
                                    }
                                }
                            }

                            if (notifications.Count > 0)
                            {
                                // Query for the changed messages. 
                                await GetChangedMessagesAsync(notifications.Values);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    // TODO: Handle the exception.
                    // Still return a 202 so the service doesn't resend the notification.
                }
                return new HttpStatusCodeResult(202);
            }
        }

        // Get information about the changed messages and send to browser via SignalR.
        // A production application would typically queue a background job for reliability.
        [HandleAdalException]
        public async Task GetChangedMessagesAsync(IEnumerable<Notification> notifications)
        {
            List<Message> messages = new List<Message>();
            string serviceRootUrl = "https://graph.microsoft.com/v1.0/";

            // Create the client that will send the request.
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var notification in notifications)
            {

                // Get the access token and add it to the request.
                var subscriptionParams = (SubscriptionData)HttpRuntime.Cache.Get("subscriptionId_" + notification.SubscriptionId);                
                string accessToken = await AadHelper.GetAccessTokenFromGraphRefreshTokenAsync(subscriptionParams);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, serviceRootUrl + notification.Resource);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Send the 'GET' request.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(continueOnCapturedContext: false);

                // Get the messages from the JSON response.
                if (response.IsSuccessStatusCode)
                {
                    string stringResult = await response.Content.ReadAsStringAsync();
                    var type = notification.ResourceData.ODataType;
                    if (type == "#Microsoft.Graph.Message")
                    {
                        messages.Add(JsonConvert.DeserializeObject<Message>(stringResult));
                    }
                }
            }
            if (messages.Count > 0)
            {
                NotificationService notificationService = new NotificationService();
                notificationService.SendNotificationToClient(messages);
            }
        }
    }
}
