namespace OpenAiApiSample;

class Program
{
    static async Task Main(string[] args)
    {
        var openAiApiService = new OpenAiApiService();

        // モデルの取得
        //await openAiApiService.ListModelAsync();
        //await openAiApiService.RetrieveModelAsync("gpt-3.5-turbo");

        // チャット
        //var systemPromt = "あなたは優秀なアシスタントです。";
        //var firstQuestion = "前回のオリンピックは何年の開催でしたか？";
        //await openAiApiService.CreateCompletionAsync(systemPromt, firstQuestion);

        // 画像生成、編集、バリエーション作成
        //await openAiApiService.CreateImageAsync("草原を馬が走っている画像を作成してください。", 2);
        //var path = "";
        //var imageName = "";
        ////await openAiApiService.CreateImageVariationAsync(path, imageName, 2);
        //await openAiApiService.CreateImageEditAsync("赤っぽい色にしてください。", path, imageName, 1);

        // 音声の文字起こし、翻訳
        //var path = "";
        //var fileName = "";
        ////await openAiApiService.CreateTranslationAsync(path, fileName);
        //await openAiApiService.CreateTranscriptionAsync(path, fileName);

        // モデレーション
        //await openAiApiService.CreateModerationAsync(new[] { "I want to kill them.", "Hello!!" });

        await Task.CompletedTask;
    }
}

