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
    public ulong? StickerId { get; set; }
    public HashSet<string>? Attachments { get; set; }
    public bool DontParseLinks { get; set; } = false;

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
                jsonWriter.WritePropertyName("dont_parse_links");
                jsonWriter.WriteValue(DontParseLinks);

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
                else if (StickerId is not null)
                {
                    jsonWriter.WritePropertyName("sticker_id");
                    jsonWriter.WriteValue(StickerId);
                }
                if (Attachments?.Count > 0)
                {
                    jsonWriter.WritePropertyName("attachment");
                    jsonWriter.WriteStartArray();
                    foreach (var attachment in Attachments)
                    {
                        jsonWriter.WriteValue(attachment.ToString());
                    }
                    jsonWriter.WriteEndArray();
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
