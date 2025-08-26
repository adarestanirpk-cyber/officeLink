using System.Text.Json;

namespace Application.Mappers;

public static class ToEntityFieldsClass
{
    public static Dictionary<string, Dictionary<string, string>> ToEntityFields(string json, string bucketKey = "fields")
    {
        var root = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                   ?? new Dictionary<string, JsonElement>();

        var inner = new Dictionary<string, string>();

        foreach (var kv in root)
        {
            inner[kv.Key] = kv.Value.ValueKind switch
            {
                JsonValueKind.String => kv.Value.GetString()!,
                JsonValueKind.Number => kv.Value.ToString(), // یا kv.Value.GetInt32().ToString(CultureInfo.InvariantCulture)
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => string.Empty,
                JsonValueKind.Object => kv.Value.GetRawText(), // شیء را به صورت JSON رشته می‌کنیم
                JsonValueKind.Array => kv.Value.GetRawText(), // آرایه را هم به صورت JSON
                _ => kv.Value.GetRawText()
            };
        }

        return new Dictionary<string, Dictionary<string, string>>
        {
            [bucketKey] = inner
        };
    }
}
