// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Security;
using PowerBILab01.Options;
using Microsoft.Rest;

namespace PowerBILab01.Services
{
    public interface IAzureADAccessService
    {
        TokenCredentials GetToken();
    }
    public class AzureADAccessService : IAzureADAccessService
    {
        private readonly AzureAdOptions _azureAdOptions;

        public AzureADAccessService(IOptions<AzureAdOptions> azureAdOptions)
        {
            _azureAdOptions = azureAdOptions.Value;
        }

        public TokenCredentials GetToken()
        {
            return new TokenCredentials(GetAccessToken());
        }

        private string GetAccessToken()
        {
            AuthenticationResult authenticationResult = null;
            if (_azureAdOptions.AuthenticationMode.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase))
            {
                // Create a public client to authorize the app with the AAD app
                IPublicClientApplication clientApp = PublicClientApplicationBuilder.Create(_azureAdOptions.ClientId).WithAuthority(_azureAdOptions.AuthorityUri).Build();
                var userAccounts = clientApp.GetAccountsAsync().Result;
                try
                {
                    // Retrieve Access token from cache if available
                    authenticationResult = clientApp.AcquireTokenSilent(_azureAdOptions.Scope, userAccounts.FirstOrDefault()).ExecuteAsync().Result;
                }
                catch (MsalUiRequiredException)
                {
                    SecureString password = new SecureString();
                    foreach (var key in _azureAdOptions.PbiPassword)
                    {
                        password.AppendChar(key);
                    }
                    authenticationResult = clientApp.AcquireTokenByUsernamePassword(_azureAdOptions.Scope, _azureAdOptions.PbiUsername, password).ExecuteAsync().Result;
                }
            }

            // Service Principal auth is the recommended by Microsoft to achieve App Owns Data Power BI embedding
            else if (_azureAdOptions.AuthenticationMode.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase))
            {
                // For app only authentication, we need the specific tenant id in the authority url
                var tenantSpecificUrl = _azureAdOptions.AuthorityUri.Replace("organizations", _azureAdOptions.TenantId);

                // Create a confidential client to authorize the app with the AAD app
                IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder
                                                                                .Create(_azureAdOptions.ClientId)
                                                                                .WithClientSecret(_azureAdOptions.ClientSecret)
                                                                                .WithAuthority(tenantSpecificUrl)
                                                                                .Build();
                // Make a client call if Access token is not available in cache
                authenticationResult = clientApp.AcquireTokenForClient(_azureAdOptions.Scope).ExecuteAsync().Result;
            }

            return authenticationResult.AccessToken;
        }
    }
}