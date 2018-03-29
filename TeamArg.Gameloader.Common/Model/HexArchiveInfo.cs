using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamArg.GameLoader.Common.Model
{
    public class HexArchiveInfo : ChangeNotifyingObject
    {
        [RaisePropertyChanged]
        [JsonProperty("title")]
        public string Title { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("description")]
        public string Description { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("version")]
        public string Version { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("genre")]
        public string Genre { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("device")]
        public string Device { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("date")]
        public string Date { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("author")]
        public string Author { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("url")]
        public string Url { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("companion")]
        public string Companion { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("sourceUrl")]
        public string SourceUrl { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("email")]
        public string EMail { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("idea")]
        public string Idea { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("code")]
        public string Code { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("art")]
        public string Art { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("sound")]
        public string Sound { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("banner")]
        public string Banner { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("buttons")]
        public List<ButtonDefinition> Buttons { get; set; }        

        [RaisePropertyChanged]
        [JsonProperty("binaries")]
        public List<BinaryInfo> Binaries { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("screenshots")]
        public List<ScreenshotInfo> Screenshots { get; set; }

        public HexArchiveInfo()
        {
            Screenshots = new List<Model.ScreenshotInfo>();
            Binaries = new List<Model.BinaryInfo>();
        }
    }
}
