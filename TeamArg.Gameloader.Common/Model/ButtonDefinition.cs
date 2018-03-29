using Newtonsoft.Json;

namespace TeamArg.GameLoader.Common.Model
{
    public class ButtonDefinition : ChangeNotifyingObject
    {
        [RaisePropertyChanged]
        [JsonProperty("control")]
        public string Control { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("action")]
        public string Action { get; set; }
    }
}