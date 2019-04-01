using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using WebMap.Web;

namespace MTASS.UserForms
{
    /// <summary>
    /// Summary description for TransmitImgSrc1
    /// </summary>
    public class TransmitImgSrc : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Boolean filenotfound = false;
            String sFolder = ConfigurationManager.AppSettings[context.Request["wcFolder"].ToWSSafeString()].ToWSSafeString();
            String sFileToSend = sFolder.CheckAppendEnd(@"\") + context.Request["imageID"].ToWSSafeString();
            if (String.IsNullOrWhiteSpace(sFileToSend)|| !(System.IO.File.Exists(sFileToSend)))
            {
                sFileToSend = context.Server.MapPath("../Images/Error/image_not_available.jpg"); //changed from GIF to JPG Jan-2014. Gif does not work with identify thumbnail in silverlight web map
                filenotfound = true;
            }
            String sFileCaption = context.Request["orgFileName"].ToWSSafeString();
            if (String.IsNullOrWhiteSpace(sFileCaption))
            {
                sFileCaption = Path.GetFileName(sFileToSend);
            }
            Boolean bCreateThumbnail = (context.Request["thumb"].ToWSSafeString().ToUpper() == "TRUE");
                        
            context.Response.ContentType = WebMap.Web.StringUtil.GetMimetype(Path.GetExtension(sFileToSend));
            context.Response.AppendHeader("Content-Disposition", "filename=" + sFileCaption);

            FileStream fs = new FileStream(sFileToSend, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] imagebytes = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            MemoryStream stream = new MemoryStream(imagebytes);
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

            if (bCreateThumbnail && !filenotfound) image = reduceImage(image, 200, 150);
            image.Save(context.Response.OutputStream, StringUtil.GetImageFormat(Path.GetExtension(sFileToSend)));
        }

        private System.Drawing.Image reduceImage(System.Drawing.Image image, int maxWidth, int maxHeight)
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}