using Newtonsoft.Json;

namespace FastVKBot.Requests;
internal interface IRequestBase
{
    string MethodName { get; }
    string GetRequestForVkScript();
    void ReadAndSetResult(JsonTextReader reader);
}
