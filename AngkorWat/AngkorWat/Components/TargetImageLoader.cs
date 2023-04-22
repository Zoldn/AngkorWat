using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using AngkorWat.Utils;

namespace AngkorWat.Components
{
    internal class Chunk
    {
        public int X { get; init; }
        public int Y { get; init; }
        public Dictionary<Color, int> PresentColors { get; set; }
        public Color Color { get; set; }
        public bool IsShooting { get; set; }
        public Chunk()
        {
            PresentColors = new();
            IsShooting = true;
        }
    }
    internal class PointOfImage
    {
        public int X { get; init; }
        public int Y { get; init; }
        public Color Color { get; init; }
    }
    internal class TargetImageLoader
    {
        public string FilePath { get; set; }
        public int ChunkSize { get; init; }
        public TargetImageLoader(string filePath)
        {
            FilePath = filePath;
            ChunkSize = 16;
        }

        private List<PointOfImage> GetTensor()
        {
            int width = 0;
            int height = 0;

            var pointsOfImage = new List<PointOfImage>(height * width);

            using (var imageIn = Image.FromFile(FilePath))
            using (var bmp = new Bitmap(imageIn))
            {
                for (int row = 0; row < bmp.Height; row++)
                {
                    for (int column = 0; column < bmp.Width; column++)
                    {
                        var pointOfImage = new PointOfImage()
                        {
                            X = column,
                            Y = row,
                            Color = bmp.GetPixel(column, row),
                        };

                        pointsOfImage.Add(pointOfImage);
                    }
                }
            }

            return pointsOfImage;
        }

        public List<Chunk> Run()
        {
            var tensor = GetTensor();

            var chunks = tensor
                .GroupBy(
                    p => new
                    {
                        CX = p.X / ChunkSize,
                        CY = p.Y / ChunkSize
                    }
                )
                .Select(g => new Chunk()
                {
                    X = g.Key.CX + ChunkSize / 2,
                    Y = g.Key.CY + ChunkSize / 2,
                    PresentColors = g
                        .GroupBy(q => q.Color)
                        .ToDictionary(q => q.Key, q => q.Count())
                })
                .OrderBy(c => c.X)
                .ThenBy(c => c.Y)
                .ToList();

            foreach (var chunk in chunks)
            {
                var mostCommonColor = chunk.PresentColors.ArgMax(kv => kv.Value).Key;

                chunk.Color = mostCommonColor;

                if (chunk.Color.R == 255 && chunk.Color.G == 255 && chunk.Color.B == 255)
                {
                    chunk.IsShooting = false;
                }
            }

            return chunks;
        }
    }
}
