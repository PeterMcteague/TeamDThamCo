using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RestockService.Clients
{
    public class ProductServiceClient : HttpClient
    {
        /// <summary>
        /// Initializes the client for sending HTTP Requests
        /// </summary>
        public ProductServiceClient()
        {
            // Get connection string
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            // Set up client
            this.BaseAddress = new Uri(configuration.GetConnectionString("ProductService") + "api/Product");
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> AddNewProduct(int BrandId, string BrandName, int CategoryId, string CategoryName, string Description, string Ean, DateTime ExpectedRestock, int Id, bool InStock, int StockNumber, string Name, decimal Price)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(JsonConvert.SerializeObject(BrandId)), "BrandId");
                formData.Add(new StringContent(JsonConvert.SerializeObject(BrandName)), "BrandName");
                formData.Add(new StringContent(JsonConvert.SerializeObject(CategoryId)), "CategoryId");
                formData.Add(new StringContent(JsonConvert.SerializeObject(CategoryName)), "CategoryName");
                formData.Add(new StringContent(JsonConvert.SerializeObject(Description)), "Description");
                formData.Add(new StringContent(JsonConvert.SerializeObject(Ean)), "Ean");
                formData.Add(new StringContent(JsonConvert.SerializeObject(ExpectedRestock)), "ExpectedRestock");
                formData.Add(new StringContent(JsonConvert.SerializeObject(Id)), "Id");
                formData.Add(new StringContent(JsonConvert.SerializeObject(InStock)), "InStock");
                formData.Add(new StringContent(JsonConvert.SerializeObject(StockNumber)), "StockNumber");
                formData.Add(new StringContent(JsonConvert.SerializeObject(Name)), "Name");
                formData.Add(new StringContent(JsonConvert.SerializeObject(Price)), "Price");

                HttpResponseMessage response = await this.PostAsync("/post/Name={Name}&BrandId={BrandId}&BrandName={BrandName}&CategoryId={CategoryId}&CategoryName={CategoryName}&Description={Description}&Ean={Ean}&ExpectedRestock={ExpectedRestock}&Id={Id}&InStock={InStock}&StockNumber={StockNumber}&Price={Price}", formData);
                response.EnsureSuccessStatusCode();

                return response;
            }
        }

        public async Task<HttpResponseMessage> UpdateProductStock(int Id, int StockNumber)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(JsonConvert.SerializeObject(Id)), "Id");
                formData.Add(new StringContent(JsonConvert.SerializeObject(StockNumber)), "StockNumber");

                HttpResponseMessage response = await this.PostAsync("put/Id={Id}&StockNumber={StockNumber}", formData);
                response.EnsureSuccessStatusCode();

                return response;
            }
        }
    }
}
