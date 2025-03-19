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
        private object _RequestBody(string PromptText, IList<string>? images = null)
        {
            var parts = new List<object>
            {
                new { text = PromptText }
            };

            if (images != null)
            {
                parts.AddRange(images.Select(img => new
                {
                    inline_data = new
                    {
                        mime_type = "image/jpeg",
                        data = img 
                    }
                }));
            }

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = parts.ToArray() }
                },
                generationConfig = new
                {
                    maxOutputTokens = 8192,
                    temperature = 0,
                    topP = 0.2,
                    topK = 40
                }
            };

            return requestBody;
        }
        private StringContent _PrepareRequestBody(string PromptText, IList<string>? images = null)
        {

            var requestBody = _RequestBody(PromptText, images);
            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return content;
        }
        private HttpResponseMessage _SendToGemini(string PromptText, IList<string>? images = null)
        {
            var content = _PrepareRequestBody(PromptText, images);
            var response = _httpClient.PostAsync(_url, content).Result;
            response.EnsureSuccessStatusCode();
            return response;
        }
        public async Task<string?> TextGenerate(string PromptText)
        {
            var response = _SendToGemini(PromptText);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        public async Task<string?> TextGenerate(string PromptText, IList<string>? images = null)
        {
            var response = _SendToGemini(PromptText, images);
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic? result = JsonConvert.DeserializeObject(responseBody);
            if (!result)
                throw new Exception("There is no result");
            return result?.candidates?[0]?.content?.parts?[0]?.text;
        }
    }
}
