using FastVKBot.DataTypes;
using FastVKBot.Requests.Messages;
using HellBrick.Collections;
using Newtonsoft.Json;
using System.Text;

namespace FastVKBot.Requests;
internal class VkScriptExecutor
{
    private readonly AsyncQueue<IRequestBase> _queue = new();
    private readonly string _token;
    private readonly HttpClient _httpClient;

    public VkScriptExecutor(VkBotClient botClient)
    {
        _token = botClient.Token;
        _httpClient = botClient.HttpClient;
        Task.Run(EcecuteRequestsLoop);
    }

    private async Task EcecuteRequestsLoop()
    {
        var delay = 1000 / Definitions.EXECUTIONS_PER_SECOND_LIMIT;
        while (true)
        {
            await Task.Delay(delay).ConfigureAwait(false);
            Task.Run(ExecuteNext);
        }
    }


    public void AddRequest(IRequestBase request)
    {
        Console.WriteLine($"AddRequest {request.GetType().Name}");
        _queue.Add(request);
    }

    private async Task ExecuteNext()
    {
        var requests = new List<IRequestBase>();
        var count = Math.Min(_queue.Count, Definitions.REQUESTS_IN_EXECUTION_LIMIT);
        for (int i = 0; i < count; i++)
        {
            var request = await _queue.TakeAsync().ConfigureAwait(false);
            if (request is not null)
            {
                requests.Add(request);
            }
        }

        Console.WriteLine($"requests count: {requests.Count}");
        if (requests.Count < 1)
        {
            return;
        }
        Console.WriteLine("Real execute!");

        var code = CreateVkScript(requests);
        var url = $"{Definitions.VK_API_ENDPOINT}execute";
        var query = new Dictionary<string, string>
        {
            ["access_token"] = _token,
            ["v"] = Definitions.VK_API_VERSION,
            ["code"] = code,
        };
        var encodedContent = new FormUrlEncodedContent(query);
        var httpResponse = await _httpClient.PostAsync(url, encodedContent).ConfigureAwait(false);
        var jsonStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

        using var streamReader = new StreamReader(jsonStream);
        using var jsonReader = new JsonTextReader(streamReader);

        while (jsonReader.Read())
        {
            if (jsonReader.TokenType == JsonToken.PropertyName)
            {
                var key = jsonReader.Value.ToString();
                switch (key)
                {
                    case "response":
                        HandleResponse(jsonReader, requests);
                        return;
                }
            }
        }

    }

    private void HandleResponse(JsonTextReader reader, List<IRequestBase> requests)
    {
        while (reader.Read())
        {
            var index = int.Parse(reader.Value.ToString());
            var request = (RequestWithResult<IRequestResult>) requests[index];
            request.ReadAndSetResult(reader);
        }
    }

    private string CreateVkScript(List<IRequestBase> requests)
    {
        var sb = new StringBuilder();
        sb.AppendLine("var results = [];");
        foreach (var request in requests)
        {
            var json = (request as RequestWithResult<IRequestResult>).GetRequestForVkScript();
            sb.AppendLine($"results.push({json})");
        }
        sb.AppendLine("return results;");
        return sb.ToString();
    }


}
