using FastVKBot.DataTypes;
using FastVKBot.Requests.Messages;
using System.Collections.Concurrent;

namespace FastVKBot.Requests;
internal class MultipleRequestScheduler
{
    private readonly VkScriptExecutor _executor;
    private readonly ConcurrentDictionary<int, MultipleSendMessageRequest> _multipleMessagesDict = new();

    public MultipleRequestScheduler(VkScriptExecutor executor)
    {
        _executor = executor;
    }

    public Task ScheduleMessage(UserId userId, string? message = null, ulong? stikerId = null, HashSet<string>? attachments = null, bool dontParseLinks = false)
    {
        var hashCode = HashCode.Combine(message, stikerId);
        if (attachments?.Count > 0)
        {
            foreach (var attachment in attachments)
            {
                hashCode = HashCode.Combine(hashCode, attachment);
            }
        }

        lock ($"messages.send-{hashCode}")
        {
            if (_multipleMessagesDict.TryGetValue(hashCode, out var request))
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
                StickerId = stikerId,
                Attachments = attachments,
                DontParseLinks = dontParseLinks,
            };
            _multipleMessagesDict[hashCode] = request;
            _executor.AddRequest(request);
            return request.Task;
        }
    }

}
