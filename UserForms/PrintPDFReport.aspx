<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintPDFReport.aspx.cs" Inherits="PrintPDFReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=7;IE=8;IE=9;IE=10;IE=11;IE=12" />
    <title>Print Survey Work Sheet</title>
    <link rel="stylesheet" href="../../CSS/jquery-ui.css" />
    <link href="../../CSS/nanoscroller.css" rel="stylesheet" type="text/css" />
    <link href="../../CSS/StyleSheet2.css" rel="stylesheet" type="text/css" />
    <link href="../../CSS/jqm_converter.css" rel="stylesheet" type="text/css" />
    <link href="../../CSS/tooltip.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" language="javascript" src="../../JavaScript/jquery-1.8.2.min.js"></script>
    <script type="text/javascript" src="../../JavaScript/jquery-ui.js"></script>
    <script type="text/javascript" src="../../JavaScript/jquery.nanoscroller.min.js"></script>
    <script src="../../JavaScript/tooltip.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript" src="../../JavaScript/b_greyoutloadinganimation.js"></script>

</head>
<body>
         <form id="form1" runat="server">
        <asp:TextBox ID="txtYr" runat="server" Visible="false"></asp:TextBox>
        <asp:TextBox ID="txtLoc" runat="server" Visible="false"></asp:TextBox>
        <asp:TextBox ID="txtType" runat="server" Visible="false"></asp:TextBox>
    </form>
</body>
</html>
