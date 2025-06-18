// See https://aka.ms/new-console-template for more information

// Написати код, який считує з директорії Data всі файли з розширенням .txt,
// зчитує дані з кожного файлу по рядкам
// перевіряє, якщо текст вже є в базі даних, пропускає, якщо немає, записує в базу даних
using DatabaseLibrary.Models;
using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string directoryPath = "Data";
        using var db = new BotContext();
        db.Database.EnsureCreated(); 
        string[] files = Directory.GetFiles(directoryPath, "*.txt");
        foreach (var file in files)
        {
            Console.WriteLine($"Обробка файлу: {Path.GetFileName(file)}");
            foreach (var line in File.ReadLines(file))
            {
                string trimmed = line.Trim();
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