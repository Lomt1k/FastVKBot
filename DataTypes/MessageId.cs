using Newtonsoft.Json;

namespace FastVKBot.DataTypes;
public struct MessageId : IRequestResult
{
    public long Id { get; set; }

    public void ReadFromResponse(JsonTextReader reader)
    {
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return;
            }
            if (reader.TokenType == JsonToken.PropertyName)
            {
                var key = reader.Value.ToString();
                switch (key)
                {
                    case "response":
                        Id = long.Parse(reader.ReadAsString());
                        return;
                }
            }
        }
    }

    public override string ToString()
    {
        return Id.ToString();
    }
}
