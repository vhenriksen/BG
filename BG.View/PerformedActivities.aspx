<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PerformedActivities.aspx.cs" Inherits="BG.View.PerformedActivities" %>

<%@ Import Namespace="System.Diagnostics" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title></title>
    <link rel="stylesheet" href="http://code.jquery.com/mobile/1.1.0/jquery.mobile-1.1.0.min.css" />
    <link href="Style.css" type="text/css" rel="stylesheet" />
    <link rel="stylesheet" href="themes/bg.min.css" />
    <style>
        /* App custom styles */
    </style>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript">
        </script>
    <script src="FixPostback.js" type="text/javascript"></script>
    <script src="http://code.jquery.com/mobile/1.1.0/jquery.mobile-1.1.0.min.js" type="text/javascript">
        </script>
    
</head>
<body>
    <form id="form1" runat="server">
        <div data-role="page" id=" main" data-theme="a">
            <!-- titel -->
            <div data-role="header" align="center" id="header" role="banner">
                <h3 role="heading" runat="server" id="HeaderText">Performed activities</h3>
            </div>
            <div data-role="content">
                <div runat="server" ID="DivText" data-role="controlgroup"></div>
            </div>
            <div>
                <asp:Label runat="server" ID="LabelError" ForeColor="Red"></asp:Label>
            </div>
           <ul data-role="fieldcontain" data-inset="true">
                <asp:Button runat="server" Text="Back" ID="ButtonBack" OnClick="ButtonBack_OnClick" data-icon="back" data-theme="b" data-inline="true" />
                <asp:Button runat="server" Text="OK" ID="ButtonAccept" OnClick="ButtonAccept_OnClick" data-icon="check" data-inline="true"/>
            </ul>
        </div>
    </form>
</body>
</html>
