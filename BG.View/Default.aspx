<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BG.View.Default" %>

<!DOCTYPE html>
<html>
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
    <script type="text/javascript">
        $(document).ready(function () {
            $.ajax({
                url: "RefreshCache.aspx",
                cache: false
            }).done(function (html) {
                $("#loadIcon").remove();
                $("#cacheStatus").text("Cache is up to date! (Last updated: " + html + ")");
                
            });
            $('#TextBoxUsername').focus();
        });
    </script>
</head>
<body>
    <!-- eksempler på elementer: http://jquerymobile.com/demos/1.1.0/ -->
    <form id="MainForm" runat="server" data-ajax="false">
        <!-- log ind side -->
        <div data-role="page" id="login" data-theme="a">
            <!-- indhold -->
            <div data-role="content">
                <center><img id="logo" alt="Logo" src="Image/BG.PNG"/></center>
                <asp:Label ID="LabelLoginError" Font-Bold="true" ForeColor="Red" Visible="false"
                    runat="server"></asp:Label>
                <br />
                <asp:Label runat="server" Text="Username"></asp:Label>
                <asp:TextBox ID="TextBoxUsername" runat="server" type="text"></asp:TextBox>
                <asp:Label runat="server" Text="Password"></asp:Label>
                <asp:TextBox ID="TextBoxPassword" runat="server" TextMode="Password"></asp:TextBox>
                <asp:Button ID="ButtonLogin" data-icon="forward" data-iconpos="right" runat="server"
                    Text="Login" OnClick="ButtonLogin_Click" />
                <center><table id="loader"><tr><td id="loadIcon"><img src="Image/Loader.gif"/></td><td id="cacheStatus">Updating cache status ...</td></tr></table></center>
            </div>
        </div>
        <script>
            
        </script>
    </form>
</body>
</html>
