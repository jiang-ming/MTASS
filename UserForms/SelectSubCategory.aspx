<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectSubCategory.aspx.cs" Inherits="MTASS.UserForms.SelectSubCategory" %>
<!DOCTYPE html>
<html lang="en-us">
<head runat="server">
	<title>Select Category - MTA S/S</title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0;"/>
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<link rel="stylesheet" type="text/css" href="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css" />
	<link rel="stylesheet" type="text/css" href="../css/custom.css"/>
    <link rel="stylesheet" type="text/css" href="../CSS/override_selectSubcat.css"/>
    <script type="text/javascript" src="../Scripts/ie8indexOf.js"></script>
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
			<asp:HyperLink ID="lnkSelectCategory" data-icon="arrow-l" runat="server" NavigateUrl="<%# GetSelectCatLink() %>">Category</asp:HyperLink>
			<h1><asp:Label ID="lblBuildingName" runat="server" Text=""></asp:Label></h1> 
			<a href="../Default.aspx" data-icon="home">Home</a>
		</div>
		<div data-role="header" data-theme="c">
			<h2><asp:Label ID="lblSection" runat="server" Text=""></asp:Label></h2>
		</div>
		<!-- / header -->
		<!-- content -->
		<div data-role="content">	
			<p class="ui-body ui-body-c message"><strong>Please select feature sub category</strong></p>
			<ul data-role="listview" data-inset="true" data-filter="true" data-filter-theme="false">
                <li data-role="list-divider"><asp:Label ID="lblCategory" runat="server" Text="" CssClass="divider-big-text"></asp:Label></li>
				<asp:Repeater ID="Repeater1" runat="server" DataSourceID="SqlFacility">
					<ItemTemplate>
						<li>
                            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="listbutton-no-ellipsis" Text='<%# GetSubCatetoryLabel(Eval("Sub_Category")) %>' 
                                CommandArgument='<%# getButtonParameter((int)Eval("ID"), (Int16)Eval("DataForm")) %>' OnClick="OnCategoryClick"></asp:LinkButton>
                            <span class="ui-li-count">
                                <asp:Label ID="Label1" runat="server" Text='<%# GetRecordCount(Eval("Sub_Category"),(int)Eval("ct"), (Int16)Eval("DataForm")) %>'></asp:Label>
                            </span>
                        </li>
					</ItemTemplate>
				</asp:Repeater>
			</ul>
		</div>
		<!-- / content -->
	</div>
	<!-- / page -->

    <asp:sqldatasource ID="SqlFacility" runat="server" 
        ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" 
        ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>" 
        SelectCommand="SELECT Q_SubCategory_List.SectionName, Q_SubCategory_List.Main_Category, Q_SubCategory_List.Sub_Category, IIf([Q_Data1_L2_Count].ct Is Null,0,[Q_Data1_L2_Count].ct) AS ct, IIf([Q_Data1_L2_Count_Failed].ct Is Null,0,[Q_Data1_L2_Count_Failed].ct) AS ctfailed, Q_SubCategory_List.DisplayOrder, QBDG.Building_ID, Q_SubCategory_List.DataForm, Q_SubCategory_List.ID FROM (Q_SubCategory_List LEFT JOIN (SELECT * FROM Q_Data1_L2_Count WHERE BUILDING_ID=@BUILDING_ID) AS QBDG ON (Q_SubCategory_List.ID = QBDG.ID)) LEFT JOIN (SELECT * FROM Q_Data1_L2_Count_Failed WHERE BUILDING_ID=@BUILDING_ID) AS QBDG2  ON (Q_SubCategory_List.ID = QBDG2.ID) WHERE Q_SubCategory_List.Main_Category=@MAIN_CATEGORY ORDER BY Q_SubCategory_List.DisplayOrder">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtBuildingID" PropertyName="Text" Type="Int32" Name="BUILDING_ID" />
            <asp:ControlParameter ControlID="txtCategory" PropertyName="Text" Type="String" Name="MAIN_CATEGORY" />
        </SelectParameters>
    </asp:sqldatasource>
    <asp:TextBox ID="txtBuildingID" runat="server" Visible="false">-1</asp:TextBox>
    <asp:TextBox ID="txtCategoryFirstID" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtSection" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtCategory" runat="server" Visible="false"></asp:TextBox>
</form>
</body>
</html>
