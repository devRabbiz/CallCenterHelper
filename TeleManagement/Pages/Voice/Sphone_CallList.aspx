<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="Sphone_CallList.aspx.cs" Inherits="Tele.Management.Pages.Sphone_CallList" %>
<%@ Register Src="~/Statics/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="Sphone_CallList.aspx">语音历史记录列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table cellspacing="0" cellpadding="0" class="searchTable">
            <tr>
                <td class="colLable">Chat时间：</td>
                <td>
                    <asp:TextBox ID="txtChatBeginTime" runat="server" CssClass="qqcalendar"></asp:TextBox>
                    <asp:TextBox ID="txtChatEndTime" runat="server" CssClass="qqcalendar"></asp:TextBox>
                </td>

                <td class="colLable">时长（秒）：</td>
                <td>
                    <asp:DropDownList ID="ddlOp" runat="server">
                        <asp:ListItem Text="大于" Value=">"></asp:ListItem>
                        <asp:ListItem Text="小于" Value="<"></asp:ListItem>
                        <asp:ListItem Text="等于" Value="="></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
                </td>

                <td class="colLable">员工ID：</td>
                <td>
                    <asp:TextBox ID="txtEmployeeID" runat="server"></asp:TextBox>

                </td>
            </tr>
            <tr>

                <td class="colLable">ChatID：</td>
                <td>
                    <asp:TextBox ID="txtChatID" runat="server"></asp:TextBox>

                </td>
                <td class="colLable">队列名称：</td>
                <td>
                    <asp:TextBox ID="txtQueueName" runat="server"></asp:TextBox>

                </td>

                <td class="colLable">坐席IP：</td>
                <td>
                    <asp:TextBox ID="txtPlaceIP" runat="server"></asp:TextBox>

                </td>
            </tr>
            <tr>
                <td class="colLable">客户ID：</td>
                <td>
                    <asp:TextBox ID="txtCustomerID" runat="server"></asp:TextBox>

                </td>
                <td class="colLable">主机编号：</td>
                <td>
                    <asp:TextBox ID="txtMachineNo" runat="server"></asp:TextBox>

                </td>
                <td colspan="2">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                </td>
            </tr>
        </table>
    </div>
    <div class="formDiv">
        <h3>数据列表</h3>
        <asp:Repeater ID="rptList" runat="server">
            <HeaderTemplate>
                <table class="formTable">
                    <tr class="header">
                        <th>选择</th>
                        <th>客户ID</th>
                        <th>客户姓名</th>
                        <th>CallID</th>
                        <th>开始时间</th>
                        <th>结束时间</th>
                        <th>通话时长</th>
                        <th>呼叫方向</th>
                        <th>队列名称</th>
                        <th>坐席ID</th>
                        <th>坐席IP</th>
                        <th>IVR</th>
                        <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td>
                        <input type="checkbox" value='<%#Eval("CallID")%>' /></td>
                    <td><%#Eval("EmployeeID")%></td>
                    <td><%#Eval("CustomerID")%></td>
                    <td><%#Eval("CallID")%></td>
                    <td><%#Eval("CallBeginTime")%></td>
                    <td><%#Eval("CallEndTime")%></td>
                    <td><%#Eval("DeskTime")%></td>
                    <td><%#Eval("InOut")%></td>
                    <td><%#Eval("CurrentQueueName")%></td>
                    <td><%#Eval("EmployeeID")%></td>
                    <td><%#Eval("PlaceIP")%></td>
                    <td></td>
                    <td></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <uc1:Pager runat="server" ID="pager1" OnPageChanged="pager1_OnPageChanged" />
    </div>
</asp:Content>
