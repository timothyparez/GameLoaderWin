using Newtonsoft.Json;
using System.Drawing;

namespace TeamArg.GameLoader.Common.Model
{
    public class ScreenshotInfo : ChangeNotifyingObject
    {
        [RaisePropertyChanged]
        [JsonProperty("title")]
        public string Title { get; set; }

        [RaisePropertyChanged]
        [JsonProperty("filename")]
        public string Filename { get; set; }

        [RaisePropertyChanged]
        [JsonIgnore]
        public Bitmap Bitmap { get; set; }
    }
}