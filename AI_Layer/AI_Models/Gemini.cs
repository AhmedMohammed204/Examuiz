using AI_Layer.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace AI_Layer.AI_Models
{
    public class Gemini : IGenerativeAI
    {
        private readonly HttpClient _httpClient;
        private string _Key { get; set; }
        private string _url;
        public Gemini(string Key)
        {
            _Key = Key;
            _httpClient = new HttpClient();
            _url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_Key}";

        }

        private StringContent _PrepareRequestBody(string PromptText)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = PromptText } } }
                },
                generationConfig = new
                {
                    maxOutputTokens = 8192,
                    temperature = 0.2,
                    topP = 0.9,
                    topK = 40
                }
            };
            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return content;
        }
        private HttpResponseMessage _SendToGemini(string PromptText)
        {
            var content = _PrepareRequestBody(PromptText);
            var response = _httpClient.PostAsync(_url, content).Result;
            response.EnsureSuccessStatusCode();
            return response;
        }
        public async Task<string> TextGenerate(string PromptText)
        {   
            var response = _SendToGemini(PromptText);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<string> FileGenerate(string PromptText)
        {
            var response = _SendToGemini(PromptText);
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseBody);
            return result?.candidates?[0]?.content?.parts?[0]?.text;
        }
    }
}
