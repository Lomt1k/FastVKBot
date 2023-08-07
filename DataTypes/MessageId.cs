using Newtonsoft.Json;

namespace FastVKBot.DataTypes;
public struct MessageId : IRequestResult
{
    public long Id { get; set; }

    public MessageId(long id)
    {
        Id = id;
    }

    public void ReadFromResponse(JsonTextReader reader)
    {
        Id = long.Parse(reader.ReadAsString());
    }

    public override string ToString()
    {
        return Id.ToString();
    }

    public static implicit operator MessageId(long id) => new(id);
}
