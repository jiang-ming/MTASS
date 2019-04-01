<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectBuilding.aspx.cs" Inherits="MTASS.UserForms.SelectBuilding" %>
<!DOCTYPE html>
<html lang="en-us">
<head runat="server">
	<title>Select Building - MTA S/S</title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0;"/>
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<link rel="stylesheet" href="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css" />
	<link href="../css/custom.css" rel="stylesheet" type="text/css" />
	<link href="../css/overrides.css" rel="stylesheet" type="text/css" />
	<script src="https://code.jquery.com/jquery-1.8.2.min.js"></script>
	<script src="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.js"></script>
	<script>
	$(document).ready(function () {
		// Add attributes to links
		$("a").attr({ 
		  'rel': 'external',
		  'data-transition': 'fade'
		});
	});
	</script>
</head>
<body>
<form id="form1" runat="server" data-ajax="false">
	<!-- page -->
	<div data-role="page" id="Home">
		<!-- header -->
		<div data-role="header"> 
			<h1>MTA S/S</h1> 
			<a href="../Default.aspx" data-icon="home" class="ui-btn-right">Home</a>
		</div>
		<!-- / header -->
		<!-- content -->
		<div data-role="content">
			<p class="ui-body ui-body-c message"><strong>Please select a building</strong></p>
			<asp:DropDownList ID="ddBuilding" runat="server" DataSourceID="SqlFacility" 
			DataTextField="Location" DataValueField="Building_ID">
			</asp:DropDownList>
			<p>
			<asp:LinkButton ID="btnGo" runat="server" rel="external" data-role="button" data-icon="arrow-r" data-iconpos="right" onclick="btnGo_Click" data-theme="a">Continue</asp:LinkButton>
			</p>
		</div>
		<!-- / content -->	
	</div>
	<!-- / page -->

	<asp:sqldatasource ID="SqlFacility" runat="server" 
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" 
		ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>" 
		SelectCommand="SELECT * FROM [Building] order by [location]">
	</asp:sqldatasource>

</form>
</body>
</html>
