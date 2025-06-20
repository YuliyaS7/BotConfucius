// Написати код, який считує з директорії Data всі файли з розширенням .txt,
// зчитує дані з кожного файлу по рядкам
// перевіряє, якщо текст вже є в базі даних, пропускає, якщо немає, записує в базу даних
using DatabaseLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();


        string directoryPath = "Data";

        var contextOptions = new DbContextOptionsBuilder<BotContext>()
        .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        .Options;

        using var db = new BotContext(contextOptions);
        db.Database.EnsureCreated();

        string[] files = Directory.GetFiles(directoryPath, "*.txt");
        foreach (var file in files)
        {
            Console.WriteLine($"Обробка файлу: {Path.GetFileName(file)}");
            foreach (var line in File.ReadLines(file, Encoding.UTF8))
            {
                string trimmed = line.Trim();
                Console.WriteLine($"Обробка рядка: {trimmed}");
                if (string.IsNullOrWhiteSpace(trimmed)) continue;
                bool exists = db.Quotes.Any(l => l.Text == trimmed);
                if (!exists)
                {
                    db.Quotes.Add(new Quote { Text = trimmed });
                    Console.WriteLine($"Додано: {trimmed}");
                }
                else
                {
                    Console.WriteLine($"Пропущено: {trimmed}");
                }
            }
            db.SaveChanges();
        }
    }
}