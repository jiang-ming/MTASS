<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportTable.aspx.cs" Inherits="MTASS.UserForms.ReportTable" EnableEventValidation="false"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="../UserControls/AppHeader.ascx" TagName="AppHeader" TagPrefix="uc1" %>
<%@ Register Assembly="ExtendedDropDownList" Namespace="Wendel" TagPrefix="Wendel" %>
<%@ Register Assembly="RealWorld.Grids" Namespace="RealWorld.Grids" TagPrefix="cc1" %>
<%@ Register Assembly="WendelCustomGridView" Namespace="Wendel" TagPrefix="Wendel" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Data Reviewer Report - MTA Safety Survey</title>
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
            document.getElementById('LoadingAnimation').style.visibility='hidden';
            IsLoading = false;
        }
        function ShowLoading() {
            if (IsLoading) {
                alert("Previous web request has not been successfully processed yet.");
                return false;
            } else {
                document.getElementById('LoadingAnimation').style.visibility='visible';
                setTimeout('InitAnimation();',50); //form submit caused animation to stop, need to kick it after form submission
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

        function ValidateDeleteAction() {
            //Don't forget to add div pagegreyout within the BODY tag
            if (jsIsEditEnabled) {
                showGreyOut();
                var banswer = (prompt('Please type the word DELETE into the textbox to confirm deletion', '') == 'DELETE');
                hideGreyOut();
                return banswer;
            }
            else {
                return false
            };
        }
        function initialColSwitchdialog() {
            var vcol = "";
            $('#dialogColSwitch').dialog({
                closeOnEscape: false,
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                buttons: {
                    'Apply': function () {
                        vcol = getcheckedCol();
                        $('#txtCurrentCols').val(vcol);
                        $('#btnPB').trigger('click'); //startpostback
                        //                     Ajax didn't work, because ajax function in code behind, can only be static.
                        $("#dialogColSwitch").parent().hide();

                    }
                }
            });
        }
        function getcheckedCol() {
            var vColNames = [];
            $.each($("#ckblCol input:checkbox:checked"), function (i, val) {
                vColNames[vColNames.length] = $("label[for=" + val.name.replace("$", "_") + "]")[0].innerHTML;

            })
            return vColNames.join("|");
        }
        $(function () {
            initialColSwitchdialog();
            $("#dialogColSwitch").parent().hide();
            vcol = getcheckedCol();
            $('#txtCurrentCols').val(vcol);
            setTableWidth();
        });
        function showDialogCol() {
            $("#dialogColSwitch").parent().show();
        }
        function hideDialogCol() {
            $("#dialogColSwitch").parent().hide();
        }
        function setTableWidth() {
            var strOpenCol = $('#txtCurrentCols').val().split('|');
            var intWidth = 0;
            strOpenCol.forEach(function(ele){
                intWidth+=colWidths[ele];
            });
            if (intWidth > 0) {
                $("#GridView1").css("width", intWidth + 150);
                $('body').css('min-width',intWidth+250);
            }
        }
        //this variable only used for calculating total width. is not used in actual width of each column. 
        var colWidths = {
            "Location": 100,
            "Component": 100,
            "Applicable Code": 100,
            "Responsible Party": 100,
            "Survey Item": 100,
            "Citation - Classification": 100,
            "Description of Finding": 250,
            "Expected Completion Date": 100,
            "Priority": 100,
            "Condition": 100,
            "Picture1": 200,
            "Picture2": 200,
            "Picture3": 200,
            "MTA Response": 100,
            "Current Status": 100,
            "Variance": 100,
            "Variance Description": 100,
            "In/Out Contract": 100,
            "Compliance Plan": 100,
            "Cost": 100,
            "Complete By": 100,
            "Contractor Notes": 100,
            "Date Inspected": 100,
            "Corrective Actions Required": 100,
            "Link": 100
        };
        
    </script>
    <style type="text/css">
        #GridView1 
        {
            table-layout:fixed;
        }
        
    </style>
</head>
<body class="linkcolor000066" style="font-size: small;">
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
                <div class="divPageTitleCenter_small"><span><strong>MTA Safety Survey - Data Review</strong></span></div>
                <div class="divPageSubTitleCenter_small" style="margin: 12px 0 12px 0;"><asp:Label ID="lblBranchInfo" runat="server" Text=""></asp:Label></div>
                <div id="menuContainer" style="font-size: 1em; font-family: Verdana,Arial,sans-serif; text-align:left; margin-bottom: 14px;">
                    <div id="searchContainer" style="float:left;">
                        <div class="gridViewFilterPanel" >
                            <asp:Panel ID="Panel2" runat="server" >
                                <div><strong>Filter</strong></div>  
                                <div style="margin-top: 10px;">Location: <Wendel:ExtendedDropDownList ID="ddLocation" runat="server" DataSourceID="SqlDomainLocationFilter" DataValueField="CODE" DataTextField="DESCRIP" OnSelectedIndexChanged="ddFitlers_SelectedIndexChanged" AutoPostBack="true" EnableViewState="true"></Wendel:ExtendedDropDownList></div>
                                <div style="margin-top: 10px;">Component Integrity: <Wendel:ExtendedDropDownList ID="ddCI" runat="server" DataSourceID="SqlDomainIntegrityFilter" DataValueField="CODE" DataTextField="DESCRIP" OnSelectedIndexChanged="ddFitlers_SelectedIndexChanged" AutoPostBack="true" EnableViewState="true"></Wendel:ExtendedDropDownList></div>
                                <div style="margin-top: 10px;">With Corrective Actions: 
                                    <Wendel:ExtendedDropDownList ID="ddCAR" runat="server" OnSelectedIndexChanged="ddFitlers_SelectedIndexChanged" AutoPostBack="true" EnableViewState="true">
                                        <asp:ListItem>View All</asp:ListItem>
                                        <asp:ListItem>Yes</asp:ListItem>
                                        <asp:ListItem>No</asp:ListItem>
                                    </Wendel:ExtendedDropDownList></div>
                            </asp:Panel>
                        </div>
                    </div>              
                    <div id="divOuterButtonContainer" style="float:left; position: relative;">
                        <div id="divInnerButtonContainer" style="border: 0px solid transparent;" class="bottomParentDIV">                   
                            <a id="btnSwitch" onclick="showDialogCol();"><img src="../Images/SmallMenuIcon/showhide.png" alt="Show/Hide Columns" title="Show/Hide Columns" class="mainGridCommandButtonImage btn4848" /></a>
                            <asp:LinkButton ID="btnXLS"  runat="Server" CausesValidation="False" OnClick="btnXLS_Click" OnClientClick="setTimeout('HideLoading();',5000)"><img src="../Images/FileType/File-Excel-48.png" alt="Export to Excel" title="Export to Excel" class="mainGridCommandButtonImage btn4848" /></asp:LinkButton>
                            <a id="btnHome" href="../Default.aspx"><img src="../Images/SmallMenuIcon/home.png" alt="Home" title="Back to Home Page" class="mainGridCommandButtonImage btn4848" /></a>
                        </div>
                    </div>
                    <div id="divRecordCounter" style="float:right; text-align: right;"><div id="divInnerRecordCounterContainer" class="bottomParentDIV"><asp:Label ID="lblRecordCount" runat="server" Font-Bold="true" ForeColor="Black"></asp:Label></div></div>
                    <div class="clearplaceholder"></div>
                </div>
                <Wendel:PagingGridView ID="GridView1" runat="server" SkinID="noskin" Width="3000px"
                    BackColor="White" BorderColor="#000066" BorderStyle="Solid" 
                    BorderWidth="1px" CellPadding="4"
                    DataSourceID="SqlFacility"  DataKeyNames="ENTRYUID" PageSize="50" 
                    AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True" 
                    PagerSettings-Mode="NumericFirstLast" CssClass="gridview_rounded_corner gridview_width_percent_border_fix"
                    OnDataBound="GridView1_DataBound" OnRowDataBound="GridView1_RowDataBound" 
                    OnPageIndexChanged="GridView1_PageIndexChanged" 
                    OnSorting="GridView1_Sorting" PagerType="Custom" >
                    <RowStyle ForeColor="#000066" VerticalAlign="Top" BorderColor="#000066" BorderWidth="1px" BorderStyle="Solid" />
                    <AlternatingRowStyle BackColor="#F0F0F0" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#8da4b9" Font-Bold="True" ForeColor="Black" />

                    <PagerSettings Mode="NumericFirstLast"></PagerSettings>

                    <PagerStyle BackColor="#E0F4FF" ForeColor="#000066" HorizontalAlign="Left" Font-Overline="False" Font-Underline="True" />
                    <FooterStyle BackColor="#E0F4FF" ForeColor="#000066" />
                    <EmptyDataTemplate>
                        <div>There are no data records matching your criteria.</div>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField DataField="Location" HeaderText="Location" SortExpression="Location" />
                        <asp:TemplateField HeaderText="Component" SortExpression="SectionName, Main_Category" >
                            <ItemTemplate>
                                <asp:Label ID="lblComponent" runat="server" Text='<%# Eval("SectionName") + " - " + Eval("Main_Category") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Survey Item" DataField="Sub_Category" SortExpression="Sub_Category" />
                        <asp:TemplateField HeaderText="Citation / Classification" SortExpression="Component_Integrity" >
                            <ItemTemplate>
                                <%# GetFlagIcon(Eval("Component_Integrity_Code"))%>
                                <asp:Label ID="lblID" runat="server" Text='<%# Eval("Component_Integrity")  %>'></asp:Label>
                                <asp:Label ID="lbldash" runat="server" Text=' - ' Visible='<%# gvstr(Eval("ClassificationShort"))!="N/A" %>'></asp:Label>
                                <u><asp:Label ID="lblCls" runat="server" Text='<%#  Eval("ClassificationShort") %>'  ToolTip='<%# Eval("Classification") %>' Visible='<%# gvstr(Eval("ClassificationShort"))!="N/A" %>'></asp:Label></u>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ApplicableCode" HeaderText="Applicable Code" SortExpression="ApplicableCode" />
                        <asp:BoundField DataField="ResponsibleParty" HeaderText="Responsible Party" SortExpression="ResponsibleParty" />
                        <asp:TemplateField HeaderText="Description of Finding" SortExpression="Comments1"  >                            
                            <ItemTemplate>
                                <div style="overflow-y:auto; height:150px">
                                    <asp:Label ID="lblComments"  runat="server" Text='<%# Eval("Comments1") + " " + Eval("Comments2") %>'></asp:Label>
                                </div>
                            </ItemTemplate>
                            <HeaderStyle Width="240px" />
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Expected Completion Date" SortExpression= "ExpectedCompletionDate" Visible="false" >
							<ItemTemplate>
								<asp:Label ID="lblExpectedCompletionDate" runat="server" text='<%# Eval("ExpectedCompletionDate","{0:d}") %>'></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
                        <asp:BoundField DataField="Priority" HeaderText="Priority" SortExpression="Priority" Visible="false" />
                        <asp:BoundField DataField="Condition" HeaderText="Condition" SortExpression="Condition" Visible="false" />
                        <%--<asp:BoundField DataField="Classification" HeaderText="Classification" SortExpression="Classification"/>--%>
                        <asp:TemplateField HeaderText="Picture1"  SortExpression="Picture1" >
                            <ItemTemplate>
                                <asp:Label ID="lblPicture1" runat="server" Text='<%# CreateLinkThumbnail(Eval("Picture1"), Eval("Picture1")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Picture2"  SortExpression="Picture2" >
                            <ItemTemplate>
                                <asp:Label ID="lblPicture2" runat="server" Text='<%# CreateLinkThumbnail(Eval("Picture2"), Eval("Picture2")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Picture3"  SortExpression="Picture3" >
                            <ItemTemplate>
                                <asp:Label ID="lblPicture3" runat="server" Text='<%# CreateLinkThumbnail(Eval("Picture3"), Eval("Picture3")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="200px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="MTA_Response" HeaderText="MTA Response" SortExpression="MTA_Response"/>
                        <asp:BoundField DataField="Status" HeaderText="Current Status" SortExpression="Status" Visible="false" />
						<asp:TemplateField HeaderText="Variance" SortExpression="VARIANCE" Visible="false"   >
							<ItemTemplate>
								<asp:Label ID="lblVARIANCE" runat="server" Text='<%# Eval("VARIANCE") %>'></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Variance Description" SortExpression="VARIANCE_DESC"  Visible="false"  >
							<ItemTemplate>
								<asp:Label ID="vtxtVARIANCE_DESC" runat="server" text='<%# Eval("VARIANCE_DESC") %>'></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="In/Out Contract" SortExpression="INOUT"  Visible="false"  >
							<ItemTemplate>
								<asp:Label ID="lblINOUT" runat="server" Text='<%# Eval("INOUT") %>'></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Compliance Plan" SortExpression="COMPLIANCE_PLAN"  Visible="false"  >
							<ItemTemplate>
								<asp:Label ID="vtxtCOMPLIANCE_PLAN" runat="server" text='<%# Eval("COMPLIANCE_PLAN") %>' ></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Cost" SortExpression="COST"  Visible="false"  >
							<ItemTemplate>
								<asp:Label ID="lblCOST" runat="server" Text='<%# Eval("COST") %>'></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Complete By" SortExpression="COMPLETE_BY"  Visible="false"  >
							<ItemTemplate>
								<asp:Label ID="lblCOMPLETE_BY" runat="server" Text='<%# Eval("COMPLETE_BY","{0:d}") %>'></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Contractor Notes" SortExpression="ContractorNotes"  >
							<ItemTemplate>
								<asp:Label ID="vtxtContractorNotes" runat="server" text='<%# Eval("ContractorNotes") %>'></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Date Inspected" SortExpression= "INSP2017">
							<ItemTemplate>
								<asp:Label ID="lbldate" runat="server" text='<%# Eval("INSP2017","{0:d}") %>'></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
                        <asp:BoundField DataField="CorrectiveActionsRequired" HeaderText="Corrective Actions Required" SortExpression="CorrectiveActionsRequired"  Visible="false"  />
                        <asp:TemplateField HeaderText="Link" SortExpression="ENTRYUID">
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkForm" runat="server" NavigateUrl='<%# GetFormUrl(Eval("ENTRYUID"), Eval("BUILDING_ID"), Eval("SUB_CATEGORY_ID")) %>' Target="_blank"><img src="../Images/SmallMenuIcon/report-32.png" alt="Form" title="View/Edit" style="border:0;" /></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </Wendel:PagingGridView>
            </div>

            <div style="margin:0;"><img src="../Images/blank.gif" alt="place holder" style="width:1px;height:20px;" /></div>
        </div> <!-- / panel leftmargin -->
    
        <%-- ------------------------------------------------------------------------------------------------
            SQL control parameters / hidden control
            -------------------------------------------------------------------------------------------------  --%>
	    <asp:sqldatasource ID="SqlFacility" runat="server" 
		    ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" 
		    ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>" 
		    SelectCommand="SELECT * FROM R_DATA1_Report ORDER BY LOCATION, DISPLAYORDER"
            OnSelected="SqlFacility_Selected" >
	    </asp:sqldatasource>

	    <asp:SqlDataSource ID="SqlDomainLocationFilter" runat="server"
		    ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		    SelectCommand="SELECT '*' AS [CODE], ' ANY' AS [DESCRIP], ' ANY' AS [DISPLAYORDER] FROM [Building] UNION SELECT [BUILDING_ID] as [CODE],[LOCATION] as [DESCRIP],[LOCATION] AS [DisplayOrder] FROM [Building] ORDER BY [DisplayOrder]">
	    </asp:SqlDataSource>
	    <asp:SqlDataSource ID="SqlDomainIntegrityFilter" runat="server"
		    ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		    SelectCommand="SELECT '*' AS [CODE], ' ANY' AS [DESCRIP], ' ANY' AS [DISPLAYORDER] FROM [DomainIntegrity] UNION SELECT [CODE],[DESCRIP],[DisplayOrder] FROM [DomainIntegrity] UNION SELECT '-1' AS [CODE], 'Fail / CV / NIA' AS [DESCRIP], '9999' AS [DISPLAYORDER] FROM [DomainIntegrity] ORDER BY [DisplayOrder]">
	    </asp:SqlDataSource>
        <asp:Button ID="btnPB" style="display:none" runat="server" onClick="btnPB_Click"/>
        <asp:TextBox ID="txtCurrentCols" runat="server" style="display:none"></asp:TextBox>
    </form>
    
    <div id="dialogColSwitch" title="Show/Hide Columns" style="display:none;" >
        <asp:CheckBoxList ID="ckblCol" runat="server" >
        </asp:CheckBoxList>
    </div>

</body>
</html>
