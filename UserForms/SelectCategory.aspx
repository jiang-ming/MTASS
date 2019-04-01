<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectCategory.aspx.cs" Inherits="MTASS.UserForms.SelectCategory" EnableViewState="false"  %>
<!DOCTYPE html>
<html lang="en-us">
<head runat="server">
	<title>Select Category - MTA S/S</title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0"/>
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<link rel="stylesheet" href="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css" />
    <link href="../css/custom.css" rel="stylesheet" type="text/css" />
	<link href="../css/override_selectcat.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/ie8indexOf.js" type="text/javascript"></script>
	<script language="javascript" type="text/javascript" src="https://code.jquery.com/jquery-1.8.2.min.js"></script>
	<script language="javascript" type="text/javascript" src="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.js"></script>

	<script language="javascript" type="text/javascript">
	    $(document).ready(function () {
		    // Add attributes to links
		    $("a").not("[href='#']").attr({ 
		      'rel': 'external',
		      'data-transition': 'fade'
		    });
        });

        function loadSubMenu(divid, firstid, _buildingid, _sectionname, _maincategory) {
            if ($("#" + divid).is(":visible")) {
                $("#" + divid).hide();
                $("#" + divid).siblings('.ul-li-image-bubble-container').removeClass('selected-ui-icon');
                $("#" + divid).siblings('.ui-li-count').removeClass('selected-ui-icon');
                $("#" + divid).parent().siblings('.ui-icon').removeClass('selected-ui-icon').removeClass('ui-icon-arrow-u').addClass('ui-icon-arrow-d');
            } else {
                //$('.submenu').hide(); //hide all other sub menu
                //$('.selected-ui-icon').removeClass('selected-ui-icon'); //reset icon override
                $.ajax({
                    url: "SelectCategory.aspx/GenerateSubMenu",
                    type: "post",
                    data: JSON.stringify({ buildingid: _buildingid, section: _sectionname, category: _maincategory }),
                    dataType: "json",
                    contentType: 'application/json',
                    success: function (data) {
                        var content = data.d;
                        $("#" + divid).html(content);
                        $("#" + divid + " ul").listview();
                        $("#" + divid).siblings('.ul-li-image-bubble-container').addClass('selected-ui-icon');
                        $("#" + divid).siblings('.ui-li-count').addClass('selected-ui-icon');
                        $("#" + divid).parent().siblings('.ui-icon').addClass('selected-ui-icon').removeClass('ui-icon-arrow-d').addClass('ui-icon-arrow-u');
                        $("a").not("[href='#']").attr({
                            'rel': 'external',
                            'data-transition': 'fade'
                        });
                    }
                });
                $("#" + divid).show();
            }
        }
        function scroolToDivID(divid) {
            if (!isElementInView("#" + divid)) {
                $('html, body').animate({
                    scrollTop: $("#" + divid).parent().offset().top
                }, 2000);
            }
        }
        function isElementInView(elem) {
            var docViewTop = $(window).scrollTop();
            var docViewBottom = docViewTop + $(window).height();

            var elemTop = $(elem).offset().top;
            var elemBottom = elemTop + $(elem).height();

            return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
        }
	</script>
</head>
<body>
<form id="form1" runat="server" data-ajax="false">
	<!-- page -->
	<div data-role="page" id="Home">
		<!-- header -->
		<div data-role="header"> 
			<a href="SelectBuilding.aspx" data-icon="arrow-l">Building</a>
			<h1>MTA S/S</h1> 
			<a href="../Default.aspx" data-icon="home">Home</a>
		</div>
		<div data-role="header" data-theme="c">
			<h2><asp:Label ID="lblBuildingName" runat="server" Text=""></asp:Label></h2>
		</div>
		<!-- / header -->
		<!-- content -->
		<div data-role="content">	
			<p class="ui-body ui-body-c message"><strong>Please select feature category</strong></p>
            <asp:Literal ID="ltCollapseULBegin" runat="server"></asp:Literal>
				<asp:Repeater ID="Repeater1" runat="server" DataSourceID="SqlFacility">
					<ItemTemplate>
						<asp:Literal ID="Literal1" runat="server" Text='<%# GetDivider(Eval("SectionName")) %>'></asp:Literal>
						<li data-icon="arrow-d">
                        <%-- 
                            // Original Code - (need EnabledViewstate="true")
                            <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# GetCategoryLabel(Eval("Main_Category")) %>' CommandArgument='<%# Eval("FirstOfID") %>' OnClick="OnCategoryClick"></asp:LinkButton>
                            <span class="ui-li-count"><asp:Label ID="Label1" runat="server" Text='<%# Eval("ct") %>'></asp:Label></span>
                            --%>
                            <%-- 
                            // AJAX section (load each individual section on click)
                            --%>
                            <a href="#" onclick="loadSubMenu('<%# GetCategorySubMenuID(Eval("FirstOfID")) %>', <%# Eval("FirstOfID") %>, '<%# txtBuildingID.Text %>', '<%# Eval("SectionName") %>', '<%# Eval("Main_Category") %>' )"><%# GetCategoryLabel(Eval("Main_Category")) %></a>
                            <%# GetErrorBubble( (int)Eval("ctfailed")) %>
                            <span class="ui-li-count" ><asp:Label ID="Label1" runat="server" Text='<%# Eval("ct") %>'></asp:Label></span>
                            <div id='<%# GetCategorySubMenuID(Eval("FirstOfID")) %>' class='submenu' style='display:none; margin: 0 10px 20px 10px;'></div>
                            <%-- 
                            // ASP.NET - generate the entire list on page load
                            <a href="#" onclick="$('#<%# GetCategorySubMenuID(Eval("FirstOfID")) %>').toggle();"><%# GetCategoryLabel(Eval("Main_Category")) %></a>
                            <img class="ul-li-image-bubble" src="../Images/SmallMenuIcon/Exclamation-32.png" alt="failed" title="This category contains at least one failed item" />
                            <span class="ui-li-count" ><asp:Label ID="Label1" runat="server" Text='<%# Eval("ct") %>'></asp:Label></span>
                            <div id='<%# GetCategorySubMenuID(Eval("FirstOfID")) %>' class='submenu' style='display:none; margin: 0 10px 20px 10px;'><%# GenerateSubMenu(txtBuildingID.Text, (String)Eval("SectionName"), (String)Eval("Main_Category"))%></div>
                            --%>
                        </li>
					</ItemTemplate>
				</asp:Repeater>
            <asp:Literal ID="ltCollapseULEnd" runat="server"></asp:Literal>

            <asp:LinkButton ID="DateButton" runat="server" Text="Mark all items as Inspected Today" data-role="button" data-theme="a" OnClick="DateButton_Click"></asp:LinkButton>
		</div>
		<!-- / content -->
	</div>
	<!-- / page -->

    <asp:sqldatasource ID="SqlFacility" runat="server" 
        ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" 
        ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>" 
        SelectCommand="SELECT Q_Category_List.Main_Category, Q_Category_List.SectionName, IIf([Q_Data1_Count].[ct] Is Null,0,[Q_Data1_Count].[ct]) AS ct, IIf([Q_Data1_Count_Failed].[ct] Is Null,0,[Q_Data1_Count_Failed].[ct]) AS ctfailed, Q_Category_List.FirstOfDisplayOrder, QBDG.Building_ID, Q_Category_List.FirstOfID FROM (Q_Category_List LEFT JOIN (Select * from Q_Data1_Count where BUILDING_ID=@BUILDING_ID)  AS QBDG ON Q_Category_List.Main_Category = QBDG.Main_Category) LEFT JOIN (Select * from Q_Data1_Count_Failed where BUILDING_ID=@BUILDING_ID)  AS QBDG2 ON Q_Category_List.Main_Category = QBDG2.Main_Category ORDER BY Q_Category_List.FirstOfDisplayOrder">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtBuildingID" PropertyName="Text" Type="Int32" Name="BUILDING_ID" />
        </SelectParameters>
    </asp:sqldatasource>
    <asp:TextBox ID="txtBuildingID" runat="server" Visible="false">-1</asp:TextBox>
    <asp:TextBox ID="txtSectionExpanded" runat="server" Visible="false"></asp:TextBox>
</form>
</body>
</html>
