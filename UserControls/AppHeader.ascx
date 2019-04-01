<%@ Control Language="C#" ClassName="Banner" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        PageHeader.Attributes.Add("style", "position: relative; background-image: url(../../Images/Banner/Blank.gif); background-repeat: no-repeat; background-color: #dfe2e8;");
        Image1.ImageUrl = "~/Images/Wendel/wendelnobg_small.png";
    }

</script>
<asp:Panel runat="server" ID="PageHeader" Visible="true" Width="100%" Height="50px" >
    <span style='line-height:50px; vertical-align: middle; font-size: 28px; font-weight: normal; color: #003768; margin-left: 16px; font-family:Impact;'>First Niagara ADA Compliance Survey</span>
    <asp:Image ID="Image1"  runat="server" ImageAlign="Right" CssClass="bannerLogo" />
</asp:Panel> 

