using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitializer(
        ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    /*
     * 'EnsureCreatedAsync' does **not** use migrations to create the database.
     * NB! In addition, the database that is created cannot be later updated using migrations.
     *
     * If you are targeting a relational database and using migrations 'MigrateAsync' to ensure the database is created using migrations and that all migrations have been applied.
     */
    public async Task InitialiseAsync(IConfiguration configuration)
    {
        try
        {
            if (!_context.Database.IsSqlServer())
                return;

            switch (configuration.GetValue<string>("DbInitializeMode"))
            {
                case "ResetOnEachStartupWithoutMigration":
                {
                    await _context.Database.EnsureDeletedAsync();
                    
                    var isCreated = await _context.Database.EnsureCreatedAsync();
                    if (isCreated) break;
                    
                    throw new Exception("Database not created.");
                }
                case "DoNotResetOnEachStartUpWithMigration":
                {
                    await _context.Database.MigrateAsync();
                    
                    break;
                }
                case "DoNotResetOnEachStartUpWithoutMigration":
                {
                    var isCreated = await _context.Database.EnsureCreatedAsync();
                    if (isCreated) break;
                    
                    throw new Exception("Database not created.");
                }
                default:
                {
                    var isCreated = await _context.Database.EnsureCreatedAsync();
                    if (isCreated) break;

                    throw new Exception("Database not created.");
                }
            }
            
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database");
            
            throw;
        }
    }
    
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            
            throw;
        }
    }
    
    private async Task TrySeedAsync()
    {
        // TODO - Default roles
        
        // Default users
        var characters = new List<Character>
        {
            new()
            {
                Characters = "一",
                Type = "Radical",
                Meanings = new List<Meaning>
                {
                    new()
                    {
                        Value = "Ground",
                        IsPrimary = true,
                    }
                },
                MeaningMnemonic =
                    "This radical consists of a single, horizontal stroke. What's the biggest, single, horizontal stroke? That's the ground. Look at the ground, look at this radical, now look at the ground again. Kind of the same, right?",
                // Metadata
                Level = 1,
                LessonPosition = 1,
                Source = "WaniKani",
            },
            new()
            {
                Characters = "一",
                Type = "Kanji",
                Meanings = new List<Meaning>
                {
                    new()
                    {
                        Value = "One",
                        IsPrimary = true,
                    }
                },
                MeaningMnemonic =
                    "Lying on the <radical>ground</radical> is something that looks just like the ground, the number <kanji>One</kanji>. Why is this One lying down? It's been shot by the number two. It's lying there, bleeding out and dying. The number One doesn't have long to live.",
                MeaningMnemonicHint = "To remember the meaning of <kanji>One</kanji>, imagine yourself there at the scene of the crime. You grab <kanji>One</kanji> in your arms, trying to prop it up, trying to hear its last words. Instead, it just splatters some blood on your face. \"Who did this to you?\" you ask. The number One points weakly, and you see number Two running off into an alleyway. He's always been jealous of number One and knows he can be number one now that he's taken the real number one out.",
                Readings = new List<Reading>
                {
                    new()
                    {
                        Type = "Onyomi",
                        IsPrimary = true,
                        Value = "いち"
                    },
                    new()
                    {
                        Type = "Kunyomi",
                        IsPrimary = false,
                        Value = "ひと"
                    },
                    new()
                    {
                        Type = "nanori",
                        IsPrimary = false,
                        Value = "かず"
                    }
                },
                ReadingMnemonic = "As you're sitting there next to <kanji>One</kanji>, holding him up, you start feeling a weird sensation all over your skin. From the wound comes a fine powder (obviously coming from the special bullet used to kill One) that causes the person it touches to get extremely <reading>itchy</reading> (いち)",
                ReadingMnemonicHint = "Make sure you feel the ridiculously <reading>itchy</reading> sensation covering your body. It climbs from your hands, where you're holding the number <kanji>One</kanji> up, and then goes through your arms, crawls up your neck, goes down your body, and then covers everything. It becomes uncontrollable, and you're scratching everywhere, writhing on the ground. It's so itchy that it's the most painful thing you've ever experienced (you should imagine this vividly, so you remember the reading of this kanji).",
                // Metadata
                Level = 1,
                LessonPosition = 2,
                Source = "WaniKani",
            },
            new()
            {
                Characters = "一",
                Type = "Vocabulary",
                Meanings = new List<Meaning>
                {
                    new()
                    {
                        Value = "One",
                        IsPrimary = true,
                    }
                },
                MeaningMnemonic = "As is the case with most vocab words that consist of a single kanji, this vocab word has the same meaning as the kanji it parallels, which is \u003cvocabulary\u003eone\u003c/vocabulary\u003e.",
                MeaningMnemonicHint = null,
                Readings = new List<Reading>
                {
                    new()
                    {
                        Type = null,
                        IsPrimary = true,
                        Value = "いち"
                    }
                },
                ReadingMnemonic = "When a vocab word is all alone and has no okurigana (hiragana attached to kanji) connected to it, it usually uses the kun'yomi reading. Numbers are an exception, however. When a number is all alone, with no kanji or okurigana, it is going to be the on'yomi reading, which you learned with the kanji.  Just remember this exception for alone numbers and you'll be able to read future number-related vocab to come.",
                ReadingMnemonicHint = null,
                ContextSentences = new List<ContextSentence>()
                {
                    new()
                    {
                        En = "Let’s meet up once.",
                        Ja = "一ど、あいましょう。"
                    },
                    new()
                    {
                        En = "First place was an American.",
                        Ja = "一いはアメリカ人でした。"
                    },
                    new()
                    {
                        En = "I’m the weakest man in the world.",
                        Ja = "ぼくはせかいで一ばんよわい。"
                    }
                },
                // Metadata
                Level = 1,
                LessonPosition = 3,
                Source = "WaniKani",
            }
        };

        if (!_context.Characters.Any())
        {
            await _context.Characters.AddRangeAsync(characters);

            await _context.SaveChangesAsync();
        }
    }
}