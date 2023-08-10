using FastVKBot.DataTypes;
using Newtonsoft.Json;
using System.Text;

namespace FastVKBot.Requests.Messages;
internal class SendMessageRequest : RequestWithResult<MessageId>
{
    private readonly int _randomId = new Random().Next();

    public override string MethodName => "messages.send";
    public int RandomId => _randomId;
    public UserId UserId { get; set; }
    public string? Message { get; set; }
    public ulong? StickerId { get; set; }
    public HashSet<string>? Attachments { get; set; }
    public bool DontParseLinks { get; set; } = false;

    public override string GetRequestForVkScript()
    {
        var sb = new StringBuilder();
        using (var sw = new StringWriter(sb))
        {
            using var jsonWriter = new JsonTextWriter(sw);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("user_id");
            jsonWriter.WriteValue(UserId.ToString());
            jsonWriter.WritePropertyName("random_id");
            jsonWriter.WriteValue(RandomId);
            jsonWriter.WritePropertyName("dont_parse_links");
            jsonWriter.WriteValue(DontParseLinks);

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

        return $"API.{MethodName}({json});";
    }

    public override void ReadAndSetResult(JsonTextReader reader)
    {
        var messageId = new MessageId();
        messageId.ReadFromResponse(reader);
        SetResult(messageId);
    }
}
