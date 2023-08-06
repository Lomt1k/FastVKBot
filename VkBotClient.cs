using FastVKBot.DataTypes;
using FastVKBot.Requests.Messages;
using System.Net.Http;
using System.Text;

namespace FastVKBot;

public class VkBotClient
{
    public string Token { get; }
    public HttpClient HttpClient { get; } = new();

    public VkBotClient(string token)
    {
        Token = token;
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

    public async Task SendMessage(UserId userId, string message)
    {
        var request = new SendMessageRequest
        {
            UserId = userId,
            Message = message,
        };
    }

}
