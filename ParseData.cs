using GlobalUsings;
using HtmlAgilityPack;

namespace Yu_Gi_Oh_Card_Translator
{
    public class ParseData
    {
        public static async Task<List<Card>> ParseTextAsync(string[] codes)
        {
            List<Card> cards = new();
            int s = 0;
            foreach (var code in codes)
            {
                var link = await LoadPage(code);
                var document = new HtmlDocument();
                document.LoadHtml(link);
                HtmlNodeCollection name = document.DocumentNode.SelectNodes("/html/body/main/div/div[1]/div[2]");
                var block = name[0].ChildNodes;
                if (block[9].ChildNodes[0].LastChild.InnerText.Contains("Monster"))
                {
                    cards.Add(new Card
                    {
                        Code = code,
                        Name = FixData(block[1].InnerText),
                        Type = block[9].ChildNodes[0].LastChild.InnerText,
                        Typing = FixData(block[9].ChildNodes[2].LastChild.InnerText),
                        Text = FixData(block[13].InnerText)
                    });
                }
                else
                {
                    cards.Add(new Card
                    {
                        Code = code,
                        Name = FixData(block[1].InnerText),
                        Type = block[9].ChildNodes[0].LastChild.InnerText,
                        Typing = FixData(block[9].ChildNodes[1].LastChild.InnerText),
                        Text = FixData(block[13].InnerText)
                    });
                }

                s += 1;
                Console.WriteLine(s);
            }



            return cards;
        }
        private static async Task<string> LoadPage(string code)
        {
            string htmlContent = string.Empty;

            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync($"https://ygoprodeck.com/card/?search={code}");

            if (response.IsSuccessStatusCode)
                htmlContent = await response.Content.ReadAsStringAsync();

            return htmlContent;
        }
        
        private static string FixData(string text) => text = text.Replace("&quot;", "\"").Replace("&#039;", "'").Replace("\n", "^");
        

    }
}