using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OrderFrontend.Clients
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
            client = new RestClient(configuration.GetSection("Auth0").GetValue<String>("Authority"));
            request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
        }

        public String getBasketBearer()
        {
            request.AddParameter("application/json", "{\"client_id\":\"" + _configuration.GetSection("Auth0").GetSection("Basket").GetValue<String>("ClientId").ToString() + "\",\"client_secret\":\"" + _configuration.GetSection("Auth0").GetSection("Basket").GetValue<String>("ClientSecret").ToString() + "\",\"audience\":\"" + _configuration.GetSection("Auth0").GetSection("Basket").GetValue<String>("Audience").ToString() + "\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            var response = client.Execute(request);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content.ToString());
            var value = dict.FirstOrDefault(x => x.Key == "access_token").Value.ToString();
            return "Bearer " + value;
        }

        public String getOrderBearer()
        {
            request.AddParameter("application/json", "{\"client_id\":\"" + _configuration.GetSection("Auth0").GetSection("Order").GetValue<String>("ClientId").ToString() + "\",\"client_secret\":\"" + _configuration.GetSection("Auth0").GetSection("Order").GetValue<String>("ClientSecret").ToString() + "\",\"audience\":\"" + _configuration.GetSection("Auth0").GetSection("Order").GetValue<String>("Audience").ToString() + "\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            var response = client.Execute(request);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content.ToString());
            var value = dict.FirstOrDefault(x => x.Key == "access_token").Value.ToString();
            return "Bearer " + value;
        }
    }
}
