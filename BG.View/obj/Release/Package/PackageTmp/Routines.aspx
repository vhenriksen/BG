<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Routines.aspx.cs" Inherits="BG.View.Routines" Debug="true" %>

<%@ Import Namespace="BG.Model" %>

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
                <h3 role="heading">Routines</h3>
            </div>
            <li data-role="listview">
                <ul data-role="fieldcontain">
                    <ul runat="server" data-role="listview" data-divider-theme="b" data-inset="true">
                        <li data-role="list-divider" role="heading">Fixed routines</li>
                        <%
                            foreach (var fixedRoutine in fixedRoutines)
                            {
                        %>
                        <li data-role="list-divider" data-content-theme="a">
                            <a href="SelectedRoutine.aspx?routineId=<%= fixedRoutine.Id%>"><%=fixedRoutine.Name%></a>
                        </li>
                        <%
                            }
                        %>
                    </ul>
                </ul>
                <ul data-role="fieldcontain">
                    <ul runat="server" data-role="listview" data-divider-theme="b" data-inset="true">
                        <li data-role="list-divider" role="heading">Old routines</li>
                        <%
                            foreach (var oldRoutinen in oldRoutines)
                            {
                        %>
                        <li data-role="list-divider" data-theme="b">
                            <a href="oldRoutine.aspx?oldRoutineId=<%= oldRoutinen.Id%>"><%= oldRoutinen.Name %></a>
                        </li>
                        <%
                            }
                        %>
                    </ul>
                </ul>
                <ul>
                    <asp:Label runat="server" ID="LabelRoutinesError" ForeColor="Red"></asp:Label>
                </ul>
                <ul data-role="fieldcontain" data-inset="true">
                    <asp:Button runat="server" Text="Home" ID="ButtonHome" OnClick="ButtonHomeClick" data-icon="home" data-theme="b" />
                </ul>
            </li>
        </div>
    </form>
</body>
</html>
