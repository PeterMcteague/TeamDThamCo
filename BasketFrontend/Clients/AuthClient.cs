using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BasketFrontend.Clients
{
    public class AuthClient
    {
        private RestClient client;
        private RestRequest request;
        private readonly IConfiguration _configuration;

        public AuthClient(IConfiguration configuration)
        {
            _configuration = configuration;
            // Auth
            client = new RestClient(configuration.GetSection("Auth0").GetValue<String>("Authority") + "oauth/token");
            request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"client_id\":\"" + configuration.GetSection("Auth0").GetValue<String>("ClientId") + "\",\"client_secret\":\"" + configuration.GetSection("Auth0").GetValue<String>("ClientSecret") + "\",\"audience\":\"" + configuration.GetSection("Auth0").GetValue<String>("Audience") + "\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
        }

        public String getBearer()
        {
            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content.ToString());
                var value = dict.FirstOrDefault(x => x.Key == "access_token").Value.ToString();
                return "Bearer " + value;
            }
            else
            {
                return "";
            }
        }
    }
}
