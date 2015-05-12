<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RUDTaskType.aspx.cs" Inherits="BG.View.RUDTaskType" %>

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
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript">
    </script>
    <script src="FixPostback.js" type="text/javascript"></script>
    <script src="http://code.jquery.com/mobile/1.1.0/jquery.mobile-1.1.0.min.js" type="text/javascript">
    </script>
</head>
<body>
    <!-- eksempler på elementer: http://jquerymobile.com/demos/1.1.0/ -->
    <form id="MainForm" runat="server" data-ajax="false">
        <!-- side -->
        <div data-role="page" id=" main" data-theme="a">
            <!-- titel -->
            <div data-role="header" id="header" role="banner">
                <h3 role="heading">Task type</h3>
            </div>
            <!-- indhold -->
            <div data-role="content">
                <li data-role="listview">
                    <ul data-role="fieldcontain">
                        <!-- indhold -->
                        <asp:Label runat="server" ID="labelProject" Text="Projects"></asp:Label>
                        <div data-role="controlgroup" data-type="horizontal">
                            <asp:DropDownList runat="server" ID="listboxProjects" AutoPostBack="True" OnSelectedIndexChanged="ListboxProjectsSelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <div runat="server" id="divSelectedProject">
                            <asp:Label runat="server" ID="LabelSelectedProject"/>
                        </div>
                    </ul>
                    <ul data-role="fieldcontain">
                        <asp:Label runat="server" ID="labelTaskType" Text="Task type"></asp:Label>
                        <div data-role="controlgroup" data-type="horizontal">
                            <asp:DropDownList runat="server" ID="listboxTaskTypes">
                            </asp:DropDownList>
                        </div>
                    </ul>
                    <ul data-role="fieldcontain">
                        <asp:Label runat="server" ID="labelTimeStart" Text="Start time"></asp:Label>
                        <div data-role="controlgroup" data-type="horizontal">
                            <asp:DropDownList runat="server" ID="listboxTimeStartHour">
                            </asp:DropDownList>&nbsp;
                            <asp:DropDownList runat="server" ID="listboxTimeStartMin">
                            </asp:DropDownList><br />
                            <asp:Label runat="server" Text="" ID="LabelTasktypeStartTime"></asp:Label>
                        </div>
                        <asp:Label runat="server" ID="labelTimeEndHour" Text="End time"></asp:Label><br />
                        <div data-role="controlgroup" data-type="horizontal">
                            <asp:DropDownList runat="server" ID="listboxTimeEndHour">
                            </asp:DropDownList>&nbsp;
                            <asp:DropDownList runat="server" ID="listboxTimeEndMin">
                            </asp:DropDownList><br />
                            <asp:Label runat="server" Text="" ID="LabelTasktypeEndTime"></asp:Label>
                        </div>
                    </ul>
                    <ul>
                        <div data-role="controlgroup" data-type="horizontal">
                            <asp:Label runat="server" ID="labelErrorText" ForeColor="Red"></asp:Label>
                        </div>
                    </ul>
                    <ul data-role="fieldcontain">
                        <div data-role="collapsible" data-theme="a" data-content-theme="a">
                            <h2>Update opportunities</h2>
                            <div data-role="collapsible" data-theme="a" data-content-theme="a">
                                <h3>Task passes midnight</h3>
                                <asp:CheckBox runat="server" Text="Passes midnight" AutoPostBack="True" ID="CheckboxPassesMidnight" OnCheckedChanged="CheckboxPassesMidnightOnCheckedChanged" Checked="False" />
                                <asp:Label ID="LabelExample" runat="server" Text="Ex: 22:00-01:00" Visible="False"></asp:Label><br />
                                <asp:Label ID="LabelNewStart" runat="server" Text="Start date" Visible="False"></asp:Label><br />
                                <asp:DropDownList runat="server" ID="DropdownListStartBeforeMidnight" Visible="False" /><br />
                                <asp:Label ID="LabelNewEnd" runat="server" Text="End date" Visible="False"></asp:Label><br />
                                <asp:DropDownList runat="server" ID="DropdownListEndAfterMidnight" Visible="False" /><br />
                            </div>
                            <div data-role="collapsible" data-theme="a" data-content-theme="a">
                                <h3>Move tasks</h3>
                                <asp:CheckBox runat="server" ID="CheckBoxMove" Text="Move" />
                            </div>
                        </div>
                    </ul>
                    <ul data-role="fieldcontain" data-inset="true">
                        <asp:Button runat="server" ID="ButtonOk" Text="Update" OnClick="ButtonUpdate_Click" data-icon="check" data-inline="true" />
                        <asp:Button runat="server" ID="ButtonCancel" Text="Cancel" OnClick="ButtonCancel_Click" data-icon="home" data-inline="true" data-theme="b"/>
                        <asp:Button runat="server" ID="ButtonDelete" Text="Delete" OnClick="ButtonDelete_OnClick" data-icon="delete" data-inline="true" />
                        <asp:Button runat="server" ID="ButtonEmail" Text="e-mail" OnClick="ButtonEmail_Click" data-icon="star" data-inline="true" data-theme="e"/>
                    </ul>
                </li>
            </div>
        </div>
        <script>
            /* app custom scripts */
        </script>
    </form>
</body>
</html>
