using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace TestClient
{

    public class Program
    {


        private const string DISCOVERY_URL = "https://localhost:44371";
        private const string CLIENT_ID = "MicroserviceClient";
        private const string CLIENT_SECRET = "p@ssw0rd";
        static void Main(string[] args)
        {
            Console.WriteLine("Test client for Ocelot, IdentityServer4, and an internal ASP.NET Core 2.1 API");
            Console.WriteLine(Environment.NewLine);

            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            try
            { 
                DiscoveryResponse discoveryResponse = await CallIdentityServerDiscoveryEndpoint(DISCOVERY_URL);

                TokenResponse tokenMicroservice1 = await GetTokenFromIdentityServer(CLIENT_ID, CLIENT_SECRET, discoveryResponse, scope: "Microservice1");
                TokenResponse tokenMicroservice2 = await GetTokenFromIdentityServer(CLIENT_ID, CLIENT_SECRET, discoveryResponse, scope: "Microservice2");

                string resultMicroservice1 = await CallOcelotMicroservice("https://localhost:44353/api/v1/microservice1", tokenMicroservice1);
                string resultMicroservice2 =await CallOcelotMicroservice("https://localhost:44353/api/v1/microservice2", tokenMicroservice2);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(Environment.NewLine);
            }

            Console.WriteLine("Press ENTER to quit.");
            Console.ReadLine();
        }

        private static async Task<TokenResponse> GetTokenFromIdentityServer(string clientId, string clientSecret, DiscoveryResponse discoveryResponse, string scope)
        {
            Console.WriteLine($"Calling IdentityServer4 authorize endpoint to get request token from scope {scope}...");
            Console.WriteLine(Environment.NewLine);
            var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, clientId, clientSecret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync(scope);
            if (tokenResponse.IsError)
            {
                throw new Exception("Authentication failed!");
            }

            return tokenResponse;
        }

        private static async Task<DiscoveryResponse> CallIdentityServerDiscoveryEndpoint(string discoveryUrl)
        {
            Console.WriteLine("Calling IdentityServer4 discovery endpoint...");
            Console.WriteLine(Environment.NewLine);

            var discoveryClient = new DiscoveryClient(discoveryUrl);
            var discoveryResponse = await discoveryClient.GetAsync();
            if (discoveryResponse.IsError)
            {
                throw new Exception("Failed to get discovery response!");
            }

            return discoveryResponse;
        }

        private static async Task<string> CallOcelotMicroservice(string uri, TokenResponse tokenResponse)
        {
            Console.WriteLine($"Calling Ocelot endpoint with bearer token to get data from {uri}...");
            Console.WriteLine(Environment.NewLine);

            var result = string.Empty;
            var gatewayClient = new HttpClient();
            gatewayClient.SetBearerToken(tokenResponse.AccessToken);
            var response = await gatewayClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Received HTTP 200 from API. Writing widgets to console...");
                Console.WriteLine(Environment.NewLine);

                result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("Response was unsuccessful with status code: " + response.StatusCode);
            }

            return result;
        }
    }
}
