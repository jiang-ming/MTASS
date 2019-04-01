using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMap.Web
{
    public static class SiteAccessHelper
    {
        public static string CreatePhotoThumbnail(String strFileName, String strCaption, String m_AttachmentPath, String sWebConfigFolder, Boolean thumb120 = true, String thumbCssClass = "thumb120", 
                                                    String imageWebService = "../TransmitImgSrc.ashx", String downloadHandler = "../TransmitFile.aspx", String pathToImageBase = "../../images")
        {
            string sR = "";
            string strDisplayText = "";
            string sFileExt = String.Empty; try { sFileExt = System.IO.Path.GetExtension(strFileName).ToUpper(); }
            catch { }
            string sessionid; try { sessionid = HttpContext.Current.Session["SID"].ToWSSafeString(); }
            catch { sessionid = "0"; }
            string username; try { username = PermissionHelper.GetUserName(); }
            catch { username = ""; }
            switch (sFileExt)
            {
                case ".JPG":
                case ".JPEG":
                case ".BMP":
                case ".PNG":
                case ".GIF":
                    String strThumb120URL = thumb120 ? "&thumb120=true" : "";
                    String strClass = !String.IsNullOrWhiteSpace(thumbCssClass) ? String.Format("class=\"{0}\"", thumbCssClass) : "";
                    if (m_AttachmentPath.StartsWith("\\\\"))
                    {
                        strDisplayText = String.Format("<img src=\"{0}?wcFolder={1}&imageID={2}&orgFileName={3}&thumb=true{5}&sessionid={7}&user{8}\" alt=\"{3}\"  {6} border=\"0\" onMouseOver=\"window.status='{4}';return true;\" onMouseOut=\"window.status='';return true;\" >", 
                            imageWebService, sWebConfigFolder, strFileName, strCaption, strCaption.Replace(" ", ""), strThumb120URL, strClass, sessionid, username);
                    }
                    else
                    {
                        strDisplayText = String.Format("<img src=\"{0}\" alt=\"{1}\" class=\"thumb120\" border=\"0\" onMouseOver=\"window.status='{2}';return true;\" onMouseOut=\"window.status='';return true;\">", m_AttachmentPath.CheckAppendEnd("/") + strFileName, strCaption, strCaption.Replace(" ", ""));
                    }
                    break;
                case ".WMA":
                case ".MP3":
                case ".WAV":
                case ".AC3":
                case ".AAC":
                case ".M4A":
                case ".M4P":
                    strDisplayText = string.Format("<img alt=\"{0}\" title=\"Click to View/Download\" src=\"{2}/Wendel/Sound.png\" border=\"0\" onMouseOver=\"window.status='{1}';return true;\" onMouseOut=\"window.status='';return true;\">", strCaption, strCaption.Replace(" ", ""), pathToImageBase);
                    break;
                case ".PDF":
                    strDisplayText = string.Format("<img alt=\"{0}\" title=\"Click to View/Download\" src=\"{2}/FileType/File-pdf-48.png\" border=\"0\" onMouseOver=\"window.status='{1}';return true;\" onMouseOut=\"window.status='';return true;\">", strCaption, strCaption.Replace(" ", ""), pathToImageBase);
                    break;
                case ".XLS":
                case ".XLSX":
                case ".XLSM":
                    strDisplayText = string.Format("<img alt=\"{0}\" title=\"Click to View/Download\" src=\"{2}/FileType/File-Excel-48.png\" border=\"0\" onMouseOver=\"window.status='{1}';return true;\" onMouseOut=\"window.status='';return true;\">", strCaption, strCaption.Replace(" ", ""), pathToImageBase);
                    break;
                case ".DOC":
                case ".DOCX":
                    strDisplayText = string.Format("<img alt=\"{0}\" title=\"Click to View/Download\" src=\"{2}/FileType/Word-48.png\" border=\"0\" onMouseOver=\"window.status='{1}';return true;\" onMouseOut=\"window.status='';return true;\">", strCaption, strCaption.Replace(" ", ""), pathToImageBase);
                    break;
                default:
                    strDisplayText = strFileName;
                    break;
            }

            string sFullLinkPathName = string.Empty;
            if (m_AttachmentPath.StartsWith("\\\\"))
            {
                switch (sFileExt)
                {
                    case ".JPG":
                    case ".JPEG":
                    case ".GIF":
                    case ".BMP":
                    case ".PNG":
                    case ".TIF":
                        //use ashx handler to push byte stream
                        sFullLinkPathName = string.Format("{0}?wcFolder={1}&imageID={2}&orgFileName={3}&sessionid={4}&user{5}", imageWebService, sWebConfigFolder, strFileName, strCaption, sessionid, username );
                        break;
                    default:
                        //use transmit file
                        int iFileCount = (new WendelWMA.WMA_Utility.SetupTransmitFile()).SetupNextSendFile(System.IO.Path.Combine(m_AttachmentPath, strFileName));
                        sFullLinkPathName = string.Format("{0}?FILE={1}", downloadHandler, iFileCount);
                        break;
                }
            }
            else
            {
                sFullLinkPathName = m_AttachmentPath.CheckAppendEnd("/") + strFileName;
            }

            sR = string.Format("<a alt=\"{0}\" href=\"{1}\" target=\"_new\" onMouseOver=\"window.status='{3}';return true;\" onMouseOut=\"window.status='';return true;\">{2}</a>", strCaption, sFullLinkPathName, strDisplayText, strCaption.Replace(" ", ""));
            return sR;

        }

        public static string CreateImgSrc(String strFileName, String strCaption, String m_AttachmentPath, String sWebConfigFolder, Boolean thumb120 = true,
                                                   String imageWebService = "../TransmitImgSrc.ashx")
        {
            string strURL = "";
            string sFileExt = String.Empty; try { sFileExt = System.IO.Path.GetExtension(strFileName).ToUpper(); }
            catch { }
            String strThumb120URL = thumb120 ? "&thumb=true&thumb120=true" : "";
            switch (sFileExt)
            {
                case ".JPG":
                case ".JPEG":
                case ".BMP":
                case ".PNG":
                case ".GIF":
                    if (m_AttachmentPath.StartsWith("\\\\"))
                    {
                        strURL = String.Format("{0}?wcFolder={1}&imageID={2}&orgFileName={3}{4}",
                            imageWebService, sWebConfigFolder, strFileName, strCaption, strThumb120URL);
                    }
                    else
                    {
                        strURL = String.Format("{0}", m_AttachmentPath.CheckAppendEnd("/") + strFileName);
                    }
                    break;
                default:
                        strURL = String.Format("{0}?wcFolder={1}&imageID={2}&orgFileName={3}{4}",
                            imageWebService, sWebConfigFolder, "ERRORIMAGENOTAVAILABLE.JPG", strCaption, strThumb120URL);
                    break;
            }

            return strURL;
        }

        public static bool HasSiteAccessImage(this System.Web.UI.WebControls.Label lbl)
        {
            return lbl.Text.ToUpper().Contains("orgFileName".ToUpper());
        }

    }
}