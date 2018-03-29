using Newtonsoft.Json;
using SharpCompress.Archives.Zip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace TeamArg.GameLoader.Common.Model
{
    public class HexArchive : ChangeNotifyingObject
    {
        private const string INFO_FILENAME = "info.json";
        private const string BANNER_FILENAME = "banner.png";
        private const string CONTROLS_FILENAME = "controls.png";
        private const string SCREENSHOT_FILENAME_PREFIX = "screenshot";
        private const string SCREENSHOT_FILENAME_SUFFIX = "png";

        [RaisePropertyChanged]
        public string Filename { get; set; }

        [RaisePropertyChanged]
        public HexArchiveInfo Info { get; set; }

        [RaisePropertyChanged]
        public Bitmap Banner { get; set; }

        [RaisePropertyChanged]
        public List<Bitmap> Screenshots { get; set; }

        [RaisePropertyChanged]
        public Bitmap Controls { get; set; }

        [RaisePropertyChanged]
        public string SupportedDevices { get; set; }

        public static object JSonConvert { get; private set; }

        public static Optional<HexArchive> FromFile(string filename)
        {
            try
            {
                var hexArchive = new HexArchive();

                using (var zipArchive = ZipArchive.Open(filename))
                {
                    hexArchive.Filename = filename;
                    hexArchive.Info = ExtractInfo(zipArchive);
                    hexArchive.Banner = ExtractBitmap(zipArchive, hexArchive.Info.Banner).Value;                    
                    hexArchive.Controls = ExtractBitmap(zipArchive, CONTROLS_FILENAME).Value;
                    hexArchive.SupportedDevices = ExtractSupportedDevicesFromInfo(hexArchive.Info);        
                    PopulateScreenshotBitmapsFromInfo(zipArchive, hexArchive.Info);
                };

                return new Optional<HexArchive>(hexArchive);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new Optional<HexArchive>();
            }
        }

        private static string ExtractSupportedDevicesFromInfo(HexArchiveInfo info)
        {
            try
            {
                return info.Binaries.Select(b => b.Device).Distinct().Aggregate((l, r) => $"{l}, {r}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return String.Empty;                
            }
        }

        private static void PopulateScreenshotBitmapsFromInfo(ZipArchive zipArchive, HexArchiveInfo hexArchiveInfo)
        {
            foreach(var screenshot in hexArchiveInfo.Screenshots)
            {
                var bitmapEntry = zipArchive.Entries.FirstOrDefault(n => n.Key == screenshot.Filename);
                if (bitmapEntry != null)
                {
                    var bitmap = ExtractBitmap(zipArchive, bitmapEntry.Key);
                    if (bitmap.HasValue)
                    {
                        screenshot.Bitmap = bitmap.Value;
                    }
                }
            }
        }

        private static Optional<Bitmap> ExtractBitmap(ZipArchive zipArchive, string bitmapFilename)
        {
            Bitmap bitmap = null;

            try
            {
                var bitmapEntry = zipArchive.Entries.First(e => e.Key == bitmapFilename);

                using (var stream = bitmapEntry.OpenEntryStream())
                {
                    bitmap = new Bitmap(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load bitmap: {bitmapFilename}: {ex.Message}");
            }

            if (bitmap != null)
            {
                return new Optional<Bitmap>(bitmap);
            }
            else
            {
                return new Optional<Bitmap>();
            }
        }

        private static HexArchiveInfo ExtractInfo(ZipArchive zipArchive)
        {
            var infoEntry = zipArchive.Entries.First(e => e.Key.EndsWith(".json"));

            using (var stream = infoEntry.OpenEntryStream())
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                var settings = new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };
                return JsonConvert.DeserializeObject<HexArchiveInfo>(json, settings);
            }
        }

        public byte[] ExtactHexData()
        {
            using (var zipArchive = ZipArchive.Open(Filename))
            {
                var hexEntry = zipArchive.Entries.First(e => e.Key.ToLower().EndsWith(".hex"));

                using (var stream = hexEntry.OpenEntryStream())
                using (var reader = new BinaryReader(stream))
                {
                    return reader.ReadBytes((int)hexEntry.Size);
                }
            }
        }

        public byte[] ExtactHexData(string archivedFilename)
        {
            using (var zipArchive = ZipArchive.Open(Filename))
            {
                var hexEntry = zipArchive.Entries.First(e => e.Key.ToLower() == archivedFilename.ToLower());

                using (var stream = hexEntry.OpenEntryStream())
                using (var reader = new BinaryReader(stream))
                {
                    return reader.ReadBytes((int)hexEntry.Size);
                }
            }
        }

        public override string ToString()
        {
            return $"{Info.Title}";
        }
    }
}
