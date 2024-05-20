using GlobalUsings;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
namespace Yu_Gi_Oh_Card_Translator
{
    public class Translator
    {
        public static async Task<List<Card>> TranslateTextAsync(List<Card> cards)
        {
            List<Card> cards1 = new();
            var client = new HttpClient();
            foreach (var card in cards)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://deep-translate1.p.rapidapi.com/language/translate/v2");
                request.Headers.Add("X-RapidAPI-Key", "604ef2880emsh1634e6c7a740845p1fa7b3jsn63dc85e705d5");
                request.Headers.Add("X-RapidAPI-Host", "deep-translate1.p.rapidapi.com");

                var json = new
                {
                    q = card.Name + " | " + card.Typing + " | " + card.Text,
                    source = "en",
                    target = "ru"
                };

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(json));
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var asd = await response.Content.ReadAsStringAsync();
                var translationData = JsonSerializer.Deserialize<RootObject>(asd);
                
                string[] result = translationData.data.translations.translatedText.Split('|');

                card.TranslatedName = (string)result[0].Trim();
                card.TranslatedTyping = (string)result[1].Trim();
                card.TranslatedText = (string)result[2].Trim();

            }


            return cards;
        }

        private static string Trim(string text) => text.Trim();


        private class Translations
        {
            public string translatedText { get; set; }
        }

        private class Data
        {
            public Translations translations { get; set; }
        }

        private class RootObject
        {
            public Data data { get; set; }
        }

    }
}
