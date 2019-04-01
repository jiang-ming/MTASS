<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReplaceImage.ascx.cs" Inherits="NFTACS.UserControls.ReplaceImage" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" >
	<ContentTemplate>
		<a href="<%= "#" + UpdatePanel1.ClientID + "popup" %>" data-rel="popup" data-position-to="window" data-role="button" data-theme="c" data-inline="true" class="popup-btn">Upload Image</a>
		
		<div data-role="popup" id="<%= UpdatePanel1.ClientID + "popup" %>" class="ui-content" data-overlay-theme="a">
			<a href="#" data-rel="back" data-role="button" data-theme="c" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			
			<asp:Panel ID="pnlPopupHeader" runat="server" SkinID="SectionPanel">
				<h4 class="ui-bar-c"><asp:Label ID="lblPopupHeader" runat="server" Text="Change Photo"></asp:Label></h4>
			</asp:Panel>
			<p>
				<asp:Label ID="lblCurrentPhoto" runat="server"></asp:Label>
				<asp:Image ID="imgCurrentPhoto" CssClass="thumb120" runat="server" />
                <asp:Label ID="ajaxlabel" runat="server" Text="Label"></asp:Label>
			</p>
			<p><strong id="UploadText">Please select a new photo to upload</strong></p>
			<div>
				File:
				<asp:AsyncFileUpload ID="MyFile" runat="server" UploaderStyle="Traditional" ThrobberID="myThrobber"
					UploadingBackColor="#66CCFF" CompleteBackColor="#00A336"
					OnUploadedComplete="MyFile_UploadedComplete" /> 
				<asp:Label runat="server" ID="myThrobber" style="display:none;" >  
					<img src="../Images/uploading.gif" alt="uploading..." />
				</asp:Label>
			</div>
			<asp:Label ID="lblError" runat="server" Text="" ForeColor="Red" Font-Size="Larger" Font-Bold="true"></asp:Label>
			<asp:Label ID="lblMessage" runat="server" Font-Bold="True" ></asp:Label>
		</div>
	</ContentTemplate>
</asp:UpdatePanel>



