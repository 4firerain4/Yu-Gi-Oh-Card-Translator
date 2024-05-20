using GlobalUsings;

namespace Yu_Gi_Oh_Card_Translator
{
    class Program
    {

        static async Task Main(string[] args)
        {
            string CacheDir = AppDomain.CurrentDomain.BaseDirectory;
            Console.ReadKey();
            var a = new StreamReader(@"D:\WORK\Yu-Gi-Oh-Card-Translator\my deck 1.ydk");
            string[] unsortedCodes = a.ReadToEnd().Split("\r\n");
            var codes = unsortedCodes.Where(p => int.TryParse(p, out _)).ToArray();

            List<Card> cards = new();

            if (CsvContext.CheckCache()) cards = await CsvContext.ReadDataAsync();
            else
            {
                cards = await ParseData.ParseTextAsync(codes);
                cards = await Translator.TranslateTextAsync(cards);
                //await DownloadImages.DownloadImagesAsync(codes);
                CsvContext.WriteData(cards);
                cards = await CsvContext.ReadDataAsync();
            }

            cards = cards.GroupBy(x => x.Code).Select(group => group.First()).ToList();
            //await ImageManipilator.ClearText(cards);
            await ImageManipilator.PrepareText(cards);
            Console.ReadKey();
        }

    }
}
