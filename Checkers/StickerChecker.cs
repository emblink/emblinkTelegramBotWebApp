namespace BarracudaTestBot.Checkers;

public class StickerChecker
{
    private static int currentAlertIndex = 0;
    private Dictionary<string, List<string>> _stickersByCommand = new Dictionary<string, List<string>>
    {
        ["ктоплатит"] = new List<string> { "https://sticker-collection.com/stickers/plain/vosem_let/512/7c81d17f-6aac-40fd-bba3-e283348b1c9afile_1910543.webp" },
        ["остановитесь"] = new List<string> { "https://tlgrm.ru/_/stickers/230/5c9/2305c9a3-dd7a-37b3-b38c-27e99d652dc2/2.webp" },
        ["тривога"] = new List<string>
                {
                    // "https://sticker-collection.com/stickers/plain/Povitryana_tryvoha/512/e739064c-be9d-4787-a628-8bb7939dec54file_1875815.webp",
                    // "https://sticker-collection.com/stickers/plain/Povitryana_tryvoha/512/acee37c2-7cea-49de-9ee6-b7cda696b7d3file_1875819.webp",
                    // "https://sticker-collection.com/stickers/plain/Povitryana_tryvoha/512/2bc666cc-5de7-41c9-941b-4b6f5f084e58file_1875825.webp",
                    "CAACAgIAAxkBAAEBY3xjSeEE269IAAGUAwAB65HFXDSyZV7tAALoIAACUmeJSJLkdz0x4VKBKgQ",
                    "CAACAgIAAxkBAAEBY4RjSeFQxyJpVnlEpBqQsulZ0C7j-wACwx8AAixxiEjoHSQ48whpRyoE",
                    "CAACAgIAAxkBAAEBY4xjSeGAkKBg1DYdukPboYTkgCgfBQACWBoAAg_ckUgeLguTYm4kMSoE",
                    "CAACAgIAAxkBAAEBY5RjSeHJiVSmT8dJPDYpjKkEQwotnQAC4h0AAokviUjltHCHPC78LCoE",
                },
        ["відбій"] = new List<string>
                {
                    // "https://sticker-collection.com/stickers/plain/Povitryana_tryvoha/512/d9ce940c-e68a-4ae0-b069-518634eff356file_1875818.webp",
                    // "https://sticker-collection.com/stickers/plain/Povitryana_tryvoha/512/f9d0b34c-c8db-4c5c-af66-fd90ab25f601file_1875820.webp",
                    // "https://sticker-collection.com/stickers/plain/Povitryana_tryvoha/512/f2d6625a-44e1-4736-9945-0a3450b3db55file_1875826.webp",
                    "CAACAgIAAxkBAAEBY4BjSeE1Cw88viOoFf4Mkk0Dv44o_wAC_RwAAuURiEggQmSHj7Cb4yoE",
                    "CAACAgIAAxkBAAEBY4hjSeFoCJfPgNuXN-1Dksc0vOjMkQACix8AAgwRiUjPeFGsKPjPISoE",
                    "CAACAgIAAxkBAAEBY5BjSeG2NTerE9_gd3MIuI70xmHnkwACZyEAAuegiEimsxDXD0wemyoE",
                    "CAACAgIAAxkBAAEBY5hjSeHY_3Sz8uqFeoQCcTgcQ9VrfwACdiIAAqDLiEhFekZT5nY2DSoE",
                }
    };

    public bool IsStickerCommand(string command) => _stickersByCommand.ContainsKey(command);

    public string GetStickerLink(string command)
    {
        _stickersByCommand.TryGetValue(command, out List<string> stickerLinks);
        if (command == "тривога") {
            currentAlertIndex = new Random().Next(0, 4);
            return stickerLinks[currentAlertIndex];
        } else if (command == "відбій") {
            return stickerLinks[currentAlertIndex];
        }

        var rnd = new Random();
        var stickerLink = stickerLinks.OrderBy(s => rnd.Next()).First();
        return stickerLink;
    }
}
