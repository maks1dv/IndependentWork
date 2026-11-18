using System.Text;

namespace IndependentWork8;

public sealed class FileMetadata
{
    public required string FileName { get; init; }
    public long Size { get; init; }
    public DateTime LastModified { get; init; }
    public required string Hash { get; init; }

    public override string ToString() =>
        $"{FileName}: {Size} байт, змінено {LastModified:yyyy-MM-dd}, hash={Hash}";
}

public sealed class FileMetadataCache
{
    private readonly Dictionary<string, FileMetadata> _cache =
        new(StringComparer.OrdinalIgnoreCase);

    public void Add(string fileName, FileMetadata metadata) =>
        _cache.Add(fileName, metadata);

    public FileMetadata Get(string fileName)
    {
        if (_cache.TryGetValue(fileName, out var metadata))
            return metadata;

        throw new KeyNotFoundException($"Файл '{fileName}' не знайдено.");
    }

    public bool Remove(string fileName) => _cache.Remove(fileName);

    public bool Contains(string fileName) => _cache.ContainsKey(fileName);
}

public sealed class TagManager
{
    private readonly Dictionary<string, HashSet<string>> _articleTags =
        new(StringComparer.Ordinal);

    public void AddTag(string articleId, string tag)
    {
        if (!_articleTags.TryGetValue(articleId, out var tags))
        {
            tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _articleTags.Add(articleId, tags);
        }

        tags.Add(tag);
    }

    public bool RemoveTag(string articleId, string tag) =>
        _articleTags.TryGetValue(articleId, out var tags) && tags.Remove(tag);

    public HashSet<string> GetAllUniqueTags()
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tags in _articleTags.Values)
            result.UnionWith(tags);
        return result;
    }

    public HashSet<string> FindCommonTags(string articleId1, string articleId2)
    {
        var result = new HashSet<string>(
            _articleTags.TryGetValue(articleId1, out var first)
                ? first
                : [],
            StringComparer.OrdinalIgnoreCase);

        result.IntersectWith(
            _articleTags.TryGetValue(articleId2, out var second)
                ? second
                : []);
        return result;
    }

    public HashSet<string> FindExclusiveTags(string articleId)
    {
        var result = GetAllUniqueTags();
        if (_articleTags.TryGetValue(articleId, out var tags))
            result.ExceptWith(tags);
        return result;
    }
}

public sealed class SearchIndex
{
    private readonly Dictionary<string, HashSet<int>> _invertedIndex =
        new(StringComparer.OrdinalIgnoreCase);

    public void AddDocument(int docId, string content)
    {
        foreach (var word in SplitWords(content))
        {
            if (!_invertedIndex.TryGetValue(word, out var documents))
            {
                documents = [];
                _invertedIndex.Add(word, documents);
            }

            documents.Add(docId);
        }
    }

    public HashSet<int> Search(string word)
    {
        if (_invertedIndex.TryGetValue(word, out var documents))
            return new HashSet<int>(documents);
        return [];
    }

    public HashSet<int> SearchMultiple(params string[] words)
    {
        if (words.Length == 0)
            return [];

        var result = Search(words[0]);
        for (var i = 1; i < words.Length && result.Count > 0; i++)
            result.IntersectWith(Search(words[i]));
        return result;
    }

    private static IEnumerable<string> SplitWords(string content)
    {
        var word = new StringBuilder();
        foreach (var character in content)
        {
            if (char.IsLetterOrDigit(character) || character is '#' or '+')
                word.Append(character);
            else if (word.Length > 0)
            {
                yield return word.ToString();
                word.Clear();
            }
        }

        if (word.Length > 0)
            yield return word.ToString();
    }
}

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=== Сценарій 1: Кеш метаданих ===");
        var metadataCache = new FileMetadataCache();
        metadataCache.Add("REPORT.pdf", new FileMetadata
        {
            FileName = "REPORT.pdf",
            Size = 125_000,
            LastModified = new DateTime(2026, 11, 18),
            Hash = "A1B2C3"
        });
        metadataCache.Add("Photo.jpg", new FileMetadata
        {
            FileName = "Photo.jpg",
            Size = 980_000,
            LastModified = new DateTime(2026, 11, 17),
            Hash = "D4E5F6"
        });
        Console.WriteLine(metadataCache.Get("report.pdf"));
        Console.WriteLine($"містить Photo.jpg: {metadataCache.Contains("Photo.jpg")}");
        Console.WriteLine($"видалено REPORT.pdf: {metadataCache.Remove("REPORT.pdf")}");

        Console.WriteLine("\n=== Сценарій 2: Унікальні теги ===");
        var tags = new TagManager();
        tags.AddTag("article-1", "C#");
        tags.AddTag("article-1", "LINQ");
        tags.AddTag("article-1", "C#");
        tags.AddTag("article-2", "LINQ");
        tags.AddTag("article-2", "PLINQ");

        Console.WriteLine("Усі теги: " + string.Join(", ", tags.GetAllUniqueTags()));
        Console.WriteLine("Спільні теги: " + string.Join(", ",
            tags.FindCommonTags("article-1", "article-2")));

        Console.WriteLine("\n=== Сценарій 3: Пошуковий індекс ===");
        var index = new SearchIndex();
        index.AddDocument(1, "C# Dictionary and HashSet collections");
        index.AddDocument(2, "LINQ and C# search examples");
        index.AddDocument(3, "Dictionary search index in C#");

        Console.WriteLine("c#: " + string.Join(", ", index.Search("C#")));
        Console.WriteLine("Dictionary+C#: " +
            string.Join(", ", index.SearchMultiple("dictionary", "c#")));
    }
}
