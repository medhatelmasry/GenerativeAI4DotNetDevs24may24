using System.Configuration;
using Azure.AI.OpenAI;

string _endpoint = ConfigurationManager.AppSettings["endpoint"]!;
string _apiKey = ConfigurationManager.AppSettings["api-key"]!;
string _gptDeployment = ConfigurationManager.AppSettings["gpt-deployment"]!;

OpenAIClient client = new OpenAIClient(new Uri(_endpoint), new Azure.AzureKeyCredential(_apiKey));

string prompt = "What are the top three items of the most recent .NET release?";

ChatCompletionsOptions options = new ChatCompletionsOptions(_gptDeployment, [
//     new ChatRequestSystemMessage("""
//   You are an assistant that helps people with learning .NET.
//   The most recent version of .NET is 8 which was released in November 2023.
//   The main features and improvements of .NET 8 are focused on performance, cloud-native applications, AI integration, Blazor, .NET MAUI, C# 12, and more.
// """),
    new ChatRequestUserMessage(prompt)
]);


var choices
    = (await client.GetChatCompletionsStreamingAsync(options)).EnumerateValues();

await foreach(var item in choices) {
  if (!string.IsNullOrEmpty(item.ContentUpdate))
    Console.Write(item.ContentUpdate);

  await Task.Delay(30);
}
