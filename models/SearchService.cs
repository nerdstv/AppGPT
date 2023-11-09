using Newtonsoft.Json;
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
using System.Text.Json;
using WinFormsApp3.models;
using Serilog;
using Serilog.Core;

namespace WinFormsApp3.Models
{
    public class SearchService
    {
        private readonly string apiKey;
        private readonly HttpClient httpClient;
        private readonly string? provider; // Nullable string
        public string PossibleMatch { get; private set; } // Nullable string property
        public double highestSimilarity;
        string cloudId = "appgpt.es.centralindia.azure.elastic-cloud.com"; // Replace with your Elastic Cloud Cluster's Cloud ID
        string username = "elastic"; // Replace with your Elasticsearch username
        string password = "rTL8YPLMmyBHlCR4nq3FRz0h"; // Replace with your Elasticsearch password


        public SearchService(string apiKey)//, string containerName) //string connectionString)
        {
            this.apiKey = apiKey;
            httpClient = new HttpClient();
            provider = ConfigurationManager.AppSettings["provider"];
            PossibleMatch = string.Empty;
            highestSimilarity = 0.0;
        }

        // Retrieve text from a blob in Azure Blob Storage
        public async Task<string?> RetrieveTextFromBlob(string fileName)
        {
            try
            {
                Log.Information("Start Retrieving Text from the the DataBase");
                var settings = new ConnectionSettings(new Uri($"https://{cloudId}"))
                      .BasicAuthentication(username, password)
                      .DefaultIndex("text"); // Replace with your index name

                var client = new ElasticClient(settings);

                string topicToSearch = fileName; //DeleteSpaces(fileName); // Replace with the topic you want to search

                string foundContent = SearchTopic(client, topicToSearch);

                if (!string.IsNullOrEmpty(foundContent))
                {
                    return foundContent;
                }
                return null;
            }

            catch(Exception ex)
            {
                Log.Information(ex, "Some error occured while  retrieving Text from the the DataBase.");
                return null;
            }
           
        }

 static string SearchTopic(ElasticClient client, string topic)
    {
            try
            {
                Log.Information("Starting searching the Query, if it is present in the DB or not.");
                // 1. Search for an exact match of the topic
                var exactMatchResponse = client.Search<TopicContent>(s => s
                    .Query(q => q
                        .MatchPhrase(m => m
                            .Field(f => f.Topic)
                            .Query(topic)
                        )
                    )
                );

                if (exactMatchResponse.IsValid && exactMatchResponse.Documents.Any())
                {
                    return exactMatchResponse.Documents.First().Content; // Exact match found, return content
                }

                // 2. If not found, perform a custom similarity search
                var allTopicsResponse = client.Search<TopicContent>(s => s
                    .Size(10000) // Retrieve a large number of documents to increase the chances of finding similar topics
                );

                if (!allTopicsResponse.IsValid)
                {
                    return null; // Error occurred
                }

                string mostSimilarTopic = FindMostSimilarTopic(topic, allTopicsResponse.Documents);

                if (!string.IsNullOrEmpty(mostSimilarTopic))
                {
                    return mostSimilarTopic; // Return content of the most similar topic
                }

                return null; // Topic not found or an error occurred
            }

            catch (Exception ex)
            {
                Log.Information("Some error occured while searching in the DB.");
                return null;
            }

    }

    static string FindMostSimilarTopic(string targetTopic, IEnumerable<TopicContent> topics)
    {
        double maxSimilarity = 0;
        string mostSimilarTopic = null;

        foreach (var topic in topics)
        {
            double similarity = CalculateStringSimilarity(targetTopic, topic.Topic);
            if (similarity > maxSimilarity)
            {
                maxSimilarity = similarity;
                mostSimilarTopic = topic.Content;
            }
        }

        return mostSimilarTopic;
    }

    public static double CalculateStringSimilarity(string string1, string string2)
    {
        int maxLength = Math.Max(string1.Length, string2.Length);
        int levenshteinDistance = ComputeLevenshteinDistance(string1, string2);
        double similarityRatio = (double)(maxLength - levenshteinDistance) / maxLength * 100;

        return similarityRatio;
    }

    private static int ComputeLevenshteinDistance(string string1, string string2)
    {
        int[,] distance = new int[string1.Length + 1, string2.Length + 1];

        for (int i = 0; i <= string1.Length; i++)
            distance[i, 0] = i;

        for (int j = 0; j <= string2.Length; j++)
            distance[0, j] = j;

        for (int i = 1; i <= string1.Length; i++)
        {
            for (int j = 1; j <= string2.Length; j++)
            {
                int cost = (string2[j - 1] == string1[i - 1]) ? 0 : 1;
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }

        return distance[string1.Length, string2.Length];
    }

        public class TopicContent
        {
            public string Topic { get; set; }
            public string Content { get; set; }
            public string Image {  get; set; }
        }

        static void IndexTopicContent(ElasticClient client, string topic, string content)
        {
            var topicContent = new TopicContent
            {
                Topic = topic,
                Content = content
            };

            var indexResponse = client.IndexDocument(topicContent);

        }


        // Store a key-value pair in Azure Blob Storage
        private async Task StoreKeyValuePairInElastic(string input, string generatedText)        
       {
            try
            {
                Log.Information("Storing the Text result in the DB.");
                string query = input;// DeleteSpaces(input);
                var settings = new ConnectionSettings(new Uri($"https://{cloudId}"))
                    .BasicAuthentication(username, password)
                    .DefaultIndex("text");

                var client = new ElasticClient(settings);
                if (generatedText != "I'm sorry, but as a text-based AI, I am unable to provide or display images. However, you can easily find pictures of Lionel Messi by searching for him on any search engine or image-sharing platform.")
                {
                    IndexTopicContent(client, query, generatedText);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Information("Error in storing the text output in the DB.");
                return;
            }
     
        }

        // Perform a text search using the API
        public async Task<string?> PerformTextSearch(string query)
        {
            try
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
                        Log.Information($"{jsonResponse}");
                        generatedText = jsonResponse.RootElement
                            .GetProperty("openai")
                            .GetProperty("generated_text")
                            .GetString();
                    }
                    catch
                    {
                        // Display error message or handle the error appropriately
                        Log.Information("Error during the text generation.");
                        return null;
                    }

                    // Store the key-value pair in Azure Blob Storage
                    await StoreKeyValuePairInElastic(query, generatedText);//checkpoint 2

                    return generatedText;
                }
            }

            catch (Exception ex)
            {
                Log.Information($"Failed to store {ex.Message}");
                return null; 
            }

        }

    // Retrieve an image from Azure Elastic Storage
    public async Task<Image?> RetrieveImageFromBlob(string query)
        {
            try
            {
                Log.Information("Retrieve Image from DB.");
                var settings = new ConnectionSettings(new Uri($"https://{cloudId}"))
    .BasicAuthentication(username, password)
    .DefaultIndex("image"); // Replace with your index name

                var client = new ElasticClient(settings);

                string topicToSearch = query;// DeleteSpaces(query); // Replace with the topic you want to search

                Image foundImage = SearchImageByTopic(client, topicToSearch);

                if (foundImage != null)
                {
                    // Display or use the retrieved image
                    return foundImage;
                }
                return null;
            }

            catch(Exception ex)
            {
                Log.Information($"{ex.Message}");
                return null;
            }

        }

        static Image SearchImageByTopic(ElasticClient client, string topic)
        {
            try
            {
                Log.Information("Searching Image by topic given.");
                // 1. Search for an exact match of the topic
                var exactMatchResponse = client.Search<TopicWithImage>(s => s
                    .Query(q => q
                        .MatchPhrase(m => m
                            .Field(f => f.Topic)
                            .Query(topic)
                        )
                    )
                );

                if (exactMatchResponse.IsValid && exactMatchResponse.Documents.Any())
                {
                    // Retrieve and return the image
                    string base64Image = exactMatchResponse.Documents.First().Image;
                    return Base64ToImage(base64Image);
                }

                // 2. If not found, perform a custom similarity search
                var allTopicsResponse = client.Search<TopicWithImage>(s => s
                    .Size(10000) // Retrieve a large number of documents to increase the chances of finding similar topics
                );

                if (!allTopicsResponse.IsValid)
                {
                    return null; // Error occurred
                }

                string mostSimilarImage = FindMostSimilarImage(topic, allTopicsResponse.Documents);

                if (!string.IsNullOrEmpty(mostSimilarImage))
                {
                    return Base64ToImage(mostSimilarImage); // Return the image of the most similar topic
                }

                return null; // Topic not found or an error occurred
            }
            catch(Exception ex)
            {
                Log.Information(topic + " " + ex.Message);
                return null;
            }
        }

        static string FindMostSimilarImage(string targetTopic, IEnumerable<TopicWithImage> topics)
        {
            double maxSimilarity = 0;
            string mostSimilarImage = null;

            foreach (var topic in topics)
            {
                double similarity = CalculateStringSimilarity(targetTopic, topic.Topic);
                if (similarity > maxSimilarity)
                {
                    maxSimilarity = similarity;
                    mostSimilarImage = topic.Image;
                }
            }
            return mostSimilarImage;
        }

        static Image Base64ToImage(string base64Image)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                return Image.FromStream(ms);
            }
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
                    Log.Information(query, response);
                    try
                    {
                        Log.Information(query, response);
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {
                        Log.Information("Some error during in Searching image result on Line 436.");
                        return null;
                    }
                         
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
                            var fileName = query;
                            await SaveImageToElastic(image, fileName);
                            return image;
                        }
                }
                catch (KeyNotFoundException)
                {
                    Log.Information($"Failed to search:{query}");
                    // Display error message or handle the error appropriately
                    return null;
                }
            }
        }

        public async Task SaveImageToElastic(Image image, string input)
        {
            try
            {
                Log.Information("Saving image to the DB.");
                string fileName = input; // DeleteSpaces(input);
                var settings = new ConnectionSettings(new Uri($"https://{cloudId}"))
                    .BasicAuthentication(username, password)
                    .DefaultIndex("image");

                var client = new ElasticClient(settings);

                IndexTopicWithImage(client, fileName, image);
            }
            catch(Exception ex)
            {
                Log.Information(input + " " + ex.Message);
            }
   
        }
        static void IndexTopicWithImage(ElasticClient client, string topic, Image image)
        {
            // Convert Image to base64 string
            string base64Image = ImageToBase64(image, ImageFormat.Jpeg);

            var topicWithImage = new TopicWithImage
            {
                Topic = topic,
                Image = base64Image
            };

            var indexResponse = client.IndexDocument(topicWithImage);
        }

        static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }


        // Function to check if a file exists in the blob container
        public async Task<bool> CheckFileExistsInContainer(string fileName)
         {
            try
            {
                Log.Information("Checking if Text result present in the DB.");
                var settings = new ConnectionSettings(new Uri($"https://{cloudId}"))
          .BasicAuthentication(username, password)
          .DefaultIndex("text"); // Replace with your index name

                var client = new ElasticClient(settings);

                bool isTopicPresent = IsTopicPresent(client, fileName);// DeleteSpaces(fileName));

                if (isTopicPresent)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Information($"Failed to load {fileName}");
                return false;
            }

         }

         static bool IsTopicPresent(ElasticClient client, string topic)
         {
             // 1. Search for an exact match of the topic
             var exactMatchResponse = client.Search<TopicContent>(s => s
                 .Query(q => q
                     .MatchPhrase(m => m
                         .Field(f => f.Topic)
                         .Query(topic)
                     )
                 )
             );

             if (exactMatchResponse.IsValid && exactMatchResponse.Documents.Any())
             {
                 return true; // Exact match found
             }

             // 2. If not found, perform a custom similarity search
             var allTopicsResponse = client.Search<TopicContent>(s => s
                 .Size(10000) // Retrieve a large number of documents to increase the chances of finding similar topics
             );

             if (!allTopicsResponse.IsValid)
             {
                 return false; // Error occurred
             }

             var similarTopics = allTopicsResponse.Documents
                 .Where(doc => CalculateStringSimilarity(topic, doc.Topic) > 80) // Example similarity threshold
                 .ToList();

             return similarTopics.Any();
         }

         public async Task<bool> CheckFileExistsInContainerImage(string fileName)
          {
             var settings = new ConnectionSettings(new Uri($"https://{cloudId}"))
                       .BasicAuthentication(username, password)
                       .DefaultIndex("image"); // Replace with your index name

             var client = new ElasticClient(settings);

            bool isTopicPresent = IsTopicPresent(client, fileName);// DeleteSpaces(fileName));

             if (isTopicPresent)
             {
                 return true;
             }
             return false;
         }

        public string[] GetAllTopicsInIndexes(ElasticClient client, params string[] indexes)
        {
            var topics = new List<string>();

            foreach (string index in indexes)
            {
                var response = client.Search<TopicContent>(s => s
                    .Index(index)
                    .Size(10000) // Adjust the size as needed to retrieve all documents
                );

                if (response.IsValid)
                {
                    // Extract topics from the response
                    var topicsInIndex = response.Documents.Select(doc => doc.Topic);
                    topics.AddRange(topicsInIndex);
                }
                else
                {
                    Console.WriteLine($"Error occurred while retrieving data from index '{index}'.");
                }
            }

            return topics.ToArray();
        }

    }
    public class TopicWithImage
    {
        public string Topic { get; set; }
        public string Image { get; set; }
    }

}

