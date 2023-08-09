using FastVKBot.DataTypes;
using Newtonsoft.Json;
using System.Text;

namespace FastVKBot.Requests.Messages;
internal class MultipleSendMessageRequest : MultipleRequest<UserId>
{
    private readonly int _randomId = new Random().Next();
    private readonly object _addUserIdLock = new();

    private bool _isRequestSended;

    public override string MethodName => "messages.send";
    public int RandomId => _randomId;
    public HashSet<UserId> UserIds { get; set; } = new();
    public string? Message { get; set; }

    public override string GetRequestForVkScript()
    {
        lock (_addUserIdLock)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using var jsonWriter = new JsonTextWriter(sw);
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("random_id");
                jsonWriter.WriteValue(RandomId);

                jsonWriter.WritePropertyName("user_ids");
                jsonWriter.WriteStartArray();
                foreach (var userId in UserIds)
                {
                    jsonWriter.WriteValue(userId.ToString());
                }
                jsonWriter.WriteEndArray();

                if (Message is not null)
                {
                    jsonWriter.WritePropertyName("message");
                    jsonWriter.WriteValue(Message);
                }
                jsonWriter.WriteEndObject();
            }
            var json = sb.ToString();

            _isRequestSended = true;
            return $"API.{MethodName}({json});";
        }
    }

    public override void ReadAndSetResult(JsonTextReader reader)
    {
        SetResult();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as MultipleSendMessageRequest);
    }

    public bool Equals(MultipleSendMessageRequest? other)
    {
        return other != null
            && other.MethodName == MethodName
            && other.RandomId == RandomId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MethodName, RandomId);
    }

    public override string GetMultipleRequestId()
    {
        return MethodName + Message;
    }

    public override bool TryAddToRequest(UserId value)
    {
        lock (_addUserIdLock)
        {
            if (UserIds.Count >= Definitions.PEERS_LIMIT_FOR_ONE_MESSAGE || _isRequestSended)
            {
                return false;
            }
            UserIds.Add(value);
            return true;
        }
    }

}
