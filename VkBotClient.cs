using FastVKBot.DataTypes;
using FastVKBot.Requests;
using FastVKBot.Requests.Messages;
using System.Net.Http;
using System.Text;

namespace FastVKBot;

public class VkBotClient
{
    private VkScriptExecutor _executor;

    public string Token { get; }
    public HttpClient HttpClient { get; } = new();


    public VkBotClient(string token)
    {
        Token = token;

        _executor = new(this);
    }

    public async Task ExecuteAsync(string code)
    {
        var url = $"{Definitions.VK_API_ENDPOINT}execute";
        var query = new Dictionary<string, string>
        {
            ["access_token"] = Token,
            ["v"] = Definitions.VK_API_VERSION,
            ["code"] = code,
        };
        var encodedContent = new FormUrlEncodedContent(query);
        var response = await HttpClient.PostAsync(url, encodedContent).ConfigureAwait(false);
    }

    public async Task<MessageId> SendMessageAsync(UserId userId, string message)
    {
        var request = new SendMessageRequest
        {
            UserId = userId,
            Message = message,
        };
        _executor.AddRequest(request);
        return await request.Task.ConfigureAwait(false);
    }

}
