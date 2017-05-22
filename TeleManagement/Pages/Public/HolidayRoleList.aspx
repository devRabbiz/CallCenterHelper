<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="HolidayRoleList.aspx.cs" Inherits="Tele.Management.Pages.HolidayRoleList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="HolidayRoleList.aspx">HolidayRole列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table cellspacing="0" cellpadding="0" class="searchTable">
            <tr>
                <td class="colLable">假期规则名称：</td>
                <td>
                    <asp:TextBox ID="txtHolidayRoleName" runat="server"></asp:TextBox>

                </td>
                <td class="colLable">开始日期：</td>
                <td>
                    <asp:TextBox ID="txtHolidayBegin" runat="server"></asp:TextBox>

                </td>

                <td class="colLable">结束日期：</td>
                <td>
                    <asp:TextBox ID="txtHolidayEnd" runat="server"></asp:TextBox>

                </td>
            </tr>

            <tr>
                <td colspan="4"></td>
                <td colspan="2">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'HolidayRoleView.aspx';"
                        value="新增" />
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
                        <th>假期规则名称</th>
                        <th>开始日期</th>
                        <th>结束日期</th>
                        <th>创建人</th>
                        <th>创建时间</th>
                        <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td><%#Eval("HolidayRoleName")%></td>
                    <td><%#Eval("HolidayBegin")%></td>
                    <td><%#Eval("HolidayEnd")%></td>
                    <td><%#Eval("CreateBy")%></td>
                    <td><%#Eval("CreateTime")%></td>
                    <td>
                        修改 删除
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
