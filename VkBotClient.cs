using FastVKBot.DataTypes;
using FastVKBot.Requests;
using FastVKBot.Requests.Messages;

namespace FastVKBot;

public class VkBotClient
{
    private VkScriptExecutor _executor;
    private MultipleRequestScheduler _multipleRequestScheduler;

    public string Token { get; }
    public HttpClient HttpClient { get; } = new();


    public VkBotClient(string token)
    {
        Token = token;

        _executor = new(this);
        _multipleRequestScheduler = new(_executor);
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

    public async Task SendMultipleMessageAsync(UserId userId, string message)
    {
        var task = _multipleRequestScheduler.ScheduleMessage(userId, message);
        await task.ConfigureAwait(false);
    }

}
