using AngkorWat.Algorithms.CBrewer.Components;
using AngkorWat.Components;
using AngkorWat.IO.HTTP;
using AngkorWat.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Phases
{
    

    internal class ColorListOnStoreResponse : BasicResponse
    {
        public Dictionary<string, int> response { get; set; }
        public ColorListOnStoreResponse() : base()
        {
            response = new();
        }
    }

    internal class ShotResponse : BasicResponse
    {
        public Dictionary<string, Dictionary<string, long>> response { get; set; }
        public ShotResponse() : base()
        {
            response = new();
        }
    }

    internal class Shoota
    {
        public string ColorListUrl { get; }
        public string UrlShoot { get; set; }
        private readonly Random random;
        public Shoota()
        {
            ColorListUrl = "http://api.datsart.dats.team/art/colors/list";
            UrlShoot = "http://api.datsart.dats.team/art/ballista/shoot";

            random = new Random();
        }

        internal async Task TestShooting(Shot shot)
        {
            var payload = new Dictionary<string, string>()
            {
                { "angleHorizontal", shot.HAngle.ToString() },
                { "angleVertical", shot.VAngle.ToString() },
                { "power", shot.Power.ToString() },
            };

            foreach (var (color, amount) in shot.ColorCodes)
            {
                payload.Add($"colors[{color}]", amount.ToString());
            }

            var response = await HttpHelper.PostMultipartWithContent(UrlShoot, payload);

            var container = JsonConvert.DeserializeObject<ShotResponse>(response);

            if (container is null || !container.success) 
            {
                Console.WriteLine("Failed shot");
                Console.WriteLine(response);
            }
        }

        private class TargetDistanceRecord 
        {
            public int VAngle { get; init; }
            public int Power { get; init; }
            public double RoundedDistance { get; init; }
        }

        public static Shot InitializeShot(int x, int y, int canvasWidth, 
            int mass,
            int shootaYPosition = -300)
        {
    
            var shot = new Shot();

            const double eta = 203.9d;

            var distanceToTarget = Math.Sqrt(
                (x - canvasWidth / 2.0d) * (x - canvasWidth / 2.0d) + 
                (y - shootaYPosition) * (y - shootaYPosition));

            shot.HAngle = Math.Round(Math.Asin((x - canvasWidth / 2.0d) / distanceToTarget) / Math.PI * 180.0d);

            var records = new List<TargetDistanceRecord>(90);

            for (int vAngle = 1; vAngle < 90; vAngle++)
            {
                double truePower = (distanceToTarget * mass / eta / Math.Sin(2 * Math.PI / 180.0d * vAngle));

                int roundPower = (int)Math.Round(truePower);

                double roundedDistance = eta * roundPower / mass * Math.Sin(2 * Math.PI / 180.0d * vAngle);

                var record = new TargetDistanceRecord()
                {
                    RoundedDistance = roundedDistance,
                    Power = roundPower,
                    VAngle = vAngle,
                };

                records.Add(record);
            }

            var bestRecord = records.ArgMin(e => Math.Abs(e.RoundedDistance - distanceToTarget));

            shot.Power = bestRecord.Power;
            shot.VAngle = bestRecord.VAngle;

            var rp = eta * shot.Power / mass * Math.Sin(2 * Math.PI / 180.0d * shot.VAngle);
            var xp = (int)Math.Round(canvasWidth / 2.0d + rp * Math.Sin(shot.HAngle * Math.PI / 180.0d));
            var yp = (int)Math.Round(shootaYPosition + rp * Math.Cos(shot.HAngle * Math.PI / 180.0d));

            var error = Math.Sqrt((xp - x) * (xp - x) + (yp - y) * (yp - y));

            Console.WriteLine($"\tBest shot to target ({x}, {y}) is ({xp}, {yp}), error = {error}");

            return shot;
        }

        internal async Task<int?> TakeRandomAvailableColor(int minValue = 1)
        {
            var listOfColorsResponse = await RequestColorList();

            if (listOfColorsResponse is null
                || !listOfColorsResponse.success)
            {
                return null;
            }

            var notZeroAvailableColors = listOfColorsResponse.response
                .Where(kv => kv.Value >= minValue)
                .Select(kv => kv.Key)
                .ToList();

            string colorString = notZeroAvailableColors[random.Next(notZeroAvailableColors.Count)];

            int colorCode = int.Parse(colorString);

            Console.WriteLine(colorCode);

            return colorCode;
        }

        internal async Task<List<AvailableColorRecord>> GetAllAvailableColors()
        {
            var listOfColorsResponse = await RequestColorList();

            if (listOfColorsResponse is null
                || !listOfColorsResponse.success)
            {
                return new List<AvailableColorRecord>();
            }

            var notZeroAvailableColors = listOfColorsResponse.response
                .Where(kv => kv.Value > 0)
                .Select(kv => new AvailableColorRecord(int.Parse(kv.Key), kv.Value))
                .ToList();

            //int colorCode = int.Parse(colorString);

            //Console.WriteLine(colorCode);

            return notZeroAvailableColors;
        }

        private async Task<ColorListOnStoreResponse?> RequestColorList()
        {
            var response = await HttpHelper.PostMultipart(ColorListUrl);

            var container = JsonConvert.DeserializeObject<ColorListOnStoreResponse>(response);

            //Console.WriteLine(response);

            return container;
        }
    }
}
