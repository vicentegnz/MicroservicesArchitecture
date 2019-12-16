
using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer
{
    public class Config
    {
        private const string ClientUsername = "MicroserviceClient";
        private const string ClientPassword = "p@ssw0rd";
        private const string ClientResource1 = "Microservice1";
        private const string ClientResource2 = "Microservice2";


        // scopes define the API resources in your system
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(ClientResource1, "Microservice1"),
                new ApiResource(ClientResource2, "Microservice2")

            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = ClientUsername,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(ClientPassword.Sha256())
                    },
                    AllowedScopes = { ClientResource1, ClientResource2 }
                }
            };
        }
    }
}
