using FastVKBot.DataTypes;
using FastVKBot.Requests.Messages;
using System.Collections.Concurrent;

namespace FastVKBot.Requests;
internal class MultipleRequestScheduler
{
    private readonly VkScriptExecutor _executor;
    private readonly ConcurrentDictionary<string, MultipleSendMessageRequest> _multipleMessagesDict = new();

    public MultipleRequestScheduler(VkScriptExecutor executor)
    {
        _executor = executor;
    }

    public Task ScheduleMessage(UserId userId, string message)
    {
        var id = "messages.send" + message;
        lock (id)
        {
            if (_multipleMessagesDict.TryGetValue(id, out var request))
            {
                if (request.TryAddToRequest(userId))
                {
                    return request.Task;
                }
            }

            request = new MultipleSendMessageRequest
            {
                UserIds = new HashSet<UserId> { userId },
                Message = message,
            };
            _multipleMessagesDict[id] = request;
            _executor.AddRequest(request);
            return request.Task;
        }
    }

}
