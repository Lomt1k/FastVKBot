using FastVKBot.DataTypes;
using FastVKBot.Requests;
using FastVKBot.Requests.Messages;

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
