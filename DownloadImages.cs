using GlobalUsings;

namespace Yu_Gi_Oh_Card_Translator
{
    public class DownloadImages
    {
        public static readonly string CacheDir = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string DataDir = CacheDir + @"Cache\Data"; //Путь к папке Cache в директории программы

        public static async Task DownloadImagesAsync(string[] codes)
        {
            string url = "https://images.ygoprodeck.com/images/cards/";
            string savePath = CacheDir + @"\Cache\SavedImages\";
            using var _client = new HttpClient();
            foreach (string i in codes)
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url + i + ".jpg");
                using var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                File.WriteAllBytes(savePath + i.ToString() + ".jpg", await response.Content.ReadAsByteArrayAsync());
            }
        
        }
    }
}