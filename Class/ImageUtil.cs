using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Configuration;

namespace WebMap.Web
{
    public class ImageUtil
    {
        public static void resizeImage_caller(String filename, String webConfigImagePath, String webConfigReducedImagePath, int maxWidth, int maxHeight)
        {
            String inputFile = ConfigurationManager.AppSettings[webConfigImagePath].CheckAppendEnd(@"\") + filename;
            String outputFile = ConfigurationManager.AppSettings[webConfigReducedImagePath].CheckAppendEnd(@"\") + filename;
            resizeImage(inputFile, outputFile, maxWidth, maxHeight);
        }
        
        public static void resizeImage(String inputFile, String outputFile, int maxWidth, int maxHeight)
        {
            try
            {
                //check output file, delete if already exist
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }

                //read file
                Image image = Image.FromFile(inputFile);

                //calculate height and width
                double width = Convert.ToDouble(image.Width);
                double height = Convert.ToDouble(image.Height);
                int newWidth = 0;
                int newHeight = 0;
                double divisor = 0;
                if (width > height)
                {
                    newWidth = maxWidth;
                    divisor = width / maxWidth;
                    if (divisor == 0)
                    {
                        divisor = 1;
                    }
                    newHeight = Convert.ToInt32(height / divisor);
                }
                else
                {
                    newHeight = maxHeight;
                    divisor = height / maxHeight;
                    if (divisor == 0)
                    {
                        divisor = 1;
                    }
                    newWidth = Convert.ToInt32(width / divisor);
                }

                //create reduced image object
                var newImage = new System.Drawing.Bitmap(newWidth, newHeight);
                System.Drawing.Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

                //rotate image per EXIF Orientation flag
                if (image.PropertyIdList.Contains(0x112)) //0x112=Orientation
                {
                    var prop = image.GetPropertyItem(0x112);
                    if (prop.Type == 3 && prop.Len == 2)
                    {
                        UInt16 orientationExif = BitConverter.ToUInt16(image.GetPropertyItem(0x112).Value, 0);
                        if (orientationExif == 8)
                        {
                            newImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        }
                        else if (orientationExif == 3)
                        {
                            newImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else if (orientationExif == 6)
                        {
                            newImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                    }
                }

                //output to file
                newImage.Save(outputFile, StringUtil.GetImageFormat(Path.GetExtension(outputFile)));

            }
            catch (Exception)
            {
                ErrorEmail.sendEmail(String.Format("Error resizing uploaded image {0} --> {1}", inputFile, outputFile), "error resizing uploaded image");
            }
        }

        public static System.Drawing.Image reduceImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            double width = Convert.ToDouble(image.Width);
            double height = Convert.ToDouble(image.Height);

            int newWidth = 0;
            int newHeight = 0;
            double divisor = 0;
            if (width > height)
            {
                newWidth = maxWidth;
                divisor = width / maxWidth;
                if (divisor == 0)
                {
                    divisor = 1;
                }
                newHeight = Convert.ToInt32(height / divisor);
            }
            else
            {
                newHeight = maxHeight;
                divisor = height / maxHeight;
                if (divisor == 0)
                {
                    divisor = 1;
                }
                newWidth = Convert.ToInt32(width / divisor);
            }


            var newImage = new System.Drawing.Bitmap(newWidth, newHeight);

            System.Drawing.Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }
    }
}