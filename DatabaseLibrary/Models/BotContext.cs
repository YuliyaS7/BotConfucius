using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Models;

public class BotContext: DbContext
{
    public BotContext() : base() { }

    public BotContext(DbContextOptions<BotContext> options) : base(options) {}

    public DbSet<Quote> Quotes { get; set; } = null!;
}
