using MemeApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MemeApp.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (await context.Memes.AnyAsync())
        {
            return;
        }

        context.Memes.AddRange(
            new Meme
            {
                Title = "Doge",
                Description = "Кадзуми Сато и её собака Кабосу — классика интернет-юмора с Comic Sans.",
                ImageUrl = "memes/doge.svg",
                Year = 2013,
                PopularityScore = 98
            },
            new Meme
            {
                Title = "Pepe the Frog",
                Description = "Зелёная лягушка из комикса Matt Furie, ставшая глобальным символом мемов.",
                ImageUrl = "memes/pepe.svg",
                Year = 2008,
                PopularityScore = 97
            },
            new Meme
            {
                Title = "Distracted Boyfriend",
                Description = "Стоковое фото, идеально передающее момент «нового интереса».",
                ImageUrl = "memes/distracted.svg",
                Year = 2017,
                PopularityScore = 95
            },
            new Meme
            {
                Title = "Woman Yelling at Cat",
                Description = "Скрин из Real Housewives + озадаченный кот за столом.",
                ImageUrl = "memes/cat.svg",
                Year = 2019,
                PopularityScore = 94
            },
            new Meme
            {
                Title = "Drake Hotline Bling",
                Description = "Два кадра из клипа Drake — отказ и одобрение.",
                ImageUrl = "memes/drake.svg",
                Year = 2016,
                PopularityScore = 93
            },
            new Meme
            {
                Title = "Success Kid",
                Description = "Малыш с кулаком на пляже — символ маленьких побед.",
                ImageUrl = "memes/success.svg",
                Year = 2007,
                PopularityScore = 90
            },
            new Meme
            {
                Title = "Hide the Pain Harold",
                Description = "Улыбка, за которой скрывается внутренняя боль.",
                ImageUrl = "memes/harold.svg",
                Year = 2011,
                PopularityScore = 89
            },
            new Meme
            {
                Title = "This Is Fine",
                Description = "Собака в горящей комнате — мем о принятии хаоса.",
                ImageUrl = "memes/fine.svg",
                Year = 2013,
                PopularityScore = 92
            },
            new Meme
            {
                Title = "Expanding Brain",
                Description = "Четыре стадии «просветления» с нарастающим сиянием мозга.",
                ImageUrl = "memes/brain.svg",
                Year = 2017,
                PopularityScore = 88
            },
            new Meme
            {
                Title = "Stonks",
                Description = "Meme Man перед графиком — ирония над финансовыми решениями.",
                ImageUrl = "memes/stonks.svg",
                Year = 2019,
                PopularityScore = 87
            }
        );

        await context.SaveChangesAsync();
    }
}
