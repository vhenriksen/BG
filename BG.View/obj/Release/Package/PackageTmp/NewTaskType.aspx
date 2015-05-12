<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="true" CodeBehind="NewTaskType.aspx.cs" Inherits="BG.View.NewTaskType" %>

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
            <li data-role="listview" data-inset="true">
                <ul data-role="fieldcontain" data-inset="true">
                    <!-- indhold -->
                    <asp:Label ID="Label1" runat="server" Text="Projects"></asp:Label>
                    <div data-role="content" style="overflow: auto; height: 200px">
                        <ul data-role="listview" data-filter="true" data-filter-placeholder="Search here..." data-inset="true">
                            <%
                                string text = "";
                                foreach (var economicProject in economicProjects)
                                {
                                    text = economicProject.Name;
                                
                            %>
                            <li data-icon="info" data-theme="a">
                                <a href="NewTaskType.aspx?projectID=<%= economicProject.Id %>&time=<%=selectedTime %>"><%= text %></a>
                            </li>
                            <%   
                                }
                            %>
                        </ul>
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
                        </asp:DropDownList><br/>
                         <asp:Label runat="server" Text="Suggested starttime: " ID="LabelSuggestedStarttime"></asp:Label><br/>
                    </div>
                    <asp:Label runat="server" ID="labelTimeEndHour" Text="End time"></asp:Label><br/>
                    <div data-role="controlgroup" data-type="horizontal">
                        <asp:DropDownList runat="server" ID="listboxTimeEndHour">
                        </asp:DropDownList>&nbsp;
                        <asp:DropDownList runat="server" ID="listboxTimeEndMin">
                        </asp:DropDownList><br/>
                        <asp:Label runat="server" Text="Suggested endtime: " ID="LabelSuggestedEndtime"></asp:Label><br/>
                        <asp:Label runat="server" Text="Pause: " ID="LabelPause"></asp:Label><br/>
                    </div>
                    <asp:Label runat="server" ID="labelErrorText" ForeColor="Red"></asp:Label>
                </ul>
                <ul data-role="fieldcontain">
                    <div data-role="collapsible" data-theme="a" data-content-theme="a">
                        <h2>Create opportunities</h2>
                        <div data-role="collapsible" data-theme="a" data-content-theme="a">
                            <h3>Task passes midnight</h3>
                            <asp:CheckBox runat="server" Text="Passes midnight" AutoPostBack="True" ID="CheckboxPassesMidnight" OnCheckedChanged="CheckboxPassesMidnightOnCheckedChanged" Checked="False"/>
                             <asp:Label ID="LabelExample" runat="server" Text="Ex: 22:00-01:00" Visible="False"></asp:Label><br/>    
                            <asp:Label ID="LabelNewStart" runat="server" Text="Start date" Visible="False"></asp:Label><br/>
                            <asp:DropDownList runat="server" ID="DropdownListStartBeforeMidnight" Visible="False"/><br/>
                            <asp:Label ID="LabelNewEnd" runat="server" Text="End date" Visible="False"></asp:Label><br/>
                            <asp:DropDownList runat="server" ID="DropdownListEndAfterMidnight" Visible="False"/><br/>
                        </div>
                        <div data-role="collapsible" data-theme="a" data-content-theme="a">
                            <h3>Move tasks</h3>
                            <asp:CheckBox runat="server" Text="Move" ID="CheckBoxMoveTasks"/>
                        </div>
                     </div>
                </ul>
                <ul data-role="fieldcontain">
                    <asp:Button runat="server" ID="ButtonOk" Text="OK" OnClick="ButtonOk_Click" data-icon="check" data-inline="true" />
                    <asp:Button runat="server" ID="ButtonCancel" Text="Cancel" OnClick="ButtonCancel_Click" data-icon="home" data-inline="true" data-theme="b"/>
                </ul>
            </li>
        </div>
        <script>
        /* app custom scripts */
        </script>
    </form>
</body>
</html>
