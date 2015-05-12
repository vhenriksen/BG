<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DayStartedEnded.aspx.cs"
    Inherits="BG.View.DayStartedEnded" %>

<!DOCTYPE html>
<html>
<head id="Head" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title></title>
    <link rel="stylesheet" href="http://code.jquery.com/mobile/1.1.0/jquery.mobile-1.1.0.min.css" />
    <link href="Style.css" type="text/css" rel="stylesheet" />
    <link rel="stylesheet" href="themes/bg.min.css" />
    <style>
        /* App custom styles */
    </style>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js">
    </script>
    <script src="FixPostback.js" type="text/javascript"></script>
    <script src="http://code.jquery.com/mobile/1.1.0/jquery.mobile-1.1.0.min.js">
    </script>
</head>
<body>
    <!-- eksempler på elementer: http://jquerymobile.com/demos/1.1.0/ -->
    <form id="MainForm" runat="server">
        <!-- side -->
        <div data-role="page" id="main" data-theme="a">
            <!-- titel -->
            <div data-role="header" id="header">
                <h3 runat="server" id="headertext">Start time</h3>
            </div>
            <!-- indhold -->
            <div data-role="content">
                <ul data-role="listview">
                    <li data-role="fieldcontain">
                        <div data-role="controlgroup" data-type="horizontal" runat="server" id="DivStarttime1">
                            <asp:Label runat="server" ID="labelStartDate" Text="Start Day"></asp:Label><br />
                            <asp:DropDownList runat="server" ID="DropDownStartDate">
                            </asp:DropDownList>
                        </div>
                        <div data-role="controlgroup" data-type="horizontal" runat="server" id="DivStarttime2">
                            <table>
                                <tr>
                                    <td align="center">
                                        <asp:Label runat="server" ID="labelStartHour" Text="Hour"></asp:Label>
                                    </td>
                                    <td align="center">
                                        <asp:Label runat="server" ID="labelStartMin" Text="Minute"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DropDownList runat="server" ID="listboxTimeStartHour">
                                        </asp:DropDownList>&nbsp;
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="listboxTimeStartMin">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <asp:Label ID="LabelStartTimeError" ForeColor="Red" runat="server" Text=""></asp:Label>

                        </div>

                        <div data-role="controlgroup" data-type="horizontal" runat="server" id="DivEndtime1">
                            <asp:Label runat="server" ID="labelDayEnd" Text="End Day"></asp:Label><br />
                            <asp:DropDownList runat="server" ID="DropDownEndDate">
                            </asp:DropDownList>
                        </div>
                        <div data-role="controlgroup" data-type="horizontal" runat="server" id="DivEndtime2">
                            <table>
                                <tr>
                                    <td align="center">
                                        <asp:Label runat="server" ID="labelEndHour" Text="Hour"></asp:Label>
                                    </td>
                                    <td align="center">
                                        <asp:Label runat="server" ID="labelEndMin" Text="Minute"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DropDownList runat="server" ID="listboxTimeEndHour">
                                        </asp:DropDownList>&nbsp;
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="listboxTimeEndMin">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <asp:Label ID="LabelEndTimeError" ForeColor="Red" runat="server" Text=""></asp:Label>
                        </div>
                    </li>
                <li data-role="fieldcontain">
                    <div data-role="collapsible" data-theme="a" data-content-theme="a">
                        <h4>Change start opportunities</h4>
                        <asp:CheckBox runat="server" ID="CheckBoxMoveTasks" Text="Move other tasks" />
                    </div>
                </li>
                <ul data-role="fieldcontain" data-inset="true">
                        <asp:Button runat="server" ID="ButtonOk" Text="OK" OnClick="ButtonOk_Click" data-icon="check" data-inline="true" />&nbsp;
                        <asp:Button runat="server" ID="ButtonCancel" Text="Cancel" OnClick="ButtonCancel_Click" data-icon="home" data-inline="true" data-theme="b"/>
                    
                </ul>
            </ul>
        </div>
        </div>
    <script>
        /* app custom scripts */
    </script>
    </form>
</body>
</html>
