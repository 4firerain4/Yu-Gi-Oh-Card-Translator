using GlobalUsings;
using System.Diagnostics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using Microsoft.VisualBasic;


namespace Yu_Gi_Oh_Card_Translator
{
    public class ImageManipilator
    {
        public static readonly string CacheDir = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string DataDir = CacheDir + @"Cache\SavedImages"; //Путь к папке Cache в директории программы
        static string fontNameTypePath1 = AppDomain.CurrentDomain.BaseDirectory + @"\Fonts\Anticva.ttf";
        static string fontNameTypePath2 = AppDomain.CurrentDomain.BaseDirectory + @"\Fonts\Anticva1.ttf";
        static string fontNameTypePath3 = AppDomain.CurrentDomain.BaseDirectory + @"\Fonts\Anticva2.ttf";
        static string fontDescPath = AppDomain.CurrentDomain.BaseDirectory + @"Fonts\Italic1.ttf";

        public static async Task DrawText(List<Card> cards)
        {
            Dictionary<string, string> spellType = new Dictionary<string, string>
            {
                {"Continuous","[Непрерывный      ]"},
                {"Counter","[Блокирующий      ]"},
                {"Quick-Play","[Быстрый      ]"},
                {"Field","[Поле      ]"},
                {"Equip","[Экипировка      ]"},
                {"Normal","[Обычный]"}
            };

            Dictionary<string, string> monsterType = new()
            {
                {"Effect","Эфект"},
                {"Flip","Переворот"},
                {"Fusion","Слияние"},
                {"Normal","Обычный"},
            };

            foreach (var card in cards)
            {
                Color color = Color.Black;
                var img = await Image.LoadAsync(@$"{DataDir}\{card.Code}.jpg");

                RichTextOptions[] options;

                if (card.Type.Contains("Monster"))
                {
                    options = CreateOptions(card, (new PointF(60, 895), new PointF(60, 922)));
                }
                else
                {
                    options = CreateOptions(card, (new PointF(732, 158), new PointF(60, 895)));
                };

                string typeText;
                if (card.Type.Contains("Spell") || card.Type.Contains("Trap"))
                {
                    color = Color.White;
                    typeText = spellType[card.Typing];
                    options[1].HorizontalAlignment = HorizontalAlignment.Right;
                    //options[1].Dpi = 48;
                }
                else
                {
                    typeText = @$"[{card.TranslatedTyping}/{monsterType[card.Type.Split()[0]]}]";
                    if (card.TranslatedText.Length > 299)
                    {
                        options[2].LineSpacing = 0.92f;
                    }
                };

                img.Mutate(x =>
                {
                    x.DrawText(options[0], card.TranslatedName, color);
                    x.DrawText(options[1], typeText, Color.Black);
                    x.DrawText(options[2], card.TranslatedText, Color.Black);
                });
                await img.SaveAsJpegAsync(@$"{CacheDir}\Cards\{card.Code}.jpg");
            }
        }

        private static RichTextOptions[] CreateOptions(Card card, (PointF type, PointF description) cordinates)
        {
            // FontFamily fonts = new();
            FontCollection collection = new();
            FontCollection collection2 = new();
            FontFamily familyName;
            FontFamily familyType = collection2.Add(fontNameTypePath1);
            FontFamily familyDesk = collection.Add(fontDescPath);

            if (card.TranslatedName.Length >= 26) familyName = collection.Add(fontNameTypePath3);
            else if ( card.TranslatedName.Length > 20) familyName = collection.Add(fontNameTypePath2);
            else familyName = collection.Add(fontNameTypePath1);
            RichTextOptions[] options = new RichTextOptions[3];

            //float fontNameScale = card.TranslatedName.Length < 21 ? 52f : card.TranslatedName.Length >= 24 ? 42f : 47f;

            Font fontName = familyName.CreateFont(52f);
            Font fontType;
            if (card.Type.Contains("Monster")) fontType = familyType.CreateFont(26);
            else fontType = familyType.CreateFont(32);
            Font fontDesk = familyDesk.CreateFont(24f, FontStyle.Italic);

            options[0] = new(fontName)
            {

                Origin = new PointF(60, 75), // Расположение текста
                WrappingLength = 685, // Длинна строки
                HorizontalAlignment = HorizontalAlignment.Left, // Выравнивание по сторонам
            };
            
            options[1] = new(fontType)
            {
                Origin = cordinates.type,// Расположение текста
                WrappingLength = 685, // Длинна строки
                HorizontalAlignment = HorizontalAlignment.Left, // Выравнивание по сторонам
            };

            options[2] = new(fontDesk)
            {
                Origin = cordinates.description, // Расположение текста
                WrappingLength = 692, // Длинна строки
                HorizontalAlignment = HorizontalAlignment.Left, // Выравнивание по сторонам
                TextJustification = TextJustification.InterWord, // Выравнивание по ширине
                VerticalAlignment = VerticalAlignment.Top
            };
            return options;
        }

        public static async Task ClearText(List<Card> cards)
        {
            var monsters = cards.Where(x => x.Type.Contains("Monster")).Select(x => x.Code).ToArray();

            var spells = cards.Where(x => x.Typing.Contains("Normal")).Select(x => x.Code).ToArray();

            var dopSpells = cards.Where(x => x.Typing.Contains("Continuous")
                                            || x.Typing.Contains("Quick-Play")
                                            || x.Typing.Contains("Counter")
                                            || x.Typing.Contains("Equip")
                                            || x.Typing.Contains("Field"))
                                .Select(x => x.Code)
                                .ToArray();

            foreach (var item in monsters)
            {
                var a = Process.Start(@"D:\WORK\Yu-Gi-Oh-Card-Translator\bin\Debug\net8.0\PhotoshopDroplets\HlamidiozMonster.exe", Path.Combine(DataDir, item + ".jpg"));
                await Task.Delay(3000);
            }
            await Task.Delay(5000);
            foreach (var item in spells)
            {
                var a = Process.Start(@"D:\WORK\Yu-Gi-Oh-Card-Translator\bin\Debug\net8.0\PhotoshopDroplets\HlamidiozSpell.exe", Path.Combine(DataDir, item + ".jpg"));
                await Task.Delay(3000);
            }
            await Task.Delay(5000);
            foreach (var item in dopSpells)
            {
                var a = Process.Start(@"D:\WORK\Yu-Gi-Oh-Card-Translator\bin\Debug\net8.0\PhotoshopDroplets\HlamidiozSuper.exe", Path.Combine(DataDir, item + ".jpg"));
                await Task.Delay(3000);
            }

            await Task.Delay(5000);

            Process[] localByName = Process.GetProcessesByName("Photoshop");
            foreach (Process p in localByName)
            {
                p.Kill();
            }
        }

    }
}