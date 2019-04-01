<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MTASS.Default" %>
<!DOCTYPE html>
<html lang="en-us">
<head runat="server">
	<title>Home - MTA S/S</title>  
	<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0;"/>
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<link rel="stylesheet" href="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css" />
	<link href="css/custom.css" rel="stylesheet" type="text/css" />
	<link href="css/overrides.css" rel="stylesheet" type="text/css" />
	<script src="https://code.jquery.com/jquery-1.8.2.min.js"></script>
	<script src="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.js"></script>
	<script>
	    $(document).ready(function () {
	        // Add attributes to links
	        $("a").attr({
	            'rel': 'external',
	            'data-transition': 'fade'
	        });
	        if (jsHidePrBtn) {
	            $("#btnPrint").hide();
	        } else {
	        }
	    });
	</script>
</head>
<body>
<form id="form1" runat="server">
	<!-- page (Home) -->
	<div data-role="page" id="Home">
		<!-- header -->
		<div data-role="header"> 
			<h1>MTA S/S</h1> 
		</div>
		<!-- / header -->
		<!-- content -->
		<div data-role="content">
			<p>Welcome to the MTA Safety Survey application.  This application allows you to record safety survey result directly into the project database.  Please be advised that you need constant internet connection while you are using this application.  If you work in the basement of a facility with no cell phone reception or other alternative data connection, you will not be able to submit your data back to our server.</p>
			<a href="UserForms/SelectBuilding.aspx" data-role="button" data-icon="grid" data-theme="a">Start Entering Data</a>
			<a href="UserForms/ReportTable.aspx" data-role="button" data-icon="grid" data-theme="a">Data Reviewer Report</a>
			<a href="#About" data-role="button" data-icon="info" data-theme="a">Help</a>
			<asp:LinkButton ID="lnkLogout" runat="server" data-role="button" data-icon="gear" onclick="lnkLogout_Click" data-theme="a">Log Out</asp:LinkButton>
            <a href="UserForms/PDFReport.aspx" data-role="button" data-icon="grid" data-theme="a" id="btnPrint">PDF Report</a>
            <%--<asp:LinkButton ID="lnkReport" runat="server" data-role="button" data-icon="gear" onclick="lnkReport_Click" data-theme="a">PDF Reports</asp:LinkButton>--%>
		</div>
		<!-- / content -->
	</div>
	<!-- / page (Home) -->

	<!-- page (About) -->
	<div data-role="page" id="About">
		<!-- header -->
		<div data-role="header"> 
			<h1>MTA S/S</h1>
			<a href="#Home" data-icon="home" rel="back" data-transition="fade" class="ui-btn-right">Home</a>
		</div>
		<!-- / header -->
		<!-- content -->
		<div data-role="content" >	
            <div style="text-align:center"><img src="Images/wendellogo.jpg" /></div>
			<p>This application is provided by Wendel Companies for the MTA Safety Survey project.  If you need assistance with the program or database, please contact Wendel GIS department through email (<a href="mailto:jneedle@wd-ae.com">JNeedle@wd-ae.com</a>) or phone(<a href="tel:7166880766">716.688.0766</a>)</p>
		</div>
		<!-- / content -->
	</div>
	<!-- page (About) -->

</form>
</body>
</html>
