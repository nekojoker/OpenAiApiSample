using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System.Text.Json;

namespace OpenAiApiSample;

public sealed class OpenAiApiService
{
    #region フィールド
    private const string ApiKey = "";
    private readonly OpenAIService openAiService;
    #endregion

    #region コンストラクタ
    public OpenAiApiService()
    {
        openAiService = new OpenAIService(new OpenAiOptions
        {
            ApiKey = ApiKey
        });
    }
    #endregion

    #region ListModelAsync
    public async ValueTask ListModelAsync(CancellationToken cancellationToken = default)
    {
        var response = await openAiService.Models.ListModel(cancellationToken);
        if (!response.Successful)
        {
            Console.WriteLine($"Error: {response.Error?.Message}");
            return;
        }

        var ids = response.Models.Select(static x => x.Id).OrderBy(static x => x);
        Console.WriteLine(string.Join("\n", ids));
    }
    #endregion

    #region RetrieveModelAsync
    public async ValueTask RetrieveModelAsync(string model, CancellationToken cancellationToken = default)
    {
        var response = await openAiService.Models.RetrieveModel(model, cancellationToken);
        if (!response.Successful)
        {
            Console.WriteLine($"Error: {response.Error?.Message}");
            return;
        }

        var options = JsonSerializerOptionsProvider.Default;
        var json = JsonSerializer.Serialize(response, options);
        Console.WriteLine(json);
    }
    #endregion

    #region CreateCompletionAsync
    public async ValueTask CreateCompletionAsync(string systemPrompt, string firstQuestion, IList<ChatMessage>? messages = null, CancellationToken cancellationToken = default)
    {
        messages ??= new List<ChatMessage>
        {
            ChatMessage.FromSystem(systemPrompt),
            ChatMessage.FromUser(firstQuestion)
        };
        var response = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = Models.Gpt_3_5_Turbo,
            N = 1,
            User = Guid.NewGuid().ToString(),
            MaxTokens = 30
        }, cancellationToken: cancellationToken);

        if (!response.Successful)
        {
            Console.WriteLine($"Error: {response.Error?.Message}");
            return;
        }

        var choice = response.Choices.First();
        var content = choice.Message.Content;
        messages.Add(ChatMessage.FromAssistant(content!));

        Console.WriteLine(content);

        if (choice.FinishReason == "stop")
        {
            Console.WriteLine("input waiting...");

            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) return;
            messages.Add(ChatMessage.FromUser(input));
        }

        await CreateCompletionAsync(systemPrompt, firstQuestion, messages, cancellationToken);
    }
    #endregion

    #region CreateImageAsync
    public async ValueTask CreateImageAsync(string prompt, int number, CancellationToken cancellationToken = default)
    {
        var response = await openAiService.Image.CreateImage(new ImageCreateRequest
        {
            Prompt = prompt,
            N = number,
            User = Guid.NewGuid().ToString(),
            Size = StaticValues.ImageStatics.Size.Size256,
            ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url
        }, cancellationToken: cancellationToken);

        if (!response.Successful)
        {
            Console.WriteLine($"Error: {response.Error?.Message}");
            return;
        }

        Console.WriteLine(string.Join("\n\n", response.Results.Select(static x => x.Url)));
    }
    #endregion

    #region CreateImageVariationAsync
    public async ValueTask CreateImageVariationAsync(string path, string imageName, int number, CancellationToken cancellationToken = default)
    {
        var image = ConvertFileToByteArray(path);
        var response = await openAiService.Image.CreateImageVariation(new ImageVariationCreateRequest
        {
            N = number,
            User = Guid.NewGuid().ToString(),
            Size = StaticValues.ImageStatics.Size.Size256,
            ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
            Image = image,
            ImageName = imageName
        }, cancellationToken: cancellationToken);

        if (!response.Successful)
        {
            Console.WriteLine($"Error: {response.Error?.Message}");
            return;
        }

        Console.WriteLine(string.Join("\n\n", response.Results.Select(static x => x.Url)));
    }
    #endregion

    #region CreateImageEditAsync
    public async ValueTask CreateImageEditAsync(string prompt, string path, string imageName, int number, CancellationToken cancellationToken = default)
    {
        var image = ConvertFileToByteArray(path);
        var response = await openAiService.Image.CreateImageEdit(new ImageEditCreateRequest
        {
            N = number,
            User = Guid.NewGuid().ToString(),
            Size = StaticValues.ImageStatics.Size.Size256,
            ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
            Image = image,
            ImageName = imageName,
            Prompt = prompt
        }, cancellationToken: cancellationToken);

        if (!response.Successful)
        {
            Console.WriteLine($"Error: {response.Error?.Message}");
            return;
        }

        Console.WriteLine(string.Join("\n\n", response.Results.Select(static x => x.Url)));
    }
    #endregion

    #region CreateTranslationAsync
    public async ValueTask CreateTranslationAsync(string path, string fileName, CancellationToken cancellationToken = default)
    {
        var file = ConvertFileToByteArray(path);
        var response = await openAiService.Audio.CreateTranslation(new AudioCreateTranscriptionRequest
        {
            File = file,
            FileName = fileName,
            Model = Models.WhisperV1,
            Language = "en",
            ResponseFormat = StaticValues.AudioStatics.ResponseFormat.VerboseJson,
        }, cancellationToken: cancellationToken);

        if (!response.Successful)
        {
            Console.WriteLine($"Error: {response.Error?.Message}");
            return;
        }

        var options = JsonSerializerOptionsProvider.Default;
        var json = JsonSerializer.Serialize(response, options);
        Console.WriteLine(json);
    }
    #endregion

    #region CreateTranscriptionAsync
    public async ValueTask CreateTranscriptionAsync(string path, string fileName, CancellationToken cancellationToken = default)
    {
        var file = ConvertFileToByteArray(path);
        var response = await openAiService.Audio.CreateTranscription(new AudioCreateTranscriptionRequest
        {
            File = file,
            FileName = fileName,
            Model = Models.WhisperV1,
            Language = "en",
            ResponseFormat = StaticValues.AudioStatics.ResponseFormat.VerboseJson,
        }, cancellationToken: cancellationToken);

        if (!response.Successful)
        {
            Console.WriteLine($"Error: {response.Error?.Message}");
            return;
        }

        var options = JsonSerializerOptionsProvider.Default;
        var json = JsonSerializer.Serialize(response, options);
        Console.WriteLine(json);
    }
    #endregion

    #region CreateModerationAsync
    public async ValueTask CreateModerationAsync(IEnumerable<string> inputAsList, CancellationToken cancellationToken = default)
    {
        var response = await openAiService.Moderation.CreateModeration(new CreateModerationRequest
        {
            InputAsList = inputAsList.ToList()
        }, cancellationToken: cancellationToken);

        if (!response.Successful)
        {
            Console.WriteLine($"Error: {response.Error?.Message}");
            return;
        }

        var options = JsonSerializerOptionsProvider.Default;
        var json = JsonSerializer.Serialize(response, options);
        Console.WriteLine(json);
    }
    #endregion

    #region Helpers
    private static byte[] ConvertFileToByteArray(string path)
    {
        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        using (var ms = new MemoryStream())
        {
            fs.CopyTo(ms);
            return ms.ToArray();
        }
    }
    #endregion
}

