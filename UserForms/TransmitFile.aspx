<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransmitFile.aspx.cs" Inherits="MTASS.UserForms.TransmitFile" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>File Download</title>
    <link href="../CSS/StyleSheet.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        a, a:visited, a:hover, a:active
        {
	        color:#0088FF;
        }
    </style>
</head>
<body onload="SetDownloadTimer()">
    <form id="form1" runat="server">
    <div>
        <asp:Panel ID="pnlPageTitleCaption" runat="server" SkinID="SectionPanel" Width="100%" Height="16px">
            <asp:Label ID="lblPageTitleCaption" runat="server" Font-Bold="True" Font-Size="16px" >File Download</asp:Label>
        </asp:Panel>
        <div>&nbsp;</div>
        <div style="font-weight: bold">Your file download will start in <span id='countdown'>a few</span> seconds...</div>
        <div>&nbsp;</div>
        <div><asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click" >Please click this direct download link if automatic file download did not start.</asp:LinkButton></div>
        <script language="javascript" type="text/javascript">
            var linkButtonClientID = '<%=LinkButton1.ClientID%>';
            var linkButton = document.getElementById(linkButtonClientID);

            var countdownElement = document.getElementById('countdown');
            var downloadtimer = 4;
            var interval;

            function SetDownloadTimer() {
                interval = setInterval(function () {
                    downloadtimer--;
                    countdownElement.innerHTML = downloadtimer.toString();
                    if (downloadtimer == 0) {
                        clearInterval(interval);
                        StartDownlad();
                    }
                }, 1000);
            }

            function StartDownlad() {
                linkButton.click();
            }
        </script>
    </div>
    </form>
</body>
</html>
