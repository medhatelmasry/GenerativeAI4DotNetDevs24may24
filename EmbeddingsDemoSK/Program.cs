using System.Configuration;
using Microsoft.SemanticKernel.Connectors.Chroma;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;

string _endpoint = ConfigurationManager.AppSettings["endpoint"]!;
string _apiKey = ConfigurationManager.AppSettings["api-key"]!;
string _embedDeployName = ConfigurationManager.AppSettings["embed-deploy-name"]!;

var memoryBuilder = new MemoryBuilder();
memoryBuilder.WithAzureOpenAITextEmbeddingGeneration(_embedDeployName, _endpoint, _apiKey, "model-id");

memoryBuilder.WithMemoryStore(new ChromaMemoryStore("http://127.0.0.1:8000"));

var memory = memoryBuilder.Build();

Console.WriteLine("Add article URLs from https://news.gov.bc.ca/ and their descriptions to Chroma Semantic Memory.");
const string MEMORY_COLLECTION_NAME = "BC-Gov-News";

var onlineLinks = new Dictionary<string, string>()
{
    ["https://news.gov.bc.ca/releases/2024EMCR0024-000712"]
        = "New tools will help people prepare, stay informed during emergencies.",
    ["https://news.gov.bc.ca/releases/2024IRR0024-000734"]
        = "Lyackson First Nation, Cowichan Tribes, B.C. reach milestone agreement",
    ["https://news.gov.bc.ca/releases/2024PSSG0043-000741"]
        = "BC Coroners Service announces inquest into death of Jaime HopeBC Coroners Service announces inquest into death of Jaime Hope",
    ["https://news.gov.bc.ca/releases/2024HOUS0079-000742"]
        = "Sixty temporary homes open in Kelowna for people experiencing homelessness",
    ["https://news.gov.bc.ca/releases/2024PREM0024-000744"]
        = "Province integrating child care options into schools",
};

Console.WriteLine("******* ADDING NEW REFERENCES *******");
int i = 0;
foreach (var entry in onlineLinks) {
    await memory.SaveReferenceAsync(
        collection: MEMORY_COLLECTION_NAME,
        description: entry.Value,
        text: entry.Value,
        externalId: entry.Key,
        externalSourceName: "BC-Gov-News"
    );
    Console.WriteLine($"  URL {++i} saved");
}

string ask = "Who is Chief Pahalicktun?";

Console.WriteLine($"===========================\n Query: {ask}\n");

var memories = memory.SearchAsync(MEMORY_COLLECTION_NAME, ask, limit: 5, minRelevanceScore: 0.6);

i = 0;
await foreach (var m in memories) {
    Console.WriteLine($"Result {++i}:");
    Console.WriteLine("  URL:     : " + m.Metadata.Id);
    Console.WriteLine("  Title    : " + m.Metadata.Description);
    Console.WriteLine("  Relevance: " + m.Relevance);
    Console.WriteLine();
}
