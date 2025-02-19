using System.Text.Json;
using System.Text.Json.Nodes;

namespace http
{
    class Program
    {
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions{WriteIndented = true};
        static async Task Main()
        {
            using HttpClient client = new();
            var url = "https://api.nationalize.io?name=drage";
            var txtFilePath = "TxtResponse.json";
            var JsonArray = "JsonArray.json";
            // treated as txt document since unknown extension
            var JsonObject = "JsonObject.myOwnFileType";
            var jsonFilePath2 = "JsonResponse2.json";
            var jsonFilePath3 = "JsonResponse3.json";

            File.WriteAllText(txtFilePath, "wRiting to the txt file");

            JsonArray jsonArray = new(
                "hey",
                21,
                true,
                new JsonArray("Going deeper", 42, true, 
                new JsonArray("That many arrays within arrays is unstable!", 42, 
                new JsonArray("Downward Is The Only Way Forward", 212, true, false, DateTime.Now.ToString("zzz"))))
                );
            await File.WriteAllTextAsync(JsonArray, jsonArray.ToString());

            JsonObject jsonObject = new JsonObject
            {
                ["Name"] = "John Doe",
                ["Age"] = 42,
                ["NestedObject"] = new JsonObject
                {
                    ["Name"] = "Mary Jane",
                    ["Age"] = 55,
                    ["NestedArray"] = new JsonArray("Test", 1, true, DateTime.Now.ToString("Y"))
                }
            
            };
            await File.WriteAllTextAsync(JsonObject, jsonObject.ToString());
            
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                // Raw json
                await File.WriteAllTextAsync(jsonFilePath2, data);
                // All 3 prettys give same output just using different ways to achieve it. 
                var prettyJson = JsonNode.Parse(data)?.ToJsonString(jsonOptions);
                Console.WriteLine($"Pretty Json: {prettyJson}");
                var prettyJson2 = JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(data), jsonOptions);
                Console.WriteLine($"Pretty Json2: {prettyJson2}");
                var prettyJson3 = JsonSerializer.Serialize(JsonDocument.Parse(data), jsonOptions);
                Console.WriteLine($"Pretty Json3: {prettyJson3}");

                await File.WriteAllTextAsync(jsonFilePath3, prettyJson3);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
