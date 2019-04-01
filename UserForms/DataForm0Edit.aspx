<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataForm0Edit.aspx.cs" Inherits="MTASS.UserForms.DataForm0Edit" %>
<%@ Register Assembly="ExtendedDropDownList" Namespace="Wendel" TagPrefix="Wendel" %>
<%@ Register Src="../UserControls/ReplaceImageAjax.ascx" TagName="ReplaceImageAjax" TagPrefix="uc1" %>
<!DOCTYPE html>
<html lang="en-us">
<head runat="server">
	<title>Assessment Form - MTA S/S</title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0;"/>
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<link rel="stylesheet" href="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css" />
	<link href="../css/custom.css" rel="stylesheet" type="text/css" />
	<link href="../css/overrides.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/ie8indexOf.js"></script>
	<script type="text/javascript" src="https://code.jquery.com/jquery-1.8.2.min.js"></script>
	<script type="text/javascript" src="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.js"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
		// Add attributes to links
		    $("a").not('.popup-btn').attr({
		      'rel': 'external',
		      'data-transition': 'fade'
		    });
	    });

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
                var banswer = false;
                showGreyOut();
                banswer = (prompt('Please type the word DELETE into the textbox to confirm deletion', '') == 'DELETE');
                hideGreyOut();
                return banswer;
            }
            else {
                return false
            };
        }
        function ValidateDeletePhotoAction() {
            //Don't forget to add div pagegreyout within the BODY tag
            if (jsIsEditEnabled) {
                var banswer = false;
                showGreyOut();
                banswer = (confirm('Are you sure you want to delete this photo?'));
                hideGreyOut();
                return banswer;
            }
            else {
                return false
            };
        }
	</script>
</head>
<body>
<form id="form1" runat="server" data-ajax="false">
	<asp:ToolkitScriptManager ID="ScriptManager1" runat="server" ></asp:ToolkitScriptManager>
    <div id="pagegreyout"></div>
	<!-- page -->
	<div data-role="page">
		<!-- header -->
		<div data-role="header">
			<asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="<%# GetBackLink() %>" Text="<%# GetBackLinkCaption() %>" data-icon="arrow-l"></asp:HyperLink>
			<h1><asp:Label ID="lblBuildingName" runat="server" Text=""></asp:Label></h1>
			<a href="../Default.aspx" rel="external" data-icon="home">Home</a>
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
			<p class="ui-body ui-body-c message"><strong><asp:Label ID="lblMessage" runat="server" Text=""></asp:Label></strong></p>
		
			<p class="ui-body ui-body-a message2"><strong><asp:Label ID="lblSubcategory" runat="server" Text=""></asp:Label></strong></p>

			<asp:DetailsView ID="DetailsView1" runat="server" 
			    DataSourceID="SqlFacility" AutoGenerateRows="False" 
			    DataKeyNames="ENTRYUID" onitemcommand="DetailsView1_ItemCommand"
			    oniteminserted="DetailsView1_ItemInserted" BorderStyle="None" GridLines="None" 
                onitemupdated="DetailsView1_ItemUpdated" 
                OnModeChanging="DetailsView1_ModeChanging" 
                onmodechanged="DetailsView1_ModeChanged">
				<Fields>
					<asp:TemplateField ShowHeader="false">
						<ItemTemplate>
                            <asp:LinkButton ID="EditButton2" runat="server" CommandName="Edit" Text="Edit" data-role="button" data-theme="a"></asp:LinkButton>
						</ItemTemplate>
						<EditItemTemplate>
                            <asp:LinkButton ID="UpdateButton2" runat="server" CommandName="Update" Text="Save" data-role="button" data-theme="a"></asp:LinkButton>
                            <asp:LinkButton ID="CancelButton2" runat="server" CommandName="Cancel" Text="Cancel" data-role="button" data-theme="c"></asp:LinkButton>
						</EditItemTemplate>
						<InsertItemTemplate>
                            <asp:LinkButton ID="InsertButton2" runat="server" CommandName="Insert" Text="Add" data-role="button" data-theme="a"></asp:LinkButton>
                            <asp:LinkButton ID="CancelButton2" runat="server" CommandName="Cancel" Text="Cancel" data-role="button" data-theme="c"></asp:LinkButton>
						</InsertItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="ENTRYUID" HeaderText="ENTRYUID" Visible="false" InsertVisible="False" ReadOnly="True" SortExpression="ENTRYUID" />
					<asp:BoundField DataField="Building_ID" HeaderText="Building_ID" Visible="false" SortExpression="Building_ID" />
					<asp:TemplateField ShowHeader="false">
						<ItemTemplate>
                            <div id="hideFieldsContainer" <%# GetHideFieldInjector() %>>
								<h3 class="ui-bar-b">General Information</h3>
								<div class="ui-grid-a">
									<div class="ui-block-a">Manufacturer</div>
									<div class="ui-block-b">
										<asp:Label ID="lblManufacturer" runat="server" Text='<%# Eval("Manufacturer") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Model Number</div>
									<div class="ui-block-b">
										<asp:Label ID="lblModel_Number" runat="server" Text='<%# Eval("Model_Number") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Part Number</div>
									<div class="ui-block-b">
										<asp:Label ID="lblPart_Number" runat="server" Text='<%# Eval("Part_Number") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Service Size</div>
									<div class="ui-block-b">
										<asp:Label ID="lblService_Size" runat="server" Text='<%# Eval("Service_Size") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Capacity</div>
									<div class="ui-block-b">
										<asp:Label ID="lblCapacity" runat="server" Text='<%# Eval("Capacity") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">SCFM</div>
									<div class="ui-block-b">
										<asp:Label ID="lblSCFM" runat="server" Text='<%# Eval("SCFM") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Pressure Rating</div>
									<div class="ui-block-b">
										<asp:Label ID="lblPressure_Rating" runat="server" Text='<%# Eval("Pressure_Rating") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Temperature Rating</div>
									<div class="ui-block-b">
										<asp:Label ID="lblTemperature_Rating" runat="server" Text='<%# Eval("Temperature_Rating") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Current Pressure</div>
									<div class="ui-block-b">
										<asp:Label ID="lblCurrent_Pressure" runat="server" Text='<%# Eval("Current_Pressure") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Current Temperature</div>
									<div class="ui-block-b">
										<asp:Label ID="lblCurrent_Temperature" runat="server" Text='<%# Eval("Current_Temperature") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Material</div>
									<div class="ui-block-b">
										<asp:Label ID="lblMaterial" runat="server" Text='<%# Eval("Material") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Number of Stages</div>
									<div class="ui-block-b">
										<asp:Label ID="lblNumber_of_Stages" runat="server" Text='<%# DoLookupSqlDomainNumStage(Eval("Number_of_Stages")) %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Fire suppression system type</div>
									<div class="ui-block-b">
										<asp:Label ID="lblFire_suppression_system_type" runat="server" Text='<%# Eval("Fire_suppression_system_type") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Equipment Location</div>
									<div class="ui-block-b">
										<asp:Label ID="lblEquipment_Location" runat="server" Text='<%# Eval("Equipment_Location") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Expiration Date</div>
									<div class="ui-block-b">
										<asp:Label ID="lblExpiration_Date" runat="server" Text='<%# Eval("Expiration_Date","{0:d}") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Special Permit Number</div>
									<div class="ui-block-b">
										<asp:Label ID="lblSpecial_Permit_Number" runat="server" Text='<%# Eval("Special_Permit_Number") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Quantity of nozzles/connectors</div>
									<div class="ui-block-b">
										<asp:Label ID="lblQuantity_of_nozzles_connectors" runat="server" Text='<%# DoLookupSqlDomainNumNozzle(Eval("Quantity_of_nozzles_connectors")) %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Defueling Type</div>
									<div class="ui-block-b">
										<asp:Label ID="lblDefueling_Type" runat="server" Text='<%# DoLookupSqlDomainDeFuel(Eval("Defueling_Type")) %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Associated Equipment</div>
									<div class="ui-block-b">
										<asp:Label ID="lblAssociated_Equipment" runat="server" Text='<%# Eval("Associated_Equipment") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Voltage</div>
									<div class="ui-block-b">
										<asp:Label ID="lblVoltage" runat="server" Text='<%# Eval("Voltage") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Enclosure NEMA rating</div>
									<div class="ui-block-b">
										<asp:Label ID="lblEnclosure_NEMA_rating" runat="server" Text='<%# Eval("Enclosure_NEMA_rating") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Last Inspection</div>
									<div class="ui-block-b">
										<asp:Label ID="lblLast_Inspection" runat="server" Text='<%# Eval("Last_Inspection","{0:d}") %>'></asp:Label>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Comments</div>
									<div class="ui-block-b">
										<asp:Textbox ID="vtxtComments" runat="server" text='<%# Eval("Comments") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000" ReadOnly="true"></asp:Textbox>
									</div>
								</div>
							<h3 class="ui-bar-b">Pictures</h3>
							<div class="ui-grid-a">
								<div class="ui-block-a">Picture</div>
								<div class="ui-block-b">
									<asp:Label ID="lblPicture1" runat="server" Text='<%# CreateLinkThumbnail(Eval("Picture1"),Eval("Picture1")) %>'></asp:Label>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Picture</div>
								<div class="ui-block-b">
									<asp:Label ID="lblPicture2" runat="server" Text='<%# CreateLinkThumbnail(Eval("Picture2"),Eval("Picture2")) %>'></asp:Label>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Picture</div>
								<div class="ui-block-b">
									<asp:Label ID="lblPicture3" runat="server" Text='<%# CreateLinkThumbnail(Eval("Picture3"),Eval("Picture3")) %>'></asp:Label>
								</div>
							</div>
						</ItemTemplate>
						<EditItemTemplate>
                            <div id="hideFieldsContainer" <%# GetHideFieldInjector() %>>
                            <h3 class="ui-bar-b">General Information</h3>
								<div class="ui-grid-a">
									<div class="ui-block-a">Manufacturer</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtManufacturer" runat="server" text='<%# Bind("Manufacturer") %>'  MaxLength="50" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Model Number</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtModel_Number" runat="server" text='<%# Bind("Model_Number") %>'  MaxLength="50" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Part Number</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtPart_Number" runat="server" text='<%# Bind("Part_Number") %>'  MaxLength="50" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Service Size</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtService_Size" runat="server" text='<%# Bind("Service_Size") %>'  MaxLength="25" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Capacity</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtCapacity" runat="server" text='<%# Bind("Capacity") %>'  MaxLength="25" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">SCFM</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtSCFM" runat="server" text='<%# Bind("SCFM") %>'  MaxLength="25" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Pressure Rating</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtPressure_Rating" runat="server" text='<%# Bind("Pressure_Rating") %>'  MaxLength="25" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Temperature Rating</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtTemperature_Rating" runat="server" text='<%# Bind("Temperature_Rating") %>'  MaxLength="25" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Current Pressure</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtCurrent_Pressure" runat="server" text='<%# Bind("Current_Pressure") %>'  MaxLength="25" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Current Temperature</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtCurrent_Temperature" runat="server" text='<%# Bind("Current_Temperature") %>'  MaxLength="25" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Material</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtMaterial" runat="server" text='<%# Bind("Material") %>'  MaxLength="50" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Number of Stages</div>
									<div class="ui-block-b">
										<Wendel:ExtendedDropDownList ID="ddNumber_of_Stages" runat="server" DataSourceID="SqlDomainNumStage" DataValueField="CODE" DataTextField="DESCRIP" SelectedValue='<%# Bind("Number_of_Stages") %>'  ></Wendel:ExtendedDropDownList>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Fire suppression system type</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtFire_suppression_system_type" runat="server" text='<%# Bind("Fire_suppression_system_type") %>'  MaxLength="50" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Equipment Location</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtEquipment_Location" runat="server" text='<%# Bind("Equipment_Location") %>'  MaxLength="50" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Expiration Date</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtExpiration_Date" runat="server" text='<%# Bind("Expiration_Date","{0:d}") %>'   type="date" placeholder="mm/dd/yyyy"></asp:Textbox>
										<%-- <asp:CalendarExtender ID="CalendarExtenderTxtExpiration_Date" runat="server" TargetControlID="txtExpiration_Date" ></asp:CalendarExtender> --%>
										<asp:CompareValidator ID="ValidateDataTypeTxtExpiration_Date" runat="server"  Display="Dynamic" ErrorMessage="Invalid Entry Format for Expiration Date." Operator="DataTypeCheck" ControlToValidate="txtExpiration_Date" Type="Date"></asp:CompareValidator>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Special Permit Number</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtSpecial_Permit_Number" runat="server" text='<%# Bind("Special_Permit_Number") %>'  MaxLength="25" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Quantity of nozzles/connectors</div>
									<div class="ui-block-b">
										<Wendel:ExtendedDropDownList ID="ddQuantity_of_nozzles_connectors" runat="server" DataSourceID="SqlDomainNumNozzle" DataValueField="CODE" DataTextField="DESCRIP" SelectedValue='<%# Bind("Quantity_of_nozzles_connectors") %>'  ></Wendel:ExtendedDropDownList>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Defueling Type</div>
									<div class="ui-block-b">
										<Wendel:ExtendedDropDownList ID="ddDefueling_Type" runat="server" DataSourceID="SqlDomainDeFuel" DataValueField="CODE" DataTextField="DESCRIP" SelectedValue='<%# Bind("Defueling_Type") %>'  ></Wendel:ExtendedDropDownList>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Associated Equipment</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtAssociated_Equipment" runat="server" text='<%# Bind("Associated_Equipment") %>'  MaxLength="50" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Voltage</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtVoltage" runat="server" text='<%# Bind("Voltage") %>'  MaxLength="25" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Enclosure NEMA rating</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtEnclosure_NEMA_rating" runat="server" text='<%# Bind("Enclosure_NEMA_rating") %>'  MaxLength="25" ></asp:Textbox>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Last Inspection</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtLast_Inspection" runat="server" text='<%# Bind("Last_Inspection","{0:d}") %>'   type="date" placeholder="mm/dd/yyyy"></asp:Textbox>
										<%-- <asp:CalendarExtender ID="CalendarExtenderTxtLast_Inspection" runat="server" TargetControlID="txtLast_Inspection" ></asp:CalendarExtender> --%>
										<asp:CompareValidator ID="ValidateDataTypeTxtLast_Inspection" runat="server"  Display="Dynamic" ErrorMessage="Invalid Entry Format for Last Inspection." Operator="DataTypeCheck" ControlToValidate="txtLast_Inspection" Type="Date"></asp:CompareValidator>
									</div>
								</div>
								<div class="ui-grid-a">
									<div class="ui-block-a">Comments</div>
									<div class="ui-block-b">
										<asp:Textbox ID="txtComments" runat="server" text='<%# Bind("Comments") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000" ></asp:Textbox>
									</div>
								</div>
							<h3 class="ui-bar-b">Pictures</h3>
							<div class="ui-grid-a">
								<div class="ui-block-a">Picture</div>
								<div class="ui-block-b">
									<asp:Label ID="lblPicture1" runat="server" Text='<%# CreateLinkThumbnail(Eval("Picture1"),Eval("Picture1")) %>'></asp:Label>
									<uc1:ReplaceImageAjax ID="ReplaceImage1" runat="server" ButtonText="Upload new photo" TitleText="Upload new or replace existing photo"
										ConnectionName="<%$ Code:m_LookupTablesDBConnection %>" TableName="<%$ Code:m_FeatureClassTable %>" 
										KeyField="ENTRYUID" KeyFieldType="Numeric" KeyValue='<%# gvstr(Eval("ENTRYUID")) %>'
										ImageField="Picture1" CurrentImageFile='<%# gvstr(Eval("Picture1")) %>'
										ServerImagePath="<%$ Code:m_AttachmentPath %>" HttpImagePath="<%$ Code:m_AttachmentPath %>" WebConfigImagePath="PhotoPath"
                                        WebConfigReducedImagePath="PhotoPath_Reduced" reducedImageMaxWidth="640" reducedImageMaxHeight="640"
										LabelIDContainingPicture="lblPicture1" />
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Picture</div>
								<div class="ui-block-b">
									<asp:Label ID="lblPicture2" runat="server" Text='<%# CreateLinkThumbnail(Eval("Picture2"),Eval("Picture2")) %>'></asp:Label>
									<uc1:ReplaceImageAjax ID="ReplaceImage2" runat="server" ButtonText="Upload new photo" TitleText="Upload new or replace existing photo"
										ConnectionName="<%$ Code:m_LookupTablesDBConnection %>" TableName="<%$ Code:m_FeatureClassTable %>" 
										KeyField="ENTRYUID" KeyFieldType="Numeric" KeyValue='<%# gvstr(Eval("ENTRYUID")) %>'
										ImageField="Picture2" CurrentImageFile='<%# gvstr(Eval("Picture2")) %>'
										ServerImagePath="<%$ Code:m_AttachmentPath %>" HttpImagePath="<%$ Code:m_AttachmentPath %>" WebConfigImagePath="PhotoPath"
                                        WebConfigReducedImagePath="PhotoPath_Reduced" reducedImageMaxWidth="640" reducedImageMaxHeight="640"
										LabelIDContainingPicture="lblPicture2" />
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Picture</div>
								<div class="ui-block-b">
									<asp:Label ID="lblPicture3" runat="server" Text='<%# CreateLinkThumbnail(Eval("Picture3"),Eval("Picture3")) %>'></asp:Label>
									<uc1:ReplaceImageAjax ID="ReplaceImage3" runat="server" ButtonText="Upload new photo" TitleText="Upload new or replace existing photo"
										ConnectionName="<%$ Code:m_LookupTablesDBConnection %>" TableName="<%$ Code:m_FeatureClassTable %>" 
										KeyField="ENTRYUID" KeyFieldType="Numeric" KeyValue='<%# gvstr(Eval("ENTRYUID")) %>'
										ImageField="Picture3" CurrentImageFile='<%# gvstr(Eval("Picture3")) %>'
										ServerImagePath="<%$ Code:m_AttachmentPath %>" HttpImagePath="<%$ Code:m_AttachmentPath %>" WebConfigImagePath="PhotoPath"
                                        WebConfigReducedImagePath="PhotoPath_Reduced" reducedImageMaxWidth="640" reducedImageMaxHeight="640"
										LabelIDContainingPicture="lblPicture3" />
								</div>
							</div>
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField ShowHeader="false">
						<ItemTemplate>
                            <asp:LinkButton ID="EditButton" runat="server" CommandName="Edit" Text="Edit" data-role="button" data-theme="a"></asp:LinkButton>
                            <asp:LinkButton ID="DeleteButton" runat="server" CommandName="Delete" Text="Delete This Record" data-role="button" data-theme="a" OnClientClick="return ValidateDeleteAction();"></asp:LinkButton>
						</ItemTemplate>
						<EditItemTemplate>
                            <asp:LinkButton ID="UpdateButton" runat="server" CommandName="Update" Text="Save" data-role="button" data-theme="a"></asp:LinkButton>
                            <asp:LinkButton ID="CancelButton" runat="server" CommandName="Cancel" Text="Cancel" data-role="button" data-theme="c"></asp:LinkButton>
						</EditItemTemplate>
						<InsertItemTemplate>
                            <asp:LinkButton ID="InsertButton" runat="server" CommandName="Insert" Text="Add" data-role="button" data-theme="a"></asp:LinkButton>
                            <asp:LinkButton ID="CancelButton" runat="server" CommandName="Cancel" Text="Cancel" data-role="button" data-theme="c"></asp:LinkButton>
						</InsertItemTemplate>
					</asp:TemplateField>
				</Fields>
			</asp:DetailsView>

		</div>
		<!-- / content -->

	</div>
	<!-- / page -->
	
	<asp:sqldatasource ID="SqlFacility" runat="server" 
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" 
		ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>" 
		SelectCommand="SELECT * FROM Data0_Equipment WHERE ENTRYUID=@ENTRYUID"
		UpdateCommand="<%$ code:sqlFeatureUpdateCommand() %>"
		InsertCommand="<%$ code:sqlFeatureInsertCommand() %>" 
		DeleteCommand="DELETE * FROM Data0_Equipment WHERE ENTRYUID=@ENTRYUID"
		oninserted="SqlFacility_Inserted" >
		<SelectParameters>
			<asp:ControlParameter ControlID="txtEntryUID" PropertyName="Text" Type="Int32" Name="ENTRYUID" />
		</SelectParameters>
		<DeleteParameters>
			<asp:ControlParameter ControlID="txtEntryUID" PropertyName="Text" Type="Int32" Name="ENTRYUID" />
		</DeleteParameters>
		<UpdateParameters>
			<%-- Start paste ------------------------------------------- --%>
			<asp:Parameter Name="Manufacturer" Type="String" />
			<asp:Parameter Name="Model_Number" Type="String" />
			<asp:Parameter Name="Part_Number" Type="String" />
			<asp:Parameter Name="Service_Size" Type="String" />
			<asp:Parameter Name="Capacity" Type="String" />
			<asp:Parameter Name="SCFM" Type="String" />
			<asp:Parameter Name="Pressure_Rating" Type="String" />
			<asp:Parameter Name="Temperature_Rating" Type="String" />
			<asp:Parameter Name="Current_Pressure" Type="String" />
			<asp:Parameter Name="Current_Temperature" Type="String" />
			<asp:Parameter Name="Material" Type="String" />
			<asp:Parameter Name="Number_of_Stages" Type="Int32" />
			<asp:Parameter Name="Fire_suppression_system_type" Type="String" />
			<asp:Parameter Name="Equipment_Location" Type="String" />
			<asp:Parameter Name="Expiration_Date" Type="DateTime" />
			<asp:Parameter Name="Special_Permit_Number" Type="String" />
			<asp:Parameter Name="Quantity_of_nozzles_connectors" Type="Int32" />
			<asp:Parameter Name="Defueling_Type" Type="Int32" />
			<asp:Parameter Name="Associated_Equipment" Type="String" />
			<asp:Parameter Name="Voltage" Type="String" />
			<asp:Parameter Name="Enclosure_NEMA_rating" Type="String" />
			<asp:Parameter Name="Last_Inspection" Type="DateTime" />
			<asp:Parameter Name="Comments" Type="String" />
			<%-- End paste ------------------------------------------- --%>
			<asp:Parameter Name="ENTRYUID" Type="Int32" />
		</UpdateParameters>
		<InsertParameters>
			<%-- Start paste ------------------------------------------- --%>
			<asp:Parameter Name="Manufacturer" Type="String" />
			<asp:Parameter Name="Model_Number" Type="String" />
			<asp:Parameter Name="Part_Number" Type="String" />
			<asp:Parameter Name="Service_Size" Type="String" />
			<asp:Parameter Name="Capacity" Type="String" />
			<asp:Parameter Name="SCFM" Type="String" />
			<asp:Parameter Name="Pressure_Rating" Type="String" />
			<asp:Parameter Name="Temperature_Rating" Type="String" />
			<asp:Parameter Name="Current_Pressure" Type="String" />
			<asp:Parameter Name="Current_Temperature" Type="String" />
			<asp:Parameter Name="Material" Type="String" />
			<asp:Parameter Name="Number_of_Stages" Type="Int32" />
			<asp:Parameter Name="Fire_suppression_system_type" Type="String" />
			<asp:Parameter Name="Equipment_Location" Type="String" />
			<asp:Parameter Name="Expiration_Date" Type="DateTime" />
			<asp:Parameter Name="Special_Permit_Number" Type="String" />
			<asp:Parameter Name="Quantity_of_nozzles_connectors" Type="Int32" />
			<asp:Parameter Name="Defueling_Type" Type="Int32" />
			<asp:Parameter Name="Associated_Equipment" Type="String" />
			<asp:Parameter Name="Voltage" Type="String" />
			<asp:Parameter Name="Enclosure_NEMA_rating" Type="String" />
			<asp:Parameter Name="Last_Inspection" Type="DateTime" />
			<asp:Parameter Name="Comments" Type="String" />
			<%-- End paste ------------------------------------------- --%>
		</InsertParameters>
	</asp:sqldatasource>

	<asp:SqlDataSource ID="SqlDomainDeFuel" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainDeFuel] ORDER BY [DisplayOrder]">
	</asp:SqlDataSource>
	<asp:SqlDataSource ID="SqlDomainNumNozzle" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainNumNozzle] ORDER BY [DisplayOrder]">
	</asp:SqlDataSource>
	<asp:SqlDataSource ID="SqlDomainNumStage" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainNumStage] ORDER BY [DisplayOrder]">
	</asp:SqlDataSource>

	<asp:TextBox ID="txtBuildingID" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtSection" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtCategory" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtSubCategory" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtSubCatID" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtEntryUID" runat="server" Visible="false"></asp:TextBox>
	<asp:TextBox ID="txtInspector"  runat="server" Visible="false"></asp:TextBox>

</form>
</body>
</html>
