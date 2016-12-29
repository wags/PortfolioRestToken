using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PortfolioRestToken
{
    static class Constants
    {
        public const string serverProtocol = "http"; // "http" or "https"
        public const string serverAddress = "portfolio.example.com";
        public const string serverPort = "8090"; // "8090" for http / "9443" for https
        public const string apiToken = "TOKEN-";
        public const string targetCatalog = "Sandbox";
        public const string imagesPath = "Assets";
        public const string metadataPath = "Metadata";
    }

    class Catalog
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string StorageType { get; set; }
    }

    class Program
    {
        public static string TargetCatalogId { get; private set; }

        static void Main(string[] args)
        {
            GetCatalogs().Wait();
        }

        private static async Task GetCatalogs()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var catalogUrl = $"{Constants.serverProtocol}://{Constants.serverAddress}:{Constants.serverPort}/api/v1/catalog?session={Constants.apiToken}";
                    var response = await client.GetAsync(catalogUrl);
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();

                    var catalogs = JsonConvert.DeserializeObject<IEnumerable<Catalog>>(stringResponse);

                    Console.WriteLine("[ Catalogs Output ]");
                    Console.WriteLine(stringResponse);
                    Console.WriteLine("The following Catalogs are available:");
                    foreach (var catalog in catalogs)
                    {
                        Console.WriteLine($"[ {catalog.Name} ]");
                    }

                    foreach (var catalog in catalogs)
                    {
                        if (catalog.Name == Constants.targetCatalog)
                        {
                            Console.WriteLine($"[ {catalog.Name} ][ {catalog.Id} ]");
                            TargetCatalogId = catalog.Id;
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"[ Get Catalogs ][ ERROR ! ] {e.Message}");
                }
            }

            Console.ReadLine();
        }
    }
}
