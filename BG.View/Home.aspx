<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="BG.View.Home" Debug="true" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
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
    <script>
        function UploadToEconomic() {
            var text = "Uploading to economic. When it is done uploading, it will return to login page. You can press OK.";
            alert(text);
        }
    </script>
</head>
<body>
    <!-- eksempler på elementer: http://jquerymobile.com/demos/1.1.0/ -->
    <form id="MainForm" runat="server" data-ajax="false">
        <!-- side -->
        <div data-role="page" id="main" data-theme="a">
            <!-- titel -->
            <div data-role="header" id="header">
                <h3 id="Head">
                    <asp:Label ID="LabelUsername" runat="server"></asp:Label></h3>
            </div>
            <!-- indhold -->
            <div data-role="content">
                <li data-role="listview">
                    <ul data-role="fieldcontain">
                        <asp:Button ID="ButtonDayStart" runat="server" Text="Set start time" OnClick="ButtonDayStartedClick" />
                        <asp:Button ID="ButtonDayEnd" runat="server" Text="Set end time" OnClick="ButtonDayEndedClick" />
                    </ul>
                    <ul data-role="fieldcontain">
                        <div data-role="controlgroup" data-mini="false">
                            <asp:Button runat="server" ID="ButtonRutines" Text="Routines" OnClick="ButtonRutinesClick" />
                        </div>
                    </ul>
                    <ul data-role="fieldcontain">
                        <ul id="ListViewTasks" runat="server" data-role="listview" data-divider-theme="b"
                            data-inset="true">
                            <li data-role="list-divider" role="heading">Tasks</li>
                            <li data-theme="a">
                                <asp:Button ID="ButtonNewTask" runat="server" Text="New task" OnClick="ButtonNewTaskClick" />
                                <div data-role="controlgroup" data-mini="false">
                                    <asp:Button runat="server" ID="ButtonManageRutines" Text="Administration" OnClick="ButtonManageRutinesClick" />
                                </div>
                            </li>
                            <%  
                                DateTime lastEndtime = new DateTime();

                                if (currentDailyreport.Break.BreakStarted.HasValue)
                                {
                                    if (currentDailyreport.DayStarted != null)
                                    {
                                        lastEndtime = currentDailyreport.DayStarted.Value;
                                    }
                                    //Alle før pause
                                    foreach (
                                        var taskBeforeBreak in
                                            tasks.Where(t => t.TimeEnded <= currentDailyreport.Break.BreakStarted).
                                                OrderBy(
                                                    t => t.TimeStarted))
                                    {
                                        //Check for tidsrum inden task
                                        if (taskBeforeBreak.TimeStarted != lastEndtime)
                                        {
                            %>
                            <li data-icon="info" data-role="list-divider" data-theme="a">
                                <a href="NewTaskType.aspx?time=<%=
                                    lastEndtime.Date.ToString()+"t"+
    lastEndtime.Hour.ToString() + "t" +
    lastEndtime.Minute.ToString() + "t" +
    taskBeforeBreak.TimeStarted.Value.Date.ToString()+"t"+
    taskBeforeBreak.TimeStarted.Value.Hour.ToString() + "t" +
    taskBeforeBreak.TimeStarted.Value.Minute.ToString() %>"
                                    style="background-color: blue">
                                    <%= lastEndtime.ToShortTimeString() + "-" +
                         taskBeforeBreak.TimeStarted.Value.ToShortTimeString() + " Create new task here" %>
                                </a>
                            </li>
                            <%
                                        }

                                        //Indsæt task
                                        string text =
                                            taskBeforeBreak.TimeStarted.Value.ToShortTimeString() + "-"
                                            + taskBeforeBreak.TimeEnded.Value.ToShortTimeString() + " "
                                            + taskBeforeBreak.EconomicProject.Name + " "
                                            + taskBeforeBreak.EconomicTask.Name;

                            %>
                            <li data-icon="info" data-role="list-divider" data-theme="a">
                                <a href="RUDTaskType.aspx?taskID=<%= taskBeforeBreak.Id%>"><%= text %></a>
                            </li>
                            <%
                                        lastEndtime = taskBeforeBreak.TimeEnded.Value;
                                    }
                                    //Check for tidsrum før pause
                                    if (lastEndtime != currentDailyreport.Break.BreakStarted)
                                    {
                            %>
                            <li data-icon="info" data-role="list-divider" data-theme="a">
                                <a href="NewTaskType.aspx?time=<%=lastEndtime.Date.ToString()+"t"+lastEndtime.Hour.ToString() + "t" +lastEndtime.Minute.ToString() + "t" +
    currentDailyreport.Break.BreakStarted.Value.Date.ToString()+"t"+currentDailyreport.Break.BreakStarted.Value.Hour.ToString() + "t" +currentDailyreport.Break.BreakStarted.Value.Minute.ToString() %>"
                                    style="background-color: blue">
                                    <%= lastEndtime.ToShortTimeString() + "-" +
                         currentDailyreport.Break.BreakStarted.Value.ToShortTimeString() + " Create new task here" %>
                                </a>
                            </li>
                            <%
                                    }

                                    //Pause
                            %>
                            <li data-role="list-divider" data-theme="a">
                                <div data-role="header" data-theme="a" class="ui-bar" align="center">
                                    <div>
                                        <asp:Button runat="server" data-icon="arrow-u" OnClick="ButtonPauseUpClick" data-theme="e" Text="UP"></asp:Button>
                                        <asp:Label runat="server" ID="LabelPause" Font-Size="15" ForeColor="black" data-theme="e"></asp:Label>
                                        <asp:Button runat="server" data-icon="arrow-d" data-theme="e" OnClick="ButtonPauseDownClick" Text="DOWN"></asp:Button>
                                    </div>
                                </div>
                            </li>
                            <%
                                    lastEndtime = currentDailyreport.Break.BreakEnded.Value;

                                    var tasksAfterBreak =
                                        currentDailyreport.Task.Where(
                                            t => t.TimeStarted >= currentDailyreport.Break.BreakEnded)
                                            .OrderBy(t => t.TimeStarted).ToList();
                                    //Tasks efter pause
                                    foreach (var taskBeforeBreak in tasksAfterBreak)
                                    {
                                        //Check for tidsrum imellem tasks
                                        if (lastEndtime != taskBeforeBreak.TimeStarted)
                                        {
                            %>
                            <li data-icon="info" data-role="list-divider" data-theme="a">
                                <a href="NewTaskType.aspx?time=<%=lastEndtime.Date.ToString()+"t"+lastEndtime.Hour.ToString() + "t" +lastEndtime.Minute.ToString() + "t" +
    taskBeforeBreak.TimeStarted.Value.Date.ToString()+"t"+taskBeforeBreak.TimeStarted.Value.Hour.ToString() + "t" +taskBeforeBreak.TimeStarted.Value.Minute.ToString() %>"
                                    style="background-color: blue">
                                    <%= lastEndtime.ToShortTimeString() + "-" +
                         taskBeforeBreak.TimeStarted.Value.ToShortTimeString() + " Create new task here" %>
                                </a>
                            </li>
                            <%
                                        }

                                        //Indsæt task
                                        string text =
                                            taskBeforeBreak.TimeStarted.Value.ToShortTimeString() + "-"
                                            + taskBeforeBreak.TimeEnded.Value.ToShortTimeString() + " "
                                            + taskBeforeBreak.EconomicProject.Name + " "
                                            + taskBeforeBreak.EconomicTask.Name;

                            %>
                            <li data-icon="info" data-role="list-divider" data-theme="a">
                                <a href="RUDTaskType.aspx?taskID=<%= taskBeforeBreak.Id %>"><%= text %></a>
                            </li>
                            <%
                                        lastEndtime = taskBeforeBreak.TimeEnded.Value;
                                    }
                                    //Tidsrum imellem sidste task og slut
                                    if (currentDailyreport.DayEnded.HasValue)
                                    {
                                        if (lastEndtime != currentDailyreport.DayEnded.Value)
                                        {
                            %>
                            <li data-icon="info" data-role="list-divider" data-theme="a">
                                <a href="NewTaskType.aspx?time=<%=lastEndtime.Date.ToString()+"t"+lastEndtime.Hour.ToString() + "t" +lastEndtime.Minute.ToString() + "t" +
    currentDailyreport.DayEnded.Value.Date.ToString()+"t"+currentDailyreport.DayEnded.Value.Hour.ToString() + "t" +currentDailyreport.DayEnded.Value.Minute.ToString() %>"
                                    style="background-color: blue">
                                    <%= lastEndtime.ToShortTimeString() + "-" +
                         currentDailyreport.DayEnded.Value.ToShortTimeString() + " Create new task here" %>
                                </a>
                            </li>
                            <%
                                        }
                                    }
                                }
                            %>
                        </ul>
                    </ul>
                    <ul data-role="fieldcontain" data-inset="true">
                        <asp:Button ID="ButtonWorkday" runat="server" Text="Workday" OnClick="ButtonWorkday_OnClick" />
                        <asp:Button ID="ButtonLogout" runat="server" Text="Log out" OnClick="ButtonLogoutClick" data-icon="back" data-theme="b" />
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
