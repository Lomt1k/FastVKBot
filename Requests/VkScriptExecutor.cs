using FastVKBot.DataTypes;
using HellBrick.Collections;
using Newtonsoft.Json;
using System.Text;

namespace FastVKBot.Requests;
internal class VkScriptExecutor
{
    private readonly AsyncQueue<RequestBase<IRequestResult>> _queue = new();
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
            await Task.Delay(delay);
            Task.Run(ExecuteNext);
        }
    }


    public void AddRequest(RequestBase<IRequestResult> request)
    {
        _queue.Add(request);
    }

    private async Task ExecuteNext()
    {
        var requests = new List<RequestBase<IRequestResult>>();
        for (int i = 0; i < Definitions.REQUESTS_IN_EXECUTION_LIMIT; i++)
        {
            var request = await _queue.TakeAsync().ConfigureAwait(false);
            if (request is not null)
            {
                requests.Add(request);
            }
        }

        if (requests.Count < 1)
        {
            return;
        }

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

    private void HandleResponse(JsonTextReader reader, List<RequestBase<IRequestResult>> requests)
    {
        while (reader.Read())
        {
            var index = int.Parse(reader.Value.ToString());
            requests[index].ReadAndSetResult(reader);
        }
    }

    private string CreateVkScript(List<RequestBase<IRequestResult>> requests)
    {
        var sb = new StringBuilder();
        sb.AppendLine("var results = [];");
        foreach (var request in requests)
        {
            sb.AppendLine($"results.push({request.GetRequestForVkScript()})");
        }
        sb.AppendLine("return results;");
        return sb.ToString();
    }


}
