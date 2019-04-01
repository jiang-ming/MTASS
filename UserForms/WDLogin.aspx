<%@ Page Language="C#" AutoEventWireup="true" Inherits="WDLogin" Codebehind="WDLogin.aspx.cs" %>
<!DOCTYPE html>
<html lang="en-us">
<head runat="server">
	<title>Login - MTA S/S</title>  
	<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0"/>
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<link rel="stylesheet" href="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.css" />
	<link href="../css/custom.css" rel="stylesheet" type="text/css" />
	<link href="../css/overrides.css" rel="stylesheet" type="text/css" />
	<script src="https://code.jquery.com/jquery-1.8.2.min.js"></script>
	<script src="https://code.jquery.com/mobile/1.2.0/jquery.mobile-1.2.0.min.js"></script>
</head>
<body>
<form id="form1" runat="server" data-ajax="false" data-theme="c">
	<!-- page -->
	<div data-role="page" id="Login">
		<!-- header -->
		<div data-role="header" data-position="fixed"> 
			<h1>MTA S/S</h1> 
		</div>   
		<!-- / header -->
		<!-- content --> 
		<div data-role="content">
			<input type="text" name="name" placeholder="User Name" ID="UserName" runat="server"/>
			<asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="UserName" Display="Dynamic" ErrorMessage="(required)" runat="server" />
			<input type="password" name="password" placeholder="Password" ID="UserPass" runat="server" />
			<asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="UserPass" Display="Dynamic" ErrorMessage="(required)" runat="server" />
			<asp:CheckBox ID="PersistCookie" runat="server" Text="&nbsp;Remember Me" />
			<asp:Button ID="cmdLogin" Text="Login" runat="server" OnClick="cmdLogin_Click" data-theme="a" />

			<p class="ui-body ui-body-c message" runat="server" id="messageContainer" ><asp:Label ID="lblResults" runat="server"></asp:Label></p>
		</div>
		<!-- / content -->
	</div>
	<!-- / page -->
</form>
</body>
</html>
