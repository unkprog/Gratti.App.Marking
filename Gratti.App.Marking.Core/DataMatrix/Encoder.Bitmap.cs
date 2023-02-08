using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Gratti.App.Marking.Core.DataMatrix
{
    public partial class Encoder
    {
        public static Bitmap EncodeToBitmap(string code, int wh = 50)
        {
            var encoder = new Encoder();
            encoder.Encode(code);

            var columns = encoder.GetColumns();
            var rows = encoder.GetRows();

            var image = new Bitmap(wh, wh, PixelFormat.Format24bppRgb);
          
            var scale = (float)image.Width / columns;

            using (var graphics = Graphics.FromImage(image))
            {
                graphics.Clear(Color.White);
                for (var row = 0; row < rows; row++)
                {
                    for (var column = 0; column < columns; column++)
                    {
                        var colorCode = encoder.GetModule(column, row) == 0
                                            ? Color.White
                                            : Color.Black;

                        graphics.FillRectangle(
                            new SolidBrush(colorCode),
                            new RectangleF(
                            column * scale,
                            row * scale,
                            scale,
                            scale));
                    }
                }
            }
            return image; // ToBitmapFormat24bpp(image);
        }

        private static Bitmap ToBitmapFormat24bpp(Bitmap image)
        {
            //this.dataMatrixImage.bitm
            var result = new Bitmap(image.Size.Width, image.Size.Height, PixelFormat.Format24bppRgb);
            var g = Graphics.FromImage(result);
            g.DrawImage(image, new Rectangle(0, 0, result.Width, result.Height));
            return result;
        }

        public static byte[] EncodeToBytes(string code, int wh = 50)
        {
            return EncodeToBytes(EncodeToBitmap(code, wh));
        }

        public static byte[] EncodeToBytes(Bitmap bmp)
        {
            byte[] result = null, temp = null;
           
            using (var stream = new MemoryStream())
            {
                //stream.Write(new byte[] { 1, 0, 3, 1, 133, 8, 0, 0 }, 0, 8);
                //stream.WriteByte(1);
                //stream.WriteByte(0);
                //stream.WriteByte(0);
                //stream.WriteByte(1);
                //stream.WriteByte(230);
                //stream.WriteByte(29);
                //stream.WriteByte(0);
                //stream.WriteByte(0);
                bmp.Save(stream, ImageFormat.Bmp);
                temp = stream.ToArray();
            }
            result = new byte[temp.Length + 8];
            Array.Copy(new byte[] { 1, 0, 0, 1, 246, 212, 1, 0 }, 0, result, 0, 8);
            Array.Copy(temp, 0, result, 8, temp.Length);
            return result;
        }
    }
}
