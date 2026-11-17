using System.Text.Json;

namespace SvoyMaster;

public class SavedData
{
    public List<Master> Masters { get; set; } = new();
    public List<ServiceOrder> Orders { get; set; } = new();
    public int NextId { get; set; } = 1;
}

public static class AppData
{
    const string StorageKey = "svoymaster-demo-v1";
    public static SavedData Data { get; private set; } = CreateDemo();
    public static string LoadMessage { get; private set; } = "";

    public static void Load()
    {
        string json = Preferences.Default.Get(StorageKey, "");
        if (json.Length == 0) return;
        try
        {
            var saved = JsonSerializer.Deserialize<SavedData>(json);
            if (saved != null && saved.Masters.Count > 0) Data = saved;
        }
        catch (JsonException)
        {
            LoadMessage = "Сохранённые данные не прочитаны. Открыт новый демопример.";
        }
    }

    public static void Save()
    {
        Preferences.Default.Set(StorageKey, JsonSerializer.Serialize(Data));
    }

    public static void Reset()
    {
        Data = CreateDemo();
        LoadMessage = "";
        Save();
    }

    static SavedData CreateDemo()
    {
        return new SavedData
        {
            Masters = new List<Master>
            {
                new() { Id = 1, Name = "Алексей", Service = "Сантехника", Price = 1500,
                    About = "Замена смесителя и мелкий ремонт сантехники." },
                new() { Id = 2, Name = "Ирина", Service = "Уборка", Price = 2000,
                    About = "Уборка квартиры до 40 м²." },
                new() { Id = 3, Name = "Дмитрий", Service = "Электрика", Price = 1200,
                    About = "Замена розетки или выключателя." },
                new() { Id = 4, Name = "Сергей", Service = "Сборка мебели", Price = 2500,
                    About = "Сборка небольшого стола или комода." }
            }
        };
    }
}
