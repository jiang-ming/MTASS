<%@ Page Language="C#" AutoEventWireup="true" Inherits="WDUserStatus" MaintainScrollPositionOnPostback="true" Codebehind="WDUserStatus.aspx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>User Status</title>
    <link href="../CSS/StyleSheet.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        function PopOut(sDocName) {
            var nw;
            nw = window.open(sDocName, 'Feature_Document');
        }
    </script>
    <script language="javascript" type="text/javascript">
        var IsLoading = false;
        function HideLoading() {
            document.getElementById('LoadingAnimation').style.visibility = 'hidden';
            IsLoading = false;
        }
        function ShowLoading() {
            var ScrollTop = document.body.scrollTop;
            if (ScrollTop == 0) {
                if (window.pageYOffset)
                    ScrollTop = window.pageYOffset;
                else
                    ScrollTop = (document.body.parentElement) ? document.body.parentElement.scrollTop : 0;
            }
            var st = (150 + ScrollTop) + 'px';
            document.getElementById('LoadingAnimation').style.top = st;
            if (IsLoading) {
                alert("Previous web request has not been successfully processed yet.");
                return false;
            } else {
                document.getElementById('LoadingAnimation').style.visibility = 'visible';
                setTimeout('InitAnimation();', 50); //form submit caused animation to stop, need to kick it after form submission
                IsLoading = true;
                return true
            }
        }

        function InitAnimation() {
            var img = document.getElementById('LoadingAnimationImage');
            img.src = "../../images/Wendel/gt_processing.gif";
        }

        function showGreyOut() {
            var cvr = document.getElementById("pagegreyout");
            cvr.style.display = "block"
            if (document.body.style.overflow = "hidden") {
                cvr.style.width = "2000"
                cvr.style.height = "100%"
            }
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

    </script>
</head>
<body alink='#0000FF' vlink='#0000ff'>
    <div id="pagegreyout"></div>
    <div>
        <form id="form1" runat="server">
            <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" EnableScriptGlobalization="True" />
            <!-- ------------------------------------------------------------------------------------------------
                          Header
             ------------------------------------------------------------------------------------------------   -->
            <div style="z-index:2000000; left: 300px; position: absolute; top: 300px; visibility: hidden;" id="LoadingAnimation">
                <img id="LoadingAnimationImage" src="../../images/Wendel/gt_processing.gif" alt="processing" style="border-right: black solid; border-top: black solid; border-left: black solid; border-bottom: black solid;" />
            </div>
            <table cellspacing="0" cellpadding="5" border="0" width="50%">
                <tr>
                    <td align="left" class="PageHeader">
                        <img src="../Images/wendellogo.jpg" />
                    </td>
                </tr>
            </table> 
            <asp:Panel ID="pnlPageTitleCaption" runat="server" SkinID="SectionPanel" Width="100%" Height="16px">
                <asp:Label ID="lblPageTitleCaption" runat="server" Font-Bold="True" Font-Size="16px" >User Status</asp:Label>
            </asp:Panel>
            <br />
            <div style="text-align:center;">
                <table border="0" width="90%" style="text-align:left;">
                    <tr>
                        <td><br />
                            <asp:Label ID="NumUserOnline" runat="server" BackColor="White" Font-Names="Verdana"
                                Font-Size="Medium" ForeColor="Black" Font-Bold="true" Height="35px" Text="1 user online." Width="642px"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" 
                                GridLines="None" AutoGenerateColumns="false" Width="100%">
                                <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                <RowStyle BackColor="#EFEFEF" />
                                <PagerStyle BackColor="#555555" ForeColor="White" HorizontalAlign="Center" />
                                <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                <HeaderStyle BackColor="#555555" Font-Bold="True" ForeColor="White" HorizontalAlign="Left"  />
                                <EditRowStyle BackColor="#7C6F57" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                     <asp:BoundField DataField="Session ID" HeaderText="Session ID" SortExpression="Session ID" ReadOnly="True" Visible="True" />
                                     <asp:BoundField DataField="User" HeaderText="User" SortExpression="User" ReadOnly="True" Visible="True" />
                                     <asp:BoundField DataField="IP Address" HeaderText="IP Address" SortExpression="IP Address" ReadOnly="True" Visible="True" />
                                     <asp:BoundField DataField="Host Name" HeaderText="Host Name" SortExpression="Host Name" ReadOnly="True" Visible="True" />
                                     <asp:BoundField DataField="Time Started" HeaderText="Time Started" SortExpression="Time Started" ReadOnly="True" Visible="True" />
                                     <asp:BoundField DataField="Last Request" HeaderText="Last Request" SortExpression="Last Request" ReadOnly="True" Visible="True" />
                                     <asp:BoundField DataField="Time Since Last Request" HeaderText="Time Since Last Request" SortExpression="Time Since Last Request" ReadOnly="True" Visible="True" />
                                </Columns>
                            </asp:GridView><br /><br />
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>
</body>
</html>
