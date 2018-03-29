using Newtonsoft.Json;

namespace TeamArg.GameLoader.Common.Model
{
    public class BinaryInfo : ChangeNotifyingObject
    {
        [RaisePropertyChanged]
        [JsonProperty("title")]                
        public string Title { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("filename")]
        public string Filename { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("device")]
        public string Device { get; set; }
    }
}