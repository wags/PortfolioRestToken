using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
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

    class Attributes
    {
        public List<string> Filename { get; set; }
        public List<string> Keywords { get; set; }
    }

    class Asset
    {
        public int Id { get; set; }
        public Attributes Attributes { get; set; }
    }

    class Response
    {
        public int TotalNumberOfAssets { get; set; }
        public List<Asset> Assets { get; set; }
    }

    class Program
    {
        // Use a single instance of HttpClient
        private static HttpClient Client = new HttpClient();

        public static string TargetCatalogId { get; private set; }
        public static int TotalNumberOfAssets { get; private set; }
        public static int RandomId { get; private set; }
        public static string RandomFilename { get; private set; }
        public static string RandomMetadata { get; private set; }

        static void Main(string[] args)
        {
            GetCatalogsAsync().Wait();
            FindNumberOfAssetsAsync().Wait();
            FindRandomIdAsync().Wait();
            GetAssetAsync().Wait();
            LogoutAsync().Wait();

            Console.ReadLine();
        }

        private static async Task GetCatalogsAsync()
        {
            try
            {
                var catalogUrl = $"{Constants.serverProtocol}://{Constants.serverAddress}:{Constants.serverPort}/api/v1/catalog?session={Constants.apiToken}";
                var response = await Client.GetAsync(catalogUrl);
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

        private static async Task FindNumberOfAssetsAsync()
        {
            var findNumberUrl = $"{Constants.serverProtocol}://{Constants.serverAddress}:{Constants.serverPort}/api/v1/catalog/{TargetCatalogId}/asset/?session={Constants.apiToken}";
            var findNumberData = "{\"fields\":[\"Item ID\",\"Filename\"],\"pageSize\":1,\"startingIndex\":0,\"sortOptions\":{\"field\":\"Cataloged\",\"order\":\"desc\"},}";

            try
            {
                var requestMessage = new HttpRequestMessage()
                {
                    RequestUri = new Uri(findNumberUrl),
                    Content = new StringContent(findNumberData, UnicodeEncoding.UTF8, "application/json")
                };
                var response = await Client.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                var stringResponse = response.Content.ReadAsStringAsync().Result;
                var assets = JsonConvert.DeserializeObject<Response>(stringResponse);

                if (assets.TotalNumberOfAssets == 0)
                {
                    Console.WriteLine($"[ Find Number of Assets ][ ERROR ! ] No Assets Available!");
                }
                else
                {
                    Console.WriteLine($"[ Assets Output ] {stringResponse}");
                    Console.WriteLine($"[ Total Number of Assets ] = {assets.TotalNumberOfAssets}");
                    TotalNumberOfAssets = assets.TotalNumberOfAssets;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"[ Find Number of Assets ][ ERROR ! ] {e.Message}");
            }
        }

        private static async Task FindRandomIdAsync()
        {
            var findIdUrl = $"{Constants.serverProtocol}://{Constants.serverAddress}:{Constants.serverPort}/api/v1/catalog/{TargetCatalogId}/asset/?session={Constants.apiToken}";
            int rnd = new Random().Next(0, TotalNumberOfAssets);
            var findIdData = $"{{\"fields\":[\"Item ID\",\"Filename\"],\"pageSize\":1,\"startingIndex\":{rnd},\"sortOptions\":{{\"field\":\"_id\",\"order\":\"desc\"}},}}";

            try
            {
                var requestMessage = new HttpRequestMessage()
                {
                    RequestUri = new Uri(findIdUrl),
                    Content = new StringContent(findIdData, Encoding.UTF8, "application/json")
                };
                var response = await Client.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                var stringResponse = response.Content.ReadAsStringAsync().Result;
                var assets = JsonConvert.DeserializeObject<Response>(stringResponse);

                if (assets.TotalNumberOfAssets == 0)
                {
                    Console.WriteLine($"[ Find Random ID ][ ERROR ! ] No Assets Available!");
                }
                else
                {
                    Console.WriteLine($"[ Assets Output ] {stringResponse}");
                    Console.WriteLine($"[ Random Item ID ] = {assets.Assets[0].Id}");
                    RandomId = assets.Assets[0].Id;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"[ Find Random ID ][ ERROR ! ] {e.Message}");
            }
        }

        private static async Task GetAssetAsync()
        {
            var getAssetUrl = $"{Constants.serverProtocol}://{Constants.serverAddress}:{Constants.serverPort}/api/v1/catalog/{TargetCatalogId}/asset/?session={Constants.apiToken}";
            var getAssetData = $"{{\"fields\":[\"Item ID\",\"Filename\",\"Keywords\"],\"pageSize\":10,\"startingIndex\":0,\"sortOptions\":{{\"field\":\"_id\",\"order\":\"desc\"}},\"term\":{{\"operator\":\"equalValue\",\"field\":\"_id\",\"values\":[\"{RandomId}\"]}}}}";

            try
            {
                var requestMessage = new HttpRequestMessage()
                {
                    RequestUri = new Uri(getAssetUrl),
                    Content = new StringContent(getAssetData, Encoding.UTF8, "application/json")
                };
                var response = await Client.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                var stringResponse = response.Content.ReadAsStringAsync().Result;
                var assets = JsonConvert.DeserializeObject<Response>(stringResponse);

                if (assets.TotalNumberOfAssets == 0)
                {
                    Console.WriteLine($"[ Get Asset ][ ERROR ! ] No Assets Available!");
                }
                else
                {
                    Console.WriteLine($"[ Assets Output ] {stringResponse}");
                    Console.WriteLine($"[ Random Item ID ] = {assets.Assets[0].Id}");

                    RandomFilename = assets.Assets[0].Attributes.Filename[0];
                    Console.WriteLine($"[ Random Filename ] = {RandomFilename}");

                    SaveRandomPreviewAsync().Wait();

                    RandomMetadata = string.Join(", ", assets.Assets[0].Attributes.Keywords.ToArray());
                    if (RandomMetadata != null)
                    {
                        Console.WriteLine($"[ Random Metadata ] = {RandomMetadata}");
                        SaveRandomMetadata();
                    }
                    else
                    {
                        Console.WriteLine($"[ Save Random Metadata ][ ERROR ! ] - [ No Available Keywords! ]");
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"[ Get Asset ][ ERROR ! ] {e.Message}");
            }
        }

        private static async Task SaveRandomPreviewAsync()
        {
            try
            {
                System.IO.Directory.CreateDirectory(Constants.imagesPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not create directory.");
            }

            var saveRandomUrl = $"{Constants.serverProtocol}://{Constants.serverAddress}:{Constants.serverPort}/api/v1/catalog/{TargetCatalogId}/asset/{RandomId}/preview?session={Constants.apiToken}";

            RandomFilename = Path.ChangeExtension(RandomFilename, "jpg");

            Console.WriteLine($"[ Save Random Item's Preview ][ Saving ! ][ {RandomFilename} ]");

            using (HttpResponseMessage response = await Client.GetAsync(saveRandomUrl, HttpCompletionOption.ResponseHeadersRead))
            using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
            {
                string fileToWriteTo = Path.Combine(Constants.imagesPath, RandomFilename);
                using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                {
                    await streamToReadFrom.CopyToAsync(streamToWriteTo);
                }
            }
        }

        private static void SaveRandomMetadata()
        {
            RandomFilename = Path.ChangeExtension(RandomFilename, "txt");

            Console.WriteLine($"[ Save Random Item's Metadata ][ Saving ! ][ {RandomFilename} ]");

            try
            {
                System.IO.Directory.CreateDirectory(Constants.metadataPath);
                File.WriteAllText(Path.Combine(Constants.metadataPath, RandomFilename), RandomMetadata);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ Save Random Item's Metadata ][ ERROR ! ] {e.Message}");
            }
        }

        private static async Task LogoutAsync()
        {
            var logoutUrl = $"{Constants.serverProtocol}://{Constants.serverAddress}:{Constants.serverPort}/api/v1/auth/logout?session={Constants.apiToken}";
            var logoutData = "{}";

            try
            {
                var requestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(logoutUrl),
                    Content = new StringContent(logoutData, Encoding.UTF8, "application/json")
                };
                var response = await Client.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();

                Console.WriteLine($"[ Logout ][ - Session Logout Successful - ]");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ Logout ][ ERROR ! ] {e.Message}");
            }
        }
    }
}
