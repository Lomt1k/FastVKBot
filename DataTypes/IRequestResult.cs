using Newtonsoft.Json;

namespace FastVKBot.DataTypes;
internal interface IRequestResult
{
    void ReadFromResponse(JsonTextReader reader);
}
