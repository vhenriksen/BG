<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="workday.aspx.cs" Inherits="BG.View.workday" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link rel="stylesheet" href="http://code.jquery.com/mobile/1.1.0/jquery.mobile-1.1.0.min.css" />
    <link href="Style.css" type="text/css" rel="stylesheet" />
    <link rel="stylesheet" href="themes/bg.min.css" />
    <style>
        /* App custom styles */
    </style>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript"></script>
    <script src="FixPostback.js" type="text/javascript"></script>
    <script src="http://code.jquery.com/mobile/1.1.0/jquery.mobile-1.1.0.min.js" type="text/javascript"></script>
    <script>
        function UploadToEconomic() {
            var text = "Uploading to economic. When it is done uploading, it will return to login page. You can press OK.";
            alert(text);
        }
    </script>
</head>
<body>
    <form id="MainForm" runat="server" data-ajax="false">
        <!-- side -->
        <div data-role="page" id=" main" data-theme="a">
            <!-- titel -->
            <div data-role="header" role="banner">
                <h3 align="center" role="heading">Workday</h3>
            </div>
            <div>
                <div>
                    <li data-role="listview">
                        <ul data-role="fieldcontain" data-inset="true">
                            <asp:Button runat="server" Text="View workday" ID="ButtonViewWorkday" OnClick="ButtonViewWorkday_OnClick" />
                        </ul>
                    </li>
                </div>
                <div data-role="header" runat="server" id="divWorkday"></div>
                <div runat="server" id="divData"></div>
                <div>
                    <li data-role="listview">
                        <ul data-role="fieldcontain" data-inset="true">
                            <asp:Button runat="server" Text="Upload workday" data-inline="true" data-icon="check" ID="ButtonUploadWorkday" OnClientClick="UploadToEconomic()" Enabled="False" OnClick="ButtonUploadWorkday_OnClick" />
                        </ul>
                    </li>
                </div>
                <div>
                    <asp:Label runat="server" ID="LabelWorkdayError" ForeColor="red"></asp:Label>
                </div>
            </div>
            <div data-role="header" role="banner">
                <h3 align="center" role="heading">Flashback</h3>
            </div>
            <div>
                <div>
                    <div>
                        <h3>Summary of monthly salary</h3>
                        <div>
                            <asp:Label runat="server" Font-Bold="true" Text="From:"></asp:Label>
                            <asp:Label runat="server" Font-Bold="true" ID="LabelFromDate"></asp:Label><br />
                            <asp:Calendar runat="server" ID="CalenderFromDate" OnSelectionChanged="CalenderFromDate_OnSelectionChanged"></asp:Calendar>
                        </div>
                        <br />
                        <div>
                            <asp:Label runat="server" Font-Bold="true" Text="To:"></asp:Label>
                            <asp:Label runat="server" Font-Bold="true" ID="LabelToDate"></asp:Label><br />
                            <asp:Calendar runat="server" ID="CalendarToDate" OnSelectionChanged="CalendarToDate_OnSelectionChanged"></asp:Calendar>
                        </div>

                        <div>
                            <asp:Button runat="server" Text="View flashback" ID="ButtonFlashback" OnClick="ButtonFlashback_OnClick" Enabled="false" /><br />
                        </div>
                        <div runat="server" id="tableViewOfMonthPay"></div>
                    </div>
                    <br />
                    <div>
                        <h3>Summary of 2 days</h3>
                        <div>
                            <asp:Label runat="server" Font-Bold="true" Text="From:"></asp:Label>
                            <asp:Label runat="server" Font-Bold="true" ID="LabelFromDateSumOfTwoDays"></asp:Label><br />
                            <asp:Calendar runat="server" ID="CalendarFromDateSumOfTwoDays" OnSelectionChanged="CalendarFromDateSumOfTwoDays_OnSelectionChanged"></asp:Calendar>
                        </div>
                        <br />
                        <div>
                            <asp:Label runat="server" Font-Bold="true" Text="To:"></asp:Label>
                            <asp:Label runat="server" Font-Bold="true" ID="LabelToDateSumOfTwoDays"></asp:Label><br />
                            <asp:Calendar runat="server" ID="CalendarToDateSumOfTwoDays" OnSelectionChanged="CalendarToDateSumOfTwoDays_OnSelectionChanged"></asp:Calendar>
                        </div>
                        <div>
                            <asp:Button runat="server" Text="View summary" ID="ButtonSummaryTwoDays" OnClick="ButtonSummaryTwoDays_OnClick" Enabled="False" />
                        </div>
                        <div runat="server" id="tableViewOfSummaryOfTwoDays"></div>
                    </div>
                </div>
                <br />
                <div>
                    <asp:Label runat="server" ForeColor="red" ID="LabelFlashbackError"></asp:Label>
                </div>
            </div>
            <div>
                <li data-role="listview">
                    <ul data-role="fieldcontain" data-inset="true">
                        <asp:Button runat="server" Text="Home" data-inline="true" data-icon="home" data-theme="b" ID="ButtonCancel" OnClick="ButtonCancel_OnClick" />
                    </ul>
                </li>
            </div>
        </div>
    </form>
</body>
</html>
