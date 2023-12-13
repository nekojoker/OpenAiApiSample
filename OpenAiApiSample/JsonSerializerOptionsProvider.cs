using System.Text.Encodings.Web;
using System.Text.Json;

namespace OpenAiApiSample;

public static class JsonSerializerOptionsProvider
{
    public static JsonSerializerOptions Default { get; } = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };
}

