using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using VideoApiWeb.Models;
using System;

namespace VideoApiWeb.Utils
{
    public class AadHelper
    {
        private static AuthenticationResult _accessTokenSP;
        private static AuthenticationResult _accessTokenMSGraph;

        public static async Task<string> GetAccessTokenForSharePoint()
        {
            if (_accessTokenSP == null || DateTime.Compare(_accessTokenSP.ExpiresOn.DateTime - TimeSpan.FromMinutes(5), DateTime.UtcNow) <= 0)
            {

                // fetch from stuff user claims
                var userObjectId = ClaimsPrincipal.Current.FindFirst(SettingsHelper.ClaimTypeObjectIdentifier).Value;

                // discover contact endpoint
                var clientCredential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);
                var userIdentifier = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);

                // create auth context
                AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority,
                  new EfAdalTokenCache(userObjectId));

                // authenticate
                var authResult =
                  await
                    authContext.AcquireTokenSilentAsync(
                      string.Format("https://{0}.sharepoint.com", SettingsHelper.Office365TenantId), clientCredential,
                      userIdentifier);

                // obtain access token
                _accessTokenSP = authResult;
            }

            return _accessTokenSP.AccessToken;
        }
        public static async Task<string> GetAccessTokenForMicrosoftGraph()
        {
            if (_accessTokenMSGraph == null || DateTime.Compare(_accessTokenMSGraph.ExpiresOn.DateTime - TimeSpan.FromMinutes(5), DateTime.UtcNow) <= 0)
            {

                // fetch from stuff user claims
                var userObjectId = ClaimsPrincipal.Current.FindFirst(SettingsHelper.ClaimTypeObjectIdentifier).Value;

                // discover contact endpoint
                var clientCredential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);
                var userIdentifier = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);

                // create auth context
                AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority,
                  new EfAdalTokenCache(userObjectId));

                // authenticate
                var authResult =
                  await
                    authContext.AcquireTokenSilentAsync(SettingsHelper.GraphUri, clientCredential, userIdentifier);

                // obtain access token
                _accessTokenMSGraph = authResult;
            }

            return _accessTokenMSGraph.AccessToken;
        }

        public static async Task<string> GetAccessTokenFromGraphRefreshTokenAsync(SubscriptionData data)
        {
            string authority = SettingsHelper.AzureADAuthorityCommon;

            AuthenticationContext authContext = new AuthenticationContext(authority, new EfAdalTokenCache(data.userObjectId));
            var clientCredential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);
            var userIdentifier = new UserIdentifier(data.userObjectId, UserIdentifierType.UniqueId);
            AuthenticationResult authResult = await authContext.AcquireTokenSilentAsync(
                SettingsHelper.GraphUri, clientCredential, userIdentifier);
            return authResult.AccessToken;
        }

    }
}