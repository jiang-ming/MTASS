<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataForm1Edit.aspx.cs" Inherits="MTASS.UserForms.DataForm1Edit" %>
<%@ Register Assembly="ExtendedDropDownList" Namespace="Wendel" TagPrefix="Wendel" %>
<%@ Register Src="../UserControls/ReplaceImageAjax.ascx" TagName="ReplaceImageAjax" TagPrefix="uc1" %>
<!DOCTYPE html>
<html lang="en-us">
<head runat="server">
	<title>Assessment Form - MTA S/S</title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0"/>
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<link rel="stylesheet" type="text/css" href="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css" />
	<link rel="stylesheet" type="text/css" href="../css/custom.css" />
	<link rel="stylesheet" type="text/css" href="../css/overrides.css" />
    <script type="text/javascript" language="javascript" src="../Scripts/ie8indexOf.js"></script>
	<script type="text/javascript" language="javascript" src="https://code.jquery.com/jquery-1.8.2.min.js"></script>
	<script type="text/javascript" language="javascript" src="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.js"></script>
    <script type="text/javascript" language="javascript">
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

        function checkTextAreaMaxLength(textBox, e, length) {
            var mLen = textBox["MaxLength"];
            if (null == mLen)
                mLen = length;

            var maxLength = parseInt(mLen);
            if (!checkSpecialKeys(e)) {
                if (textBox.value.length > maxLength - 1) {
                    if (window.event)//IE
                        e.returnValue = false;
                    else//Firefox
                        e.preventDefault();
                }
            }
        }
        function checkSpecialKeys(e) {
            if (e.keyCode != 8 && e.keyCode != 46 && e.keyCode != 37 && e.keyCode != 38 && e.keyCode != 39 && e.keyCode != 40)
                return false;
            else
                return true;
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
							<h3 class="ui-bar-b">Component Integrity</h3>
						    <div class="ui-grid-a">
							    <div class="ui-block-a">Applicable Regulatory Citation</div>
							    <div class="ui-block-b">
								    <asp:Label ID="lblComponent_Integrity" runat="server" Text='<%# DoLookupSqlDomainIntegrity(Eval("Component_Integrity")) %>'></asp:Label>
							    </div>
						    </div>
                            <div class="ui-grid-a">
							    <div class="ui-block-a">Applicable Code</div>
							    <div class="ui-block-b">
								    <asp:Label ID="lblApplicableCode" runat="server" text='<%# Eval("ApplicableCode") %>' ></asp:Label>
							    </div>
						    </div>
						    <div class="ui-grid-a">
							    <div class="ui-block-a">Description of Finding</div>
							    <div class="ui-block-b">
								    <asp:Textbox ID="vtxtComments1" runat="server" text='<%# Eval("Comments1") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000" ReadOnly="true"></asp:Textbox>
							    </div>
						    </div>
							<h3 class="ui-bar-b">Condition</h3>
                            <div class="ui-grid-a">
                                <div class="ui-block-a">Priority</div>
                                <div class="ui-block-b">
                                    <asp:Label ID="lblPriority" runat="server" Text='<%# DoLookupSqlDomainPriority(Eval("Priority")) %>'></asp:Label>
                                </div>
                            </div>
						    <div class="ui-grid-a">
							    <div class="ui-block-a">Condition (old field)</div>
							    <div class="ui-block-b">
								    <asp:Label ID="lblCondition" runat="server" Text='<%# DoLookupSqlDomainCondition(Eval("Condition")) %>'></asp:Label>
							    </div>
						    </div>
                            <div class="ui-grid-a">
							    <div class="ui-block-a">Responsible Party</div>
							    <div class="ui-block-b">
								    <asp:Label ID="lblResponsibleParty" runat="server" Text='<%# DoLookupSqlDomainResponsibleParty(Eval("ResponsibleParty")) %>'></asp:Label>
							    </div>
						    </div>
						    <div class="ui-grid-a">
							    <div class="ui-block-a">Classification of Finding</div>
							    <div class="ui-block-b">
								    <asp:Label ID="lblClassification" runat="server" Text='<%# DoLookupSqlDomainClassification(Eval("Classification")) %>'></asp:Label>
							    </div>
						    </div>
						    <div class="ui-grid-a">
							    <div class="ui-block-a">Description of Finding</div>
							    <div class="ui-block-b">
								    <asp:Textbox ID="vtxtComments2" runat="server" text='<%# Eval("Comments2") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000" ReadOnly="true"></asp:Textbox>
							    </div>
						    </div>
                            <div class="ui-grid-a">
                                <div class="ui-block-a">Corrective Actions Required</div>
                                <div class="ui-block-b">
                                    <asp:TextBox ID="vtxtCorrectiveActionsRequired" runat="server" Text='<%# Eval("CorrectiveActionsRequired") %>' Rows="4" TextMode="MultiLine" MaxLength="2000" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="ui-grid-a">
                                <div class="ui-block-a">Expected Completion Date</div>
                                <div class="ui-block-b">
                                    <asp:Label ID="lblExpectedCompletionDate" runat="server" Text='<%# Eval("ExpectedCompletionDate","{0:d}") %>'></asp:Label>
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
						    <div class="ui-grid-a">
							    <div class="ui-block-a">Comments</div>
							    <div class="ui-block-b">
								    <asp:Textbox ID="vtxtComments3" runat="server" text='<%# Eval("Comments3") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000" ReadOnly="true"></asp:Textbox>
							    </div>
						    </div>
							<h3 class="ui-bar-b">Contractor Notes</h3>
							<div class="ui-grid-a">
								<div class="ui-block-a">Variance</div>
								<div class="ui-block-b">
									<asp:Label ID="lblVARIANCE" runat="server" Text='<%# Eval("VARIANCE") %>'></asp:Label>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Variance Description</div>
								<div class="ui-block-b">
									<asp:Textbox ID="vtxtVARIANCE_DESC" runat="server" text='<%# Eval("VARIANCE_DESC") %>' Rows="4"  TextMode="MultiLine"   ReadOnly="true"></asp:Textbox>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">In/Out Contract</div>
								<div class="ui-block-b">
									<asp:Label ID="lblINOUT" runat="server" Text='<%# Eval("INOUT") %>'></asp:Label>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Compliance Plan</div>
								<div class="ui-block-b">
									<asp:Textbox ID="vtxtCOMPLIANCE_PLAN" runat="server" text='<%# Eval("COMPLIANCE_PLAN") %>' Rows="4"  TextMode="MultiLine"   ReadOnly="true"></asp:Textbox>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Cost</div>
								<div class="ui-block-b">
									<asp:Label ID="lblCOST" runat="server" Text='<%# Eval("COST") %>'></asp:Label>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Complete By</div>
								<div class="ui-block-b">
									<asp:Label ID="lblCOMPLETE_BY" runat="server" Text='<%# Eval("COMPLETE_BY","{0:d}") %>'></asp:Label>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Contractor Notes</div>
								<div class="ui-block-b">
									<asp:Textbox ID="vtxtContractorNotes" runat="server" text='<%# Eval("ContractorNotes") %>' Rows="4"  TextMode="MultiLine"   ReadOnly="true"></asp:Textbox>
								</div>
							</div>
							<h3 class="ui-bar-b">Status</h3>
							<div class="ui-grid-a">
								<div class="ui-block-a">MTA Response</div>
								<div class="ui-block-b">
									<asp:Label ID="lblMTA" runat="server" Text='<%# Eval("MTA_Response") %>'></asp:Label>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Status</div>
								<div class="ui-block-b">
									<asp:Label ID="Label1" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
								</div>
							</div>
						</ItemTemplate>
						<EditItemTemplate>
                            <div id="hideFieldsContainer" <%# GetHideFieldInjector() %>>
							<h3 class="ui-bar-b">Component Integrity</h3>
							<div class="ui-grid-a">
							    <div class="ui-block-a">Applicable Regulatory Citation</div>
								<div class="ui-block-b">
									<Wendel:ExtendedDropDownList ID="ddComponent_Integrity" runat="server" DataSourceID="SqlDomainIntegrity" DataValueField="CODE" DataTextField="DESCRIP" SelectedValue='<%# Bind("Component_Integrity") %>'  ></Wendel:ExtendedDropDownList>                            
								</div>
							</div>
                            <div class="ui-grid-a">
							    <div class="ui-block-a">Applicable Code</div>
								<div class="ui-block-b">
									<asp:Textbox ID="txtApplicableCode" runat="server" text='<%# Bind("ApplicableCode") %>'  MaxLength="200" onkeyDown="checkTextAreaMaxLength(this,event,200);" ></asp:Textbox>
								</div>
							</div>
							<div class="ui-grid-a">
							    <div class="ui-block-a">Description of Finding</div>
								<div class="ui-block-b">
									<asp:Textbox ID="txtComments1" runat="server" text='<%# Bind("Comments1") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000" onkeyDown="checkTextAreaMaxLength(this,event,2000);" ></asp:Textbox>
								</div>
							</div>
							<h3 class="ui-bar-b">Condition</h3>
							<div class="ui-grid-a">
                                <div class="ui-block-a">Priority</div>
                                <div class="ui-block-b">
                                    <asp:Label ID="lblPriority" runat="server" Text='<%# DoLookupSqlDomainPriority(Eval("Priority")) %>'></asp:Label>
                                </div>
                            </div>
                            <div class="ui-grid-a">
							    <div class="ui-block-a">Condition (old field)</div>
								<div class="ui-block-b">
									<Wendel:ExtendedDropDownList ID="ddCondition" runat="server" DataSourceID="SqlDomainCondition" DataValueField="CODE" DataTextField="DESCRIP" SelectedValue='<%# Bind("Condition") %>'  ></Wendel:ExtendedDropDownList>
								</div>
							</div>
                            <div class="ui-grid-a">
							    <div class="ui-block-a">Responsible Party</div>
								<div class="ui-block-b">
									<Wendel:ExtendedDropDownList ID="ddResponsibleParty" runat="server" DataSourceID="SqlDomainResponsibleParty" DataValueField="CODE" DataTextField="DESCRIP" SelectedValue='<%# Bind("ResponsibleParty") %>'  ></Wendel:ExtendedDropDownList>                            
								</div>
							</div>
						    <div class="ui-grid-a">
							    <div class="ui-block-a">Classification of Finding</div>
							    <div class="ui-block-b">
									<Wendel:ExtendedDropDownList ID="ddClassification" runat="server" DataSourceID="SqlDomainClassification" DataValueField="CODE" DataTextField="DESCRIP" SelectedValue='<%# Bind("Classification") %>'  ></Wendel:ExtendedDropDownList>
							    </div>
						    </div>
							<div class="ui-grid-a">
							    <div class="ui-block-a">Description of Finding</div>
								<div class="ui-block-b">
									<asp:Textbox ID="txtComments2" runat="server" text='<%# Bind("Comments2") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000" onkeyDown="checkTextAreaMaxLength(this,event,2000);" ></asp:Textbox>
								</div>
							</div>
                            <div class="ui-grid-a">
                                <div class="ui-block-a">Corrective Actions Required</div>
                                <div class="ui-block-b">
                                    <asp:TextBox ID="txtCorrectiveActionsRequired" runat="server" Text='<%# Bind("CorrectiveActionsRequired") %>' Rows="4" TextMode="MultiLine" MaxLength="2000" onkeyDown="checkTextAreaMaxLength(this,event,2000;"></asp:TextBox>
                                </div>
                            </div>
                            <div class="ui-grid-a">
                                <div class="ui-block-a">Expected Completion Date</div>
                                <div class="ui-block-b">
                                    <asp:TextBox ID="txtExpectedCompletionDate" runat="server" Text='<%# Bind("ExpectedCompletionDate","{0:d}") %>' placeholder="mm/dd/yyy"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtendertxtExpectedCompletionDate" runat="server" TargetControlID="txtExpectedCompletionDate" ></asp:CalendarExtender>
									<asp:CompareValidator ID="ValidateDataTypetxtExpectedCompletionDate" runat="server"  Display="Dynamic" ErrorMessage="Invalid Entry Format for Date." Operator="DataTypeCheck" ControlToValidate="txtExpectedCompletionDate" Type="Date"></asp:CompareValidator>
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
							<div class="ui-grid-a">
								<div class="ui-block-a">Comments</div>
								<div class="ui-block-b">
									<asp:Textbox ID="txtComments3" runat="server" text='<%# Bind("Comments3") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000" onkeyDown="checkTextAreaMaxLength(this,event,2000);" ></asp:Textbox>
								</div>
							</div>
							<h3 class="ui-bar-b">Contractor Notes</h3>
							<div class="ui-grid-a">
								<div class="ui-block-a">Variance</div>
								<div class="ui-block-b">
									<Wendel:ExtendedDropDownList ID="ddVARIANCE" runat="server" DataSourceID="SqlDomainYesNoText" DataValueField="CODE" DataTextField="DESCRIP" SelectedValue='<%# Bind("VARIANCE") %>'  ></Wendel:ExtendedDropDownList>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Variance Description</div>
								<div class="ui-block-b">
									<asp:Textbox ID="txtVARIANCE_DESC" runat="server" text='<%# Bind("VARIANCE_DESC") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000"  onkeyDown="checkTextAreaMaxLength(this,event,2000);"  ></asp:Textbox>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">In/Out Contract</div>
								<div class="ui-block-b">
									<Wendel:ExtendedDropDownList ID="ddINOUT" runat="server" DataSourceID="SqlDomainInOut" DataValueField="CODE" DataTextField="DESCRIP" SelectedValue='<%# Bind("INOUT") %>'  ></Wendel:ExtendedDropDownList>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Compliance Plan</div>
								<div class="ui-block-b">
									<asp:Textbox ID="txtCOMPLIANCE_PLAN" runat="server" text='<%# Bind("COMPLIANCE_PLAN") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000"  onkeyDown="checkTextAreaMaxLength(this,event,2000);"  ></asp:Textbox>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Cost</div>
								<div class="ui-block-b">
									<asp:Textbox ID="txtCOST" runat="server" text='<%# Bind("COST") %>'   type="number" step="any" ></asp:Textbox>
									<asp:CompareValidator ID="ValidateDataTypeTxtCOST" runat="server"  Display="Dynamic" ErrorMessage="Invalid Entry Format for Cost." Operator="DataTypeCheck" ControlToValidate="txtCOST" Type="Double"></asp:CompareValidator>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Complete By</div>
								<div class="ui-block-b">
									<asp:Textbox ID="txtCOMPLETE_BY" runat="server" text='<%# Bind("COMPLETE_BY","{0:d}") %>'   placeholder="mm/dd/yyyy" ></asp:Textbox>
									<asp:CalendarExtender ID="CalendarExtenderTxtCOMPLETE_BY" runat="server" TargetControlID="txtCOMPLETE_BY" ></asp:CalendarExtender>
									<asp:CompareValidator ID="ValidateDataTypeTxtCOMPLETE_BY" runat="server"  Display="Dynamic" ErrorMessage="Invalid Entry Format for Complete By." Operator="DataTypeCheck" ControlToValidate="txtCOMPLETE_BY" Type="Date"></asp:CompareValidator>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Contractor Notes</div>
								<div class="ui-block-b">
									<asp:Textbox ID="txtContractorNotes" runat="server" text='<%# Bind("ContractorNotes") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000"  onkeyDown="checkTextAreaMaxLength(this,event,2000);"  ></asp:Textbox>
								</div>
							</div>
							<h3 class="ui-bar-b">Status</h3>
							<div class="ui-grid-a">
								<div class="ui-block-a">MTA Response</div>
								<div class="ui-block-b">
									<asp:Textbox ID="txtMTA" runat="server" text='<%# Bind("MTA_Response") %>' Rows="4"  TextMode="MultiLine" MaxLength="2000"  onkeyDown="checkTextAreaMaxLength(this,event,2000);"  ></asp:Textbox>
								</div>
							</div>
							<div class="ui-grid-a">
								<div class="ui-block-a">Status</div>
								<div class="ui-block-b">
									<Wendel:ExtendedDropDownList ID="ddOpenClose" runat="server" DataSourceID="SqlDomainOpenClose" DataValueField="CODE" DataTextField="DESCRIP" SelectedValue='<%# Bind("Status") %>'  ></Wendel:ExtendedDropDownList>
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
		SelectCommand="SELECT * FROM Data1_DataEntry WHERE ENTRYUID=@ENTRYUID"
		UpdateCommand="<%$ code:sqlFeatureUpdateCommand() %>"
		InsertCommand="<%$ code:sqlFeatureInsertCommand() %>" 
		DeleteCommand="DELETE * FROM Data1_DataEntry WHERE ENTRYUID=@ENTRYUID"
		oninserted="SqlFacility_Inserted" >
		<SelectParameters>
			<asp:ControlParameter ControlID="txtEntryUID" PropertyName="Text" Type="Int32" Name="ENTRYUID" />
		</SelectParameters>
		<UpdateParameters>
			<%-- Start paste ------------------------------------------- --%>
			<asp:Parameter Name="Component_Integrity" Type="Int32" />
			<asp:Parameter Name="Comments1" Type="String" />
			<asp:Parameter Name="Condition" Type="Int32" />
			<asp:Parameter Name="ResponsibleParty" Type="Int32" />
			<asp:Parameter Name="Classification" Type="Int32" />
			<asp:Parameter Name="Comments2" Type="String" />
			<asp:Parameter Name="Comments3" Type="String" />
			<asp:Parameter Name="VARIANCE" Type="String" />
			<asp:Parameter Name="VARIANCE_DESC" Type="String" />
			<asp:Parameter Name="INOUT" Type="String" />
			<asp:Parameter Name="COMPLIANCE_PLAN" Type="String" />
			<asp:Parameter Name="COST" Type="Double" />
			<asp:Parameter Name="COMPLETE_BY" Type="DateTime" />
			<asp:Parameter Name="ContractorNotes" Type="String" />
			<asp:Parameter Name="MTA_Response" Type="String" />
			<asp:Parameter Name="Status" Type="String" />
            <asp:Parameter Name="CorrectiveActionsRequired" Type="String" />
            <asp:Parameter Name="ApplicableCode" Type="String" />
            <asp:Parameter Name="ExpectedCompletionDate" Type="DateTime" />
			<%-- End paste ------------------------------------------- --%>
			<asp:Parameter Name="ENTRYUID" Type="Int32" />
		</UpdateParameters>
		<InsertParameters>
			<%-- Start paste ------------------------------------------- --%>
			<asp:Parameter Name="Component_Integrity" Type="Int32" />
			<asp:Parameter Name="Comments1" Type="String" />
			<asp:Parameter Name="Condition" Type="Int32" />
            <asp:Parameter Name="Priority" Type="Int32" />
            <asp:Parameter Name="ResponsibleParty" Type="Int32" />
			<asp:Parameter Name="Classification" Type="Int32" />
			<asp:Parameter Name="Comments2" Type="String" />
			<asp:Parameter Name="Comments3" Type="String" />
			<asp:Parameter Name="VARIANCE" Type="String" />
			<asp:Parameter Name="VARIANCE_DESC" Type="String" />
			<asp:Parameter Name="INOUT" Type="String" />
			<asp:Parameter Name="COMPLIANCE_PLAN" Type="String" />
			<asp:Parameter Name="COST" Type="Double" />
			<asp:Parameter Name="COMPLETE_BY" Type="DateTime" />
			<asp:Parameter Name="ContractorNotes" Type="String" />
			<asp:Parameter Name="MTA_Response" Type="String" />
			<asp:Parameter Name="Status" Type="String" />
            <asp:Parameter Name="CorrectiveActionsRequired" Type="String" />
            <asp:Parameter Name="ApplicableCode" Type="String" />
            <asp:Parameter Name="ExpectedCompletionDate" Type="DateTime" />
			<%-- End paste ------------------------------------------- --%>
		</InsertParameters>
		<DeleteParameters>
			<asp:ControlParameter ControlID="txtEntryUID" PropertyName="Text" Type="Int32" Name="ENTRYUID" />
		</DeleteParameters>
	</asp:sqldatasource>

	<asp:SqlDataSource ID="SqlDomainIntegrity" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainIntegrity] ORDER BY [DisplayOrder]">
	</asp:SqlDataSource>
	<asp:SqlDataSource ID="SqlDomainCondition" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainCondition] ORDER BY [DisplayOrder]">
	</asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDomainPriority" runat="server"
        ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
        SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainPriority] ORDER BY [DisplayOrder]">
    </asp:SqlDataSource>
	<asp:SqlDataSource ID="SqlDomainYesNoText" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainYesNoText] ORDER BY [DisplayOrder]">
	</asp:SqlDataSource>
	<asp:SqlDataSource ID="SqlDomainInOut" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainInOut] ORDER BY [DisplayOrder]">
	</asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDomainResponsibleParty" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainResponsibleParty] ORDER BY [DisplayOrder]">
	</asp:SqlDataSource>
	<asp:SqlDataSource ID="SqlDomainClassification" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainClassification] ORDER BY [DisplayOrder]">
	</asp:SqlDataSource>
	<asp:SqlDataSource ID="SqlDomainOpenClose" runat="server"
		ConnectionString="<%$ ConnectionStrings:_SiteAccess %>" ProviderName="<%$ ConnectionStrings:_SiteAccess.ProviderName %>"
		SelectCommand="SELECT [CODE],[DESCRIP] FROM [DomainStatus] ORDER BY [DisplayOrder]">
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
