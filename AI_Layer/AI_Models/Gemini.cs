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
        public string Prompt { get; set; } = "";
        public IList<string>? Images { get; set; } = null;
        public Gemini(string Key)
        {
            _Key = Key;
            _httpClient = new HttpClient();
            _url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_Key}";

        }
        private object _RequestBody()
        {
            var parts = new List<object>
            {
                new { text = Prompt }
            };

            if (Images != null)
            {
                parts.AddRange(Images.Select(img => new
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
                    topP = 0,
                    topK = 1
                }
            };

            return requestBody;
        }
        private StringContent _PrepareRequestBody()
        {

            var requestBody = _RequestBody();
            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return content;
        }
        private HttpResponseMessage _SendToGemini()
        {
            var content = _PrepareRequestBody();
            var response = _httpClient.PostAsync(_url, content).Result;
            response.EnsureSuccessStatusCode();
            return response;
        }
        public async Task<string?> TextGenerate(string PromptText)
        {
            this.Prompt = PromptText;
            var response = _SendToGemini();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        public async Task<string?> TextGenerate(string PromptText, IList<string>? images = null)
        {
            this.Prompt = PromptText;
            this.Images = images;
            var response = _SendToGemini();
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic? result = JsonConvert.DeserializeObject(responseBody);
            return result?.candidates?[0]?.content?.parts?[0]?.text;
        }
    }
}
