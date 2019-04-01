<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataForm1.aspx.cs" Inherits="MTASS.UserForms.DataForm1" %>
<!DOCTYPE html>
<html lang="en-us">
<head runat="server">
	<title>Assessment Form</title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0;"/>
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<link rel="stylesheet" href="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css" />
	<link href="../css/custom.css" rel="stylesheet" type="text/css" />
	<link href="../css/overrides.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/ie8indexOf.js" type="text/javascript"></script>
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
	<div data-role="page">
		<!-- header -->
		<div data-role="header">
			<asp:HyperLink ID="lnkSelectSubCategory" data-icon="arrow-l" runat="server" NavigateUrl="<%# GetSelectSubCatLink() %>">Category</asp:HyperLink>
			<h1><asp:Label ID="lblBuildingName" runat="server" Text=""></asp:Label></h1>
			<a href="../Default.aspx" data-icon="home">Home</a>
		</div>
		<div data-role="header" data-theme="c">
			<h2><asp:Label ID="lblSection" runat="server" Text=""></asp:Label></h2>
		</div>
		<div data-role="header" data-theme="e">
			<h2><asp:Label ID="lblCategory" runat="server" Text=""></asp:Label></h2>
		</div>
		<!-- / header -->
		<!-- content -->
		<div data-role="content">
            <asp:Literal ID="ltNoData" runat="server"></asp:Literal>
			<p><asp:LinkButton ID="btnAddNew" data-role="button" data-theme="a" runat="server" onclick="btnAddNew_Click">Add New Record</asp:LinkButton></p>
			<asp:Repeater ID="Repeater1" runat="server" DataSourceID="SqlFacility" 
			onitemcommand="Repeater1_ItemCommand" onitemdatabound="Repeater1_ItemDataBound">
				<ItemTemplate>
					<ul data-role="listview" data-theme="c" data-divider-theme="c" data-inset="true">
						<li data-theme="b"><asp:LinkButton ID="LinkButton1" runat="server" Text='<%# GetSubtypeDisplay(Eval("SubTypeDesc"), Eval("Custom_Sub_Type"), Eval("DateCreated")) %>' CommandName="ViewEdit" CommandArgument='<%# Eval("ENTRYUID") %>'></asp:LinkButton></li>
						<li>Component Integrity: <asp:Label ID="Label1" runat="server" Text='<%# DoLookupSqlDomainIntegrity(Eval("Component_Integrity")) %>'></asp:Label></li>
						<li>Condition: <asp:Label ID="Label2" runat="server" Text='<%# DoLookupSqlDomainCondition(Eval("Condition")) %>'></asp:Label></li>
					</ul>
				</ItemTemplate>
			</asp:Repeater>
		</div>
		<!-- / content -->
	</div>
	<!-- / page -->

	<asp:sqldatasource ID="SqlFacility" runat="server" 
	ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" 
	ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>" 
	SelectCommand="SELECT * FROM Q_Data1_Join_TypeName WHERE BUILDING_ID=@BUILDING_ID AND SUB_CATEGORY_ID=@SUBCATID ORDER BY DISPLAYORDER">
	<SelectParameters>
	    <asp:ControlParameter ControlID="txtBuildingID" PropertyName="Text" Type="Int32" Name="BUILDING_ID" />
	    <asp:ControlParameter ControlID="txtSubCatID" PropertyName="Text" Type="Int32" Name="SUBCATID" />
	</SelectParameters>

	</asp:sqldatasource>
	<asp:SqlDataSource ID="SqlDomainIntegrity" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainIntegrity] ORDER BY [DisplayOrder]">
	</asp:SqlDataSource>
	<asp:SqlDataSource ID="SqlDomainCondition" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainCondition] ORDER BY [DisplayOrder]">
	</asp:SqlDataSource>

	<asp:TextBox ID="txtBuildingID" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtSection" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtCategory" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtSubCategory" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtSubCatID" runat="server" Visible="false"></asp:TextBox>
</form>
</body>
</html>
