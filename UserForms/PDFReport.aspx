<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PDFReport.aspx.cs" Inherits="MTASS.UserForms.PDFReport" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="../UserControls/AppHeader.ascx" TagName="AppHeader" TagPrefix="uc1" %>
<%@ Register Assembly="ExtendedDropDownList" Namespace="Wendel" TagPrefix="Wendel" %>
<%@ Register Assembly="RealWorld.Grids" Namespace="RealWorld.Grids" TagPrefix="cc1" %>
<%@ Register Assembly="WendelCustomGridView" Namespace="Wendel" TagPrefix="Wendel" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Export PDF Report - MTA Safety Survey</title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0"/>
	<meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="https://code.jquery.com/ui/1.10.4/themes/smoothness/jquery-ui.css" />
    <link href="../CSS/StyleSheet2.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/ie8indexOf.js"></script>
	<script type="text/javascript" src="https://code.jquery.com/jquery-1.12.4.min.js"></script>
    <script type="text/javascript" src="https://code.jquery.com/jquery-migrate-1.1.1.js"></script>
    <script type="text/javascript" src="https://code.jquery.com/ui/1.11.3/jquery-ui.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.equalHeights.js"></script>
    <script language="javascript" type="text/javascript">        
        $(document).ready(function () {
            initAllUI();
            $("body").css({ "min-width": $("#GridView1").width() + 80 });
        });
        function initAllUI() {
            $("#menuContainer").equalHeights();
            fixBottomParentDIV();
        }

        function fixBottomParentDIV() {
            $(".bottomParentDIV").css('margin-top', function (index) {
                return ($(this).parent('div').height() - $(this).height()) + 'px'
            });
        }

        function popNormalWindow(url, winid) {
            //normal window with toolbar / address bar
            var pwin = window.open(url, winid);
            if (pwin) pwin.focus();
        }

        var IsLoading = false;
        function HideLoading() {
            document.getElementById('LoadingAnimation').style.visibility = 'hidden';
            IsLoading = false;
        }
        function ShowLoading() {
            if (IsLoading) {
                alert("Previous web request has not been successfully processed yet.");
                return false;
            } else {
                document.getElementById('LoadingAnimation').style.visibility = 'visible';
                setTimeout('InitAnimation();', 50); //form submit caused animation to stop, need to kick it after form submission
                IsLoading = true;
                return true
            }
        }

        function InitAnimation() {
            var img = document.getElementById('LoadingAnimationImage');
            img.src = "";
            img.src = "../images/Wendel/processing3.gif";
        }

        function showGreyOut() {
            var cvr = document.getElementById("pagegreyout");
            cvr.style.display = "block"
        }

        function hideGreyOut() {
            var cvr = document.getElementById("pagegreyout");
            cvr.style.display = "none"
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" EnableScriptGlobalization="True" />
    <div id="pagegreyout"></div>
    <div style="z-index:2000000; position: absolute; left: 50%; margin-left:-90px; top: 35%; margin-top: -25px; padding: 10px; border: 1px solid black; background: white; visibility: hidden;" id="LoadingAnimation">
        <div style="margin-bottom: 2px; height: 14px;">Loading...</div><img id="LoadingAnimationImage" src="../images/Wendel/processing3.gif" alt="processing" style="border: 0px none transparent;" />
    </div>
    <div style="padding-bottom: 40px;  ">
        <!-- ------------------------------------------------------------------------------------------------
                        Header
            ------------------------------------------------------------------------------------------------   -->
        <div style="z-index:2000000; left: 50%; margin-left: -50px; position: absolute; top: 300px; visibility: hidden;" id="Div1">
            <img id="Img1" src="../images/Wendel/gt_processing.gif" alt="processing" style="border-right: black solid; border-top: black solid; border-left: black solid; border-bottom: black solid;" />
        </div>
        <asp:Panel runat="server" ID="PageHeader" Width="100%" Height="50px" style="position: relative; background-image: url(../Images/Wendel/wendelnobg_small.png); background-repeat: no-repeat; background-color: #DDDDDD;">
            <asp:Image ID="Image1"  runat="server" ImageAlign="Right" ImageUrl="" Visible="false"/>
        </asp:Panel>
        <div style="padding: 0 40px 0 40px;">      
              
            <div class="divPageTitleCenter_small"><span><strong>MTA Safety Survey - Print PDF Report</strong></span></div>
            <div class="divPageSubTitleCenter_small" style="margin: 12px 0 12px 0;"><asp:Label ID="lblBranchInfo" runat="server" Text=""></asp:Label></div>
            <asp:Label runat="server" ID="lbltest"></asp:Label>
            <div id="menuContainer" style="font-size: 1em; font-family: Verdana,Arial,sans-serif; text-align:left; margin-bottom: 14px;">
                <div id="searchContainer" style="float:left;">
                    <div class="gridViewFilterPanel" >
                        <asp:Panel ID="Panel2" runat="server" >
                            <div><strong>Print</strong></div>  
                            <div style="margin-top: 10px;">Year: <asp:TextBox ID="txtYear" runat="server">2019</asp:TextBox></div>
                            <div style="margin-top: 10px;">Location: <Wendel:ExtendedDropDownList ID="ddLocation" runat="server" DataSourceID="SqlDomainLocationFilter" DataValueField="CODE" DataTextField="DESCRIP" AutoPostBack="true" EnableViewState="true"></Wendel:ExtendedDropDownList></div>
                            <div style="margin-top: 10px;">Report Type: <Wendel:ExtendedDropDownList ID="ddReportType" runat="server" DataSourceID="SqlDomainReportType" DataValueField="CODE" DataTextField="DESCRIP" AutoPostBack="true" EnableViewState="true"></Wendel:ExtendedDropDownList></div>
                            <div style="margin-top: 10px;text-align:right;">
                                <asp:Button id="btnPrint" runat="server" 
                                    Text="Print" onclick="btnPrint_Click" /></div>
                            <div style="margin-top: 10px;text-align:right;">
                                <asp:Button id="btnPrintAll" runat="server" 
                                    Text="Print All" onclick="btnPrintAll_Click"/></div>
                        </asp:Panel>
                    </div>
                    <a id="btnHome" href="../Default.aspx"><img src="../Images/SmallMenuIcon/home.png" alt="Home" title="Back to Home Page" class="mainGridCommandButtonImage btn4848" /></a>
                </div>              
                 <asp:SqlDataSource ID="SqlDomainLocationFilter" runat="server"
		            ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		            SelectCommand=" SELECT [BUILDING_ID] as [CODE],[LOCATION] as [DESCRIP]  FROM [Building] ORDER BY LOCATION">
	            </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDomainReportType" runat="server"
		            ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		            SelectCommand="SELECT  [CODE], [DESCRIP] FROM [DomainReportType] ORDER BY [DisplayOrder]">
	            </asp:SqlDataSource>
                <div class="clearplaceholder"></div>
            </div>
        </div>
    </div>

    </form>
</body>
</html>
