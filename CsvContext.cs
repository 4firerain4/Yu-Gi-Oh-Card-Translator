namespace Yu_Gi_Oh_Card_Translator
{
    public class CsvContext
    {
        public static readonly string CacheDir = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string DataDir = CacheDir + @"Cache\Data"; //Путь к папке Cache в директории программы

        public static bool CheckCache()
        {

            if (!Directory.Exists(DataDir))
            {
                Directory.CreateDirectory(DataDir);
                return false;
            }

            string[] dirFiles = Directory.GetFileSystemEntries(DataDir);

            if (dirFiles.Length == 0)
            {
                return false;
            }

            return true;
        }

        public static async Task<List<Card>> ReadDataAsync()
        {
            var lines = File.ReadAllLines(DataDir + "\\data.csv");
            var cards = lines.Select(line =>
            {
                var columns = line.Split('\t').Select(column => column.Trim('\"').Trim().Replace('^','\n').Replace('ё','е')).ToArray();
                return new Card
                {
                    Code = columns[0],
                    Name = columns[1],
                    TranslatedName = columns[2],
                    Type = columns[3],
                    Typing = columns[4],
                    TranslatedTyping = columns[5],
                    Text = columns[6],
                    TranslatedText = columns[7]
                };
            }).ToList();
            await Task.CompletedTask;
            return cards;
        }
        public static void WriteData(List<Card> data)
        {
            var query = data.Select(x => $"{x.Code} \t {x.Name} \t {x.TranslatedName} \t {x.Type} \t {x.Typing} \t {x.TranslatedTyping} \t {x.Text} \t {x.TranslatedText}");

            File.WriteAllLines(DataDir + "\\data.csv", query);
        }
    }
}