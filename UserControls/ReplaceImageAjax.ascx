<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReplaceImageAjax.ascx.cs" Inherits="ReplaceImageAjax" %>
<div id="<%= UpdatePanel1.ClientID + "_btn_popup" %>">
    <a href="<%= "#" + UpdatePanel1.ClientID + "popup" %>" data-rel="popup" data-position-to="window" data-role="button" data-theme="c" data-inline="true" class="popup-btn replace-image-button">Upload Photo</a>
    <div id="<%= UpdatePanel1.ClientID + "_deletephoto" %>" class="replace-image-button" style="margin-top:5px">
        <asp:Button ID="btnDeletePicture" runat="server" Text="Delete Photo" onclick="btnDelete_Click" OnClientClick="return ValidateDeletePhotoAction();" data-role="button" data-theme="c" data-inline="true" />
    </div>
</div>
<div id="<%= UpdatePanel1.ClientID + "_disabled_msg" %>" style="display:none;">
    Please save the record prior to uploading pictures.
</div>

<div data-role="popup" id="<%= UpdatePanel1.ClientID + "popup" %>" class="ui-content" data-overlay-theme="a" data-history="false" data-position-to="window" >
	<a href="#" data-rel="back" data-role="button" data-theme="c" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			
	<asp:Panel ID="pnlPopupHeader" runat="server" SkinID="noSkin">
		<h4 class="ui-bar-c"><asp:Label ID="lblPopupHeader" runat="server" Text="Change Photo"></asp:Label></h4>
	</asp:Panel>

    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
	    <ContentTemplate>
			<p>
				<asp:Label ID="lblCurrentPhoto" runat="server"></asp:Label>
			</p>
	    </ContentTemplate>
    </asp:UpdatePanel>
	<div>
		File:
		<asp:AsyncFileUpload ID="MyFile" runat="server" UploaderStyle="Traditional" ThrobberID="myThrobber"
			UploadingBackColor="#66CCFF" CompleteBackColor="#00A336"
			OnUploadedComplete="MyFile_UploadedComplete" OnClientUploadComplete='<%# UpdatePanel1.ClientID + "_refresh" %>' OnClientUploadStarted='<%# UpdatePanel1.ClientID + "_reposition" %>' /> 
		<asp:Label runat="server" ID="myThrobber" style="display:none;" ><img src="../Images/Loading/uploading.gif" alt="uploading..." /></asp:Label>
	</div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
	    <ContentTemplate>
			<%--<p><strong>Please select a new photo to upload</strong></p>--%>
            <div>
			    <asp:Label ID="lblError" runat="server" Text="" ForeColor="Red" Font-Size="Larger" Font-Bold="true"></asp:Label>
			    <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Green" ></asp:Label>
            </div>
            <div style="display:none;">
                <asp:Button ID="btnRefresh" runat="server" Text="Button" onclick="btnRefresh_Click" />
            </div>
	    </ContentTemplate>
        <Triggers>
            <%-- assign btnRefresh Click event in AsyncPostBackTrigger to prevent full page postback when you click the button --%>
            <asp:AsyncPostBackTrigger ControlID="btnRefresh" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnDeletePicture" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</div>
<script language="javascript" type="text/javascript">
    function <%= UpdatePanel1.ClientID + "_refresh" %>(sender,args) {
        $get('<%=btnRefresh.ClientID %>').click();
    }
    function <%= UpdatePanel1.ClientID + "_reposition" %>(sender,args) {
        var dlgheight = $('<%= "#" + UpdatePanel1.ClientID + "popup" %>').parent().height();
        var dlgwidth = $('<%= "#" + UpdatePanel1.ClientID + "popup" %>').parent().width();
        window_width = $(window).width();
        window_height = $(window).height();
        var mt = ((window_height - dlgheight) / 2) + 'px';
        var ml = ((window_width - dlgwidth) / 2) + 'px';
        $('<%= "#" + UpdatePanel1.ClientID + "popup" %>').parent().css({ 'position':'fixed','top':mt,'left':ml });
        $('<%= "#" + UpdatePanel1.ClientID + "popup" %>').popup("open");
    }
    $(document).ready(function () {
        $('<%= "#" + UpdatePanel1.ClientID + "popup" %>').popup({
            afteropen: function( e) {
                $('#<%=lblError.ClientID %>').empty();
                $('#<%=lblMessage.ClientID %>').empty();
                //$('body').css({'overflow':'hidden'}).on('touchmove', function(e) {
                //     e.preventDefault();
                //});
            },
            afterclose: function(e) {
                //$('body').css({'overflow':'inherit'}).off('touchmove');
            }
        });
    });
</script>
