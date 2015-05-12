<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="oldRoutine.aspx.cs" Inherits="BG.View.oldRoutine" %>

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
    <form id="MainForm" runat="server" data-ajax="false">
        <!-- side -->
        <div data-role="page" id=" main" data-theme="a">
            <!-- titel -->
            <div data-role="header" align="center" id="header" role="banner">
                <h2 role="heading">Old routine</h2>
            </div>
            <div>
                <h3 role="heading" runat="server" id="headRoutinename"></h3>
            </div>
            <div data-role="fieldcontain">
                <div data-role="fieldcontain">
                    <h4>From date</h4>
                    <div data-role="fieldcontain">
                        <asp:Label runat="server" Text="Day"></asp:Label><br/>
                        <asp:DropDownList runat="server" ID="DropdownlistFromDateDay"/><br/>
                        <asp:Label runat="server" Text="Month"></asp:Label><br/>
                        <asp:DropDownList runat="server" ID="DropdownlistFromDateMonth"/><br/>
                        <asp:Label runat="server" Text="Year"></asp:Label><br/>
                        <asp:DropDownList runat="server" ID="DropdownlistFromDateYear"/>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <h4>To date</h4>
                    <div data-role="fieldcontain">
                        <asp:Label runat="server" Text="Day"></asp:Label><br/>
                        <asp:DropDownList runat="server" ID="DropdownlistToDateDay"/><br/>
                        <asp:Label runat="server" Text="Month"></asp:Label><br/>
                        <asp:DropDownList runat="server" ID="DropdownlistToDateMonth"/><br/>
                        <asp:Label runat="server" Text="Year"></asp:Label><br/>
                        <asp:DropDownList runat="server" ID="DropdownlistToDateYear"/>
                    </div>
                </div>
            </div>
            <div>
                <asp:Label runat="server" ForeColor="red" ID="LabelError"></asp:Label>
            </div>
            <div>
                <li data-role="listview">
                    <ul data-role="fieldcontain" data-inset="true">
                        <asp:Button runat="server" ID="ButtonAcceptOldRoutine" Text="Accept" data-icon="check" data-inline="true" data-theme="b" OnClick="ButtonAcceptOldRoutine_OnClick"/>
                        <asp:Button runat="server" ID="ButtonBack" Text="Back" data-icon="back" data-inline="true" data-theme="b"/>
                    </ul>
                </li>
            </div>
        </div>
    </form>
</body>
</html>
