using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steganography.Encoders;
using Steganography.Model;

namespace Steganography.Signing
{
    public class SimpleHashSigner
    {
        private readonly IEncoder _signatureEncoder;
        private const int TileSize = 32;

        public SimpleHashSigner(IEncoder signatureEncoder)
        {
            _signatureEncoder = signatureEncoder;
        }

        public Bitmap Sign(Bitmap unsignedImage)
        {
            var signedImage = new Bitmap(unsignedImage.Width, unsignedImage.Height);

            var tiles = unsignedImage.SplitIntoTiles(TileSize, TileSize);
            using (var graphics = Graphics.FromImage(signedImage))
            {
                foreach (var tile in tiles)
                {
                    using (tile)
                    {
                        var hash = CalculateSimpleHash(tile);
                        var signedTile = _signatureEncoder.EncodeBits(hash, tile.Bitmap);
                        graphics.DrawImage(signedTile, tile.X, tile.Y);
                    }
                }
            }

            return signedImage;
        }

        public bool CheckSignature(Bitmap signedImage)
        {

            var tiles = signedImage.SplitIntoTiles(TileSize, TileSize);
            foreach (var tile in tiles)
            {
                using (tile)
                {
                    var hash = CalculateSimpleHash(tile);
                    var signature = _signatureEncoder.DecodeBits(tile.Bitmap);
                    if (hash != signature)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        private string CalculateSimpleHash(Tile tile)
        {
            const int brightnessThreshold = 127;
            const int matrixSize = 8;
            var matrix = new byte[matrixSize, matrixSize];
            var hashBuilder = new StringBuilder(matrixSize * matrixSize);

            // вычисляем размер области, которая усредняется в одну точку матрицы 8*8
            var deltaX = (int)Math.Floor((double)tile.Bitmap.Width / matrixSize);
            var deltaY = (int)Math.Floor((double)tile.Bitmap.Height / matrixSize);

            // усредняем области, соответствующие каждому из элементов матрицы
            for (var i = 0; i < matrixSize; i++)
            {
                for (var j = 0; j < matrixSize; j++)
                {
                    var yFirst = i * deltaY;
                    var xFirst = j * deltaX;
                    var brightness = tile.Bitmap.CalculateAvgAreaBrightness(xFirst, yFirst, deltaX, deltaY);

                    if (brightness <= brightnessThreshold) // черное
                    {
                        matrix[i, j] = 0;
                        hashBuilder.Append('0');
                    }
                    else // белое
                    {
                        matrix[i, j] = 1;
                        hashBuilder.Append('1');
                    }
                    //                    Console.Write(@"{0} ", matrix[i,j]);
                }
                //                Console.WriteLine();
            }
            return hashBuilder.ToString();
        }
    }
}
