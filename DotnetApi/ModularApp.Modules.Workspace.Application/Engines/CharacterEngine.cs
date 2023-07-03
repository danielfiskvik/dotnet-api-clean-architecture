using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Modules.Workspace.Application.Interfaces;
using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Application.Engines;

public class CharacterEngine : ICharacterEngine
{
    private readonly IUnitOfWork _unitOfWork;

    public CharacterEngine(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Character>> TestCreateCharacterAsync(CancellationToken ct)
    {
        // One
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
                MeaningHint = "To remember the meaning of <kanji>One</kanji>, imagine yourself there at the scene of the crime. You grab <kanji>One</kanji> in your arms, trying to prop it up, trying to hear its last words. Instead, it just splatters some blood on your face. \"Who did this to you?\" you ask. The number One points weakly, and you see number Two running off into an alleyway. He's always been jealous of number One and knows he can be number one now that he's taken the real number one out.",
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
                ReadingHint = "Make sure you feel the ridiculously <reading>itchy</reading> sensation covering your body. It climbs from your hands, where you're holding the number <kanji>One</kanji> up, and then goes through your arms, crawls up your neck, goes down your body, and then covers everything. It becomes uncontrollable, and you're scratching everywhere, writhing on the ground. It's so itchy that it's the most painful thing you've ever experienced (you should imagine this vividly, so you remember the reading of this kanji).",
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
                MeaningHint = null,
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
                ReadingHint = null,
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
        
        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                await _unitOfWork.AddRangeAsync(characters, ct);

                await _unitOfWork.SaveChangesAsync(ct);

                await _unitOfWork.CommitTransactionAsync(ct);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
            
                // TODO What to do when exception? Make a standard way to return a Result object with fault state.

                throw;
            }
        }
        catch (Exception e)
        {
            var message = e.ToString();
            // TODO what to do if cancellation token on BeginTransactionAsync?
            
            throw new Exception(message);
        }
        
        return characters;
    }
}