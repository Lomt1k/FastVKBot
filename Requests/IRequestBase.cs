using Newtonsoft.Json;

namespace FastVKBot.Requests;
internal interface IRequestBase
{
    string GetRequestForVkScript();
    void ReadAndSetResult(JsonTextReader reader);
}
