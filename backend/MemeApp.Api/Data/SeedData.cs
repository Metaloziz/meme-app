using MemeApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MemeApp.Api.Data;

public static class SeedData
{
    private static readonly (string Title, string Description, string FileName, int Year, int Score)[] SeedMemes =
    [
        ("Doge", "Кадзуми Сато и её собака Кабосу — классика интернет-юмора с Comic Sans.", "doge.jpg", 2013, 98),
        ("Pepe the Frog", "Зелёная лягушка из комикса Matt Furie, ставшая глобальным символом мемов.", "pepe.jpg", 2008, 97),
        ("Distracted Boyfriend", "Стоковое фото, идеально передающее момент «нового интереса».", "distracted.jpg", 2017, 95),
        ("Woman Yelling at Cat", "Скрин из Real Housewives + озадаченный кот за столом.", "cat.jpg", 2019, 94),
        ("Drake Hotline Bling", "Два кадра из клипа Drake — отказ и одобрение.", "drake.jpg", 2016, 93),
        ("Success Kid", "Малыш с кулаком на пляже — символ маленьких побед.", "success.jpg", 2007, 90),
        ("Hide the Pain Harold", "Улыбка, за которой скрывается внутренняя боль.", "harold.jpg", 2011, 89),
        ("This Is Fine", "Собака в горящей комнате — мем о принятии хаоса.", "fine.jpg", 2013, 92),
        ("Expanding Brain", "Четыре стадии «просветления» с нарастающим сиянием мозга.", "brain.jpg", 2017, 88),
        ("Stonks", "Meme Man перед графиком — ирония над финансовыми решениями.", "stonks.jpg", 2019, 87),
    ];

    public static async Task InitializeAsync(AppDbContext context, string seedImagesPath)
    {
        if (await context.Memes.AnyAsync())
        {
            return;
        }

        foreach (var seed in SeedMemes)
        {
            var (imageData, contentType) = await LoadSeedImageAsync(seedImagesPath, seed.FileName);
            context.Memes.Add(new Meme
            {
                Title = seed.Title,
                Description = seed.Description,
                ImageData = imageData,
                ImageContentType = contentType,
                Year = seed.Year,
                PopularityScore = seed.Score,
            });
        }

        await context.SaveChangesAsync();
    }

    public static async Task EnsureSeedImagesAsync(AppDbContext context, string seedImagesPath)
    {
        var memesWithoutImages = await context.Memes
            .Where(m => m.ImageData == null || m.ImageData.Length == 0)
            .ToListAsync();

        if (memesWithoutImages.Count == 0)
        {
            return;
        }

        var fileMap = SeedMemes.ToDictionary(s => s.Title, s => s.FileName);

        foreach (var meme in memesWithoutImages)
        {
            if (!fileMap.TryGetValue(meme.Title, out var fileName))
            {
                continue;
            }

            var (imageData, contentType) = await LoadSeedImageAsync(seedImagesPath, fileName);
            meme.ImageData = imageData;
            meme.ImageContentType = contentType;
            meme.ImageUrl = null;
        }

        await context.SaveChangesAsync();
    }

    private static async Task<(byte[] Data, string ContentType)> LoadSeedImageAsync(string seedImagesPath, string fileName)
    {
        var path = Path.Combine(seedImagesPath, fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Seed image not found: {path}");
        }

        var data = await File.ReadAllBytesAsync(path);
        var contentType = fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            ? "image/png"
            : "image/jpeg";

        return (data, contentType);
    }
}
