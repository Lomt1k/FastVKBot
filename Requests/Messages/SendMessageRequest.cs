using FastVKBot.DataTypes;
using Newtonsoft.Json;
using System.Text;

namespace FastVKBot.Requests.Messages;
internal class SendMessageRequest : RequestBase<MessageId>
{
    private readonly int _randomId = new Random().Next();

    public override string MethodName => "messages.send";
    public int RandomId => _randomId;
    public UserId UserId { get; set; }
    public string? Message { get; set; }

    public override string GetRequestForVkScript()
    {
        var sb = new StringBuilder();
        using (var sw = new StringWriter(sb))
        {
            using var jsonWriter = new JsonTextWriter(sw);
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("user_id");
            jsonWriter.WriteValue(UserId);
            jsonWriter.WritePropertyName("random_id");
            jsonWriter.WriteValue(RandomId);
            if (Message is not null)
            {
                jsonWriter.WritePropertyName("message");
                jsonWriter.WriteValue(Message);
            }            
            jsonWriter.WriteEndObject();
        }
        var json = sb.ToString();

        return $"API.messages.send({json});";
    }

    public override void ReadAndSetResult(JsonTextReader reader)
    {
        var messageId = new MessageId();
        messageId.ReadFromResponse(reader);
        SetResult(messageId);
    }
}
