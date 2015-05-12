<%@ Page Language="C#" AutoEventWireup="true" Debug="true" CodeBehind="ManageRoutines.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="BG.View.ManageRoutines" %>

<%@ Import Namespace="BG.Controller" %>

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
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript">
    </script>
    <script src="FixPostback.js" type="text/javascript"></script>
    <script src="http://code.jquery.com/mobile/1.1.0/jquery.mobile-1.1.0.min.js" type="text/javascript">
    </script>
    <script>
        function UpdateCache() {
            alert("Cache is being updated. Press OK.");
            document.getElementById("LabelUpdateCache").innerText = "Updating...";
        }
    </script>

</head>
<body>
    <form id="MainForm" runat="server" data-ajax="false">
        <!-- side -->
        <div data-role="page" id=" main" data-theme="a">
            <!-- titel -->
            <table>
                <tr>
                    <td colspan="6">
                        <div data-role="header" id="header" role="banner">
                            <h3 align="center" role="heading">Manage routines</h3>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Routinename:"></asp:Label></td>
                    <td colspan="5">
                        <asp:TextBox runat="server" ID="TextBoxRoutinename" OnTextChanged="TextboxRoutinenameTextChanged" /></td>
                </tr>
                <tr>
                    <td colspan="6" align="left">
                        <h4>Occurrence</h4>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Date"></asp:Label></td>
                    <td colspan="5">
                        <asp:Label runat="server" Text="Selected dates to this routine"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:Calendar runat="server" ID="CalendarRoutineDates" OnSelectionChanged="CalendarRoutineDatesSelectionChanged">
                            <WeekendDayStyle ForeColor="yellowgreen" />
                            <SelectedDayStyle ForeColor="green" />
                        </asp:Calendar>
                    </td>
                    <td valign="top" colspan="5">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label runat="server" Text="From: "></asp:Label></td>
                                <td>
                                    <asp:Label runat="server" Text="" ID="LabelSelectedDateToRoutineFrom"></asp:Label></td>

                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" Text="To: "></asp:Label></td>
                                <td>
                                    <asp:Label runat="server" Text="" ID="LabelSelectedDateToRoutineTo"></asp:Label></td>
                            </tr>

                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="6" align="left">
                        <h4>Route</h4>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Select route: "></asp:Label></td>
                    <td>
                        <asp:DropDownList runat="server" AutoPostBack="true" ID="DropdownlistRoutes" DataValueField="Id" OnSelectedIndexChanged="DropdownlistRoutes_SelectedIndexChanged"/></td>
                </tr>
                <tr>
                    <td colspan="6" align="left">
                        <h4>Select an employee</h4>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:Label ID="Label2" runat="server" Text="Employee:"></asp:Label></td>
                    <td valign="top">
                        <asp:DropDownList runat="server" ID="DropdownlistEmployeeRoutine" AutoPostBack="True" DataValueField="Id" OnSelectedIndexChanged="DropdownlistEmployeeOnSelectedIndexChanged" /></td>
                    <td valign="top" colspan="4">
                        <asp:GridView runat="server" ID="GridviewAddEmployee" CellPadding="4" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="Id" PageSize="3"
                            OnRowDeleting="GridviewAddEmployeeOnRowDeleting" AutoGenerateDeleteButton="True" OnPageIndexChanging="GridviewAddEmployee_PageIndexChanging" ForeColor="#333333" GridLines="None">
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:BoundField HeaderText="Employee" DataField="Username" />
                            </Columns>
                            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                            <SortedAscendingCellStyle BackColor="#FDF5AC" />
                            <SortedAscendingHeaderStyle BackColor="#4D0000" />
                            <SortedDescendingCellStyle BackColor="#FCF6C0" />
                            <SortedDescendingHeaderStyle BackColor="#820000" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Label runat="server" ID="LabelErrorAddEmployee" ForeColor="red"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="6" align="left">
                        <h4>Create tasks to this routine</h4>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Project:"></asp:Label></td>
                    <td valign="top">
                        <asp:DropDownList runat="server" ID="DropdownlistAllProjects" AutoPostBack="True" OnSelectedIndexChanged="DropdownlistAllProjectsSelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:Label runat="server" Text="Task:"></asp:Label></td>
                    <td valign="top">
                        <asp:DropDownList runat="server" ID="DropdownlistTasks" /></td>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:Label runat="server" Text="Hours:"></asp:Label></td>
                    <td valign="top">
                        <asp:DropDownList runat="server" ID="DropdownlistHourBeforeComma" /></td>
                    <td valign="top" colspan="2">
                        <asp:DropDownList runat="server" ID="DropdownlistHourAfterComma" /></td>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:Button runat="server" Text="Add" ID="ButtonAddNewRoutineToList" OnClick="ButtonAddActivityToRoutineList" data-icon="plus" /></td>
                    <td valign="top">
                        <asp:Button runat="server" Text="Reset" OnClick="ButtonCancelActivityClick" /></td>
                    <td valign="top" colspan="2">
                        <asp:Button runat="server" Text="Update" ID="ButtonUpdateActivity" OnClick="ButtonUpdateActivityClick" Enabled="False" data-theme="e" data-icon="refresh" /></td>
                </tr>
                <tr>
                    <td colspan="6">
                        <asp:Label runat="server" ForeColor="Red" ID="LabelErrorRoutine"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="6" align="left">
                        <h5>Tasks</h5>
                    </td>
                </tr>
                <tr>
                    <td colspan="6" valign="top">
                        <asp:GridView runat="server" AllowPaging="True" OnPageIndexChanging="GridviewActivitiesPageIndexChanging" OnSelectedIndexChanging="GridviewActivitiesOnSelect"
                            OnRowDeleting="GridviewActivitiesOnDeleting" DataKeyNames="Id" ID="GridviewTasksToThisRoutine" AutoGenerateColumns="False" AutoGenerateDeleteButton="True" AutoGenerateSelectButton="True" CellPadding="4" ForeColor="#333333" GridLines="None">
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:BoundField HeaderText="Project" DataField="EconomicProject.Name" />
                                <asp:BoundField HeaderText="Task" DataField="EconomicTaskType.Name" />
                                <asp:BoundField HeaderText="Hours" DataField="Minutes" />
                            </Columns>
                            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                            <SortedAscendingCellStyle BackColor="#FDF5AC" />
                            <SortedAscendingHeaderStyle BackColor="#4D0000" />
                            <SortedDescendingCellStyle BackColor="#FCF6C0" />
                            <SortedDescendingHeaderStyle BackColor="#820000" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td colspan="6" align="left">
                        <h4>Create this routine and associate to an employee</h4>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="ButtonCreateAndAssociateRoutine" OnClick="ButtonCreateAndAssociateRoutineClick" runat="server" Text="Associate" data-icon="check" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4" align="left" valign="top">
                        <h4>All routines</h4>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Button runat="server" Text="View all routines" ID="ButtonViewAllRoutines" OnClick="ButtonViewAllRoutines_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="7">
                        <div data-role="header" id="Div1" role="banner">
                            <h3 align="center" role="heading">Manage groups</h3>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="6" align="left">
                        <h4>Group the employees</h4>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Choose group"></asp:Label></td>

                </tr>
                <tr>
                    <td colspan="4">
                        <asp:DropDownList runat="server" ID="DropdownlistAllGroups" AutoPostBack="True" OnSelectedIndexChanged="DropdownlistAllGroups_SelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td colspan="6">
                        <%--<div runat="server" id="divCheckbox"></div>--%>
                        <asp:GridView ID="GridviewGroupEmployee" runat="server" AutoGenerateColumns="False" DataKeyNames="IsMember, EmployeeId">
                            <Columns>
                                <asp:BoundField DataField="Employee" HeaderText="Employee" />
                                <asp:TemplateField HeaderText="Is member of group">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CheckBoxIsMember" runat="server" Checked='<%# Bind("IsMember") %>' Enabled="true" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="EmployeeId" Visible="False" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Label runat="server" ID="LabelErrorGroup" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" align="center">
                        <asp:Button runat="server" Text="Update" ID="ButtonUpdate" OnClick="ButtonUpdate_Click" data-theme="e" /></td>
                </tr>
                <tr>
                    <td colspan="6">
                        <div data-role="header" id="Div2" role="banner">
                            <h3 align="center" role="heading">Manage employee</h3>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="5">
                        <asp:TextBox runat="server" ID="TextboxEconomicId"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Username: "></asp:Label>
                    </td>
                    <td colspan="5">
                        <asp:TextBox runat="server" ID="TextBoxUsername"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Password: "></asp:Label>
                    </td>
                    <td colspan="5">
                        <asp:TextBox runat="server" ID="TextboxPassword"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="5">
                        <asp:CheckBox runat="server" Text="Is admin" ID="CheckBoxIsAdmin" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button runat="server" Text="Accept" ID="ButtonAcceptEmployee" OnClick="ButtonAccepEmployeeClick" /></td>
                    <td>
                        <asp:Button runat="server" Text="Update" ID="ButtonUpdateEmployee" OnClick="ButtonUpdateEmployeeClick" data-theme="e" /></td>
                    <td>
                        <asp:Button runat="server" Text="Reset" ID="ButtonCancelEmployee" OnClick="ButtonCancelEmployeeClick" /></td>
                    <td>
                        <asp:Button runat="server" Text="Delete" ID="ButtonDeleteEmployee" OnClick="ButtonDeleteEmployeeClick" data-icon="delete" data-theme="b" /></td>
                </tr>
                <tr>
                    <td colspan="4" align="left">
                        <h4>Employees</h4>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:DropDownList runat="server" ID="DropdownlistAllEmployees" AutoPostBack="True" OnSelectedIndexChanged="DropdownlistAllEmployeesOnselectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Label runat="server" ID="LabelErrorEmployee" ForeColor="red"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="6">
                        <div data-role="header" id="Div2" role="banner">
                            <h3 align="center" role="heading">Manage cache</h3>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Label runat="server" ID="LabelUpdateCacheText"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button runat="server" Text="Update cache" OnClientClick="UpdateCache()" OnClick="ButtonUpdateCacheClick" />
                    </td>
                    <td colspan="2">
                        <asp:Label runat="server" Text="" ID="LabelUpdateCache"></asp:Label></td>
                </tr>

                <tr>
                    <td colspan="4">
                        <asp:Label runat="server" ID="LabelCleanCacheText"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button runat="server" Text="Clean cache for old projects" ID="ButtonCleanCache" Enabled="false" OnClick="ButtonCleanCache_Click"/>
                    </td>
                    <td colspan="2">
                        <asp:Label runat="server" ID="LabelCleanCache" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Label runat="server" ID="LabelCompareCacheText"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button runat="server" Text="Compare economic project" ID="ButtoneCompareEconomicProjects" OnClick="ButtonCompare_Click"/>
                    </td>
                    <td colspan="2">
                        <asp:Label runat="server" ID="LabelCompareCache"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <asp:Button runat="server" Text="Home" ID="ButtonHome" data-icon="home" OnClick="ButtonHome_Click" data-theme="b" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
