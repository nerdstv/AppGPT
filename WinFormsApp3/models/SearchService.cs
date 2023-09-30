using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System;
using Nest;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text.Json;
using WinFormsApp3.models;

namespace WinFormsApp3.Models
{
    public class SearchService
    {
        private readonly string apiKey;
        private readonly HttpClient httpClient;
        private readonly BlobServiceClient blobServiceClient;
        private readonly string? provider; // Nullable string
        private readonly string containerName; // Name of the container in Azure Blob Storage
        private readonly string connectionString; // Azure Blob Storage connection string
        public string PossibleMatch { get; private set; } // Nullable string property
        public double highestSimilarity;

        public SearchService(string apiKey, string containerName, string connectionString)
        {
            this.apiKey = apiKey;
            httpClient = new HttpClient();
            this.containerName = containerName;
            this.connectionString = connectionString;

            // Create the BlobServiceClient using the connection string
            blobServiceClient = new BlobServiceClient(connectionString);

            // Get the provider from the configuration
            provider = ConfigurationManager.AppSettings["provider"];
            PossibleMatch = string.Empty;
            highestSimilarity = 0.0;
        }

        // Function to check if a file exists in the blob container
        public async Task<bool> CheckFileExistsInContainer(string fileName)
        {
            BlobContainerClient containerClient = GetBlobContainerClient();

            if (!await containerClient.ExistsAsync())
            {
                // Container does not exist, return false
                return false;
            }

            string bestMatch = null;
            highestSimilarity = 0.0;

            await foreach (BlobItem blob in containerClient.GetBlobsAsync())
            {
                string normalizedBlobName = RemovePunctuation(blob.Name).ToLowerInvariant();
                string normalizedFileName = RemovePunctuation(fileName).ToLowerInvariant();

                double similarityPercentage = Similarity.CalculateStringSimilarity(normalizedBlobName, normalizedFileName);

                if (similarityPercentage > highestSimilarity)
                {
                    bestMatch = normalizedBlobName;
                    highestSimilarity = similarityPercentage;
                }
            }

            if (highestSimilarity >= 75)
            {
                PossibleMatch = bestMatch; // Assign the best match to PossibleMatch
                return true;
            }

            return false; // No valid match found
        }


        // Retrieve text from a blob in Azure Blob Storage
        public async Task<string?> RetrieveTextFromBlob(string fileName)
        {
            // Get a reference to the blob container
            //BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobContainerClient containerClient = GetBlobContainerClient();

            // Check if the container exists
            if (!await containerClient.ExistsAsync())
            {
                // Container does not exist, return null
                return null;
            }

            // Get the blobs in the container
            await foreach (BlobItem blob in containerClient.GetBlobsAsync())
            {
                // Normalize the blob name and the provided file name by removing punctuation and converting to lowercase
                string normalizedBlobName = RemovePunctuation(blob.Name).ToLowerInvariant();
                string normalizedFileName = RemovePunctuation(fileName).ToLowerInvariant();

                // Compare the normalized names
                if (normalizedBlobName.Equals(normalizedFileName))
                {
                    BlobClient blobClient = containerClient.GetBlobClient(blob.Name);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        await blobClient.DownloadToAsync(stream);
                        stream.Position = 0;
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string text = await reader.ReadToEndAsync();
                            return text;
                        }
                    }
                }
            }

            return null; // File does not exist
        }

        // Remove punctuation from a text
        private string RemovePunctuation(string text)
        {
            // Remove punctuation from the text using regular expressions
            return Regex.Replace(text, @"\p{P}", string.Empty);
        }
        public class TopicContent
        {
            public string Topic { get; set; }
            public string Content { get; set; }
        }

        static void IndexTopicContent(ElasticClient client, string topic, string content)
        {
            var topicContent = new TopicContent
            {
                Topic = topic,
                Content = content
            };

            var indexResponse = client.IndexDocument(topicContent);

            if (indexResponse.IsValid)
            {
                Console.WriteLine("Document indexed successfully!");
            }
            else
            {
                Console.WriteLine($"Failed to index document: {indexResponse.DebugInformation}");
            }
        }

        // Store a key-value pair in Azure Blob Storage
        private async Task StoreKeyValuePairInAzureBlobStorage(string query, string generatedText)
        {

            var cloudId = "testdeploy.es.centralindia.azure.elastic-cloud.com"; // Replace with your Elastic Cloud Cluster's Cloud ID
            var username = "elastic"; // Replace with your Elasticsearch username
            var password = "nJ5lLkMmzk9b9ho1B97yaYnx"; // Replace with your Elasticsearch password

            var settings = new ConnectionSettings(new Uri($"https://{cloudId}"))
                .BasicAuthentication(username, password)
                .DefaultIndex("test");

            var client = new ElasticClient(settings);

            IndexTopicContent(client, query, generatedText);

        }

        // Perform a text search using the API
        public async Task<string?> PerformTextSearch(string query)
        {
            ChatRequest requestPayload = new ChatRequest
            {
                response_as_dict = true,
                attributes_as_list = false,
                show_original_response = false,
                temperature = 0,
                max_tokens = 250,
                providers = provider,
                text = query
            };

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestUri = new Uri("https://api.edenai.run/v2/text/chat");
            var requestContent = new StringContent(JsonConvert.SerializeObject(requestPayload), Encoding.UTF8, "application/json");

            using (var response = await httpClient.PostAsync(requestUri, requestContent))
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var jsonResponse = JsonDocument.Parse(responseBody);

                var generatedText = "";
                try
                {
                    generatedText = jsonResponse.RootElement
                        .GetProperty("openai")
                        .GetProperty("generated_text")
                        .GetString();
                }
                catch
                {
                    // Display error message or handle the error appropriately
                    return null;
                }

                // Store the key-value pair in Azure Blob Storage
                await StoreKeyValuePairInAzureBlobStorage(query, generatedText);//checkpoint 2

                return generatedText;
            }
        }

        // Save an image to Azure Blob Storage
        public async Task SaveImageToAzureStorage(Image image, string fileName)
        {
            // Create the BlobServiceClient using the connection string
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Get a reference to the container
            var containerClient = GetBlobContainerClient();

            // Convert the Image to a byte array
            byte[] imageBytes;
            using (var stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Jpeg);
                imageBytes = stream.ToArray();
            }

            // Get a reference to the blob
            var blobClient = containerClient.GetBlobClient(fileName);

            // Upload the image bytes to Azure Blob Storage
            await blobClient.UploadAsync(new MemoryStream(imageBytes));
        }

        // Retrieve an image from Azure Blob Storage
        public async Task<Image?> RetrieveImageFromBlob(string fileName)
        {
            // Get a reference to the blob container
            //BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            BlobContainerClient containerClient = GetBlobContainerClient();

            // List all blobs in the container
            var blobItems = containerClient.GetBlobs();

            // Search for the blob with a case-insensitive and punctuation-insensitive match
            var matchingBlob = blobItems.FirstOrDefault(blob => string.Equals(blob.Name, fileName, StringComparison.OrdinalIgnoreCase) ||
                                                                 string.Equals(Regex.Replace(blob.Name, @"\p{P}", ""),
                                                                               Regex.Replace(fileName, @"\p{P}", ""),
                                                                               StringComparison.OrdinalIgnoreCase));

            if (matchingBlob != null)
            {
                // Get a reference to the matching blob
                BlobClient blobClient = containerClient.GetBlobClient(matchingBlob.Name);

                // Download the blob contents
                BlobDownloadInfo downloadInfo = await blobClient.DownloadAsync();

                // Create a MemoryStream to hold the blob contents
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Copy the blob contents to the MemoryStream
                    await downloadInfo.Content.CopyToAsync(memoryStream);

                    // Reset the MemoryStream position to the beginning
                    memoryStream.Position = 0;

                    // Create an Image from the MemoryStream
                    Image image = Image.FromStream(memoryStream);

                    // Return the Image
                    return image;
                }
            }

            return null; // No matching blob found
        }

        // Perform an image search using the API
        public async Task<Image?> PerformImageSearch(string query)
        {
            ImageGenerationRequest imgPayload = new ImageGenerationRequest
            {
                response_as_dict = true,
                attributes_as_list = false,
                show_original_response = false,
                resolution = "256x256",
                num_images = 1,
                providers = provider,
                text = query
            };

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestUri = new Uri("https://api.edenai.run/v2/image/generation");
            var requestContent = new StringContent(JsonConvert.SerializeObject(imgPayload), Encoding.UTF8, "application/json");
            using (var response = await httpClient.PostAsync(requestUri, requestContent))
            {
                try
                {
                    try
                    {

                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception) { return null; }
                    var responseBody = await response.Content.ReadAsStringAsync();

                    var jsonResponse = JsonDocument.Parse(responseBody);

                    var imageUrl = "";
                    
                        imageUrl = jsonResponse.RootElement
                            .GetProperty("openai")
                            .GetProperty("items")[0]
                            .GetProperty("image_resource_url")
                            .GetString();

                        using (var imageStream = await httpClient.GetStreamAsync(imageUrl))
                        {
                            var image = Image.FromStream(imageStream);
                            var fileName = $"{query}.jpg";
                            await SaveImageToAzureStorage(image, fileName);
                            return image;
                        }
                }
                catch (KeyNotFoundException)
                {
                    // Display error message or handle the error appropriately
                    //MessageBox.Show("Enter something logical...");
                    return null;
                }
            }
        }

        private BlobContainerClient GetBlobContainerClient()
        {
            return new BlobContainerClient(connectionString, containerName);
        }

    }
}
