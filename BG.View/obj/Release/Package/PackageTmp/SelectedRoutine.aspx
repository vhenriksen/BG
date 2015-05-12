<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectedRoutine.aspx.cs" Inherits="BG.View.SelectedRoutine" Debug="true" %>

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
        <div data-role="page" id="main" data-theme="a">
            <!-- titel -->
            <div data-role="header" align="center" id="header" role="banner">
                <h3 role="heading" runat="server" id="HeaderText">Activities</h3>
            </div>
            <li data-role="listview" >
                 <div runat="server" id="divCheckbox"></div>
              <ul data-role="fieldcontain" data-inset="true">
                    <asp:Button runat="server" Text="Back" ID="ButtonDeclineRoutine" data-theme="b" data-icon="back" data-inline="true" OnClick="ButtonDeclineRoutineClick" />
                    <asp:Button runat="server" Text="OK" ID="ButtonAcceptRoutine" data-icon="check" data-inline="true" OnClick="ButtonAcceptRoutineClick" />
                </ul>
            </li>
        </div>
    </form>
</body>
</html>
