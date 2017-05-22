<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="HolidayList.aspx.cs" Inherits="Tele.Management.Pages.HolidayList" %>

<%@ Register Src="~/Statics/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="HolidayList.aspx">工作时间配置列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table cellspacing="0" cellpadding="0" class="searchTable">
            <tr>
                <td class="colLable">类型</td>
                <td>
                    <asp:DropDownList ID="ddlType" runat="server">
                        <asp:ListItem Text="——不限——" Value=""></asp:ListItem>
                        <asp:ListItem Text="语音" Value="1"></asp:ListItem>
                        <asp:ListItem Text="文本" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="colLable">假期规则：</td>
                <td>
                    <asp:DropDownList ID="ddlHolidayRole" runat="server">
                    </asp:DropDownList>
                </td>
                <td class="colLable">IVR队列：</td>
                <td>
                    <asp:TextBox ID="txtSkill" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="colLable">号码：</td>
                <td>
                    <asp:TextBox ID="txtDNIS" runat="server"></asp:TextBox>

                </td>
                <td></td>
                <td colspan="3">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'HolidayView.aspx';"
                        value="新增" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'HolidayRoleView.aspx';"
                        value="新增假期规则" />
                </td>
            </tr>
        </table>
    </div>
    <div class="formDiv">
        <h3>数据列表</h3>
        <asp:Repeater ID="rptList" runat="server" OnItemCommand="rptList_ItemCommand">
            <HeaderTemplate>
                <table class="formTable">
                    <tr class="header">
                        <th>类型</th>
                        <th>假期规则</th>
                        <th>号码</th>
                        <th>IVR队列</th>
                        <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td><%#Eval("TypeName")%></td>
                    <td><%# GetRoleNameByID(Eval("HolidayRoleID").ToString()) %></td>
                    <td><%#Eval("DNIS")%></td>
                    <td><%#Eval("QueueName")%></td>
                    <td align="center">
                        <asp:LinkButton ID="btnStatus" OnClientClick="return deleteConfirm();" CommandName="Status" CommandArgument='<%#Eval("HolidayID") %>' runat="server" Text="删除" />
                        &nbsp;&nbsp;
                            <a href='HolidayView.aspx?id=<%#Eval("HolidayID") %>'>编辑</a>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="aitem">
                    <td><%#Eval("TypeName")%></td>
                    <td><%# GetRoleNameByID(Eval("HolidayRoleID").ToString()) %></td>
                    <td><%#Eval("DNIS")%></td>
                    <td><%#Eval("QueueName")%></td>
                    <td align="center">
                        <asp:LinkButton ID="btnStatus" OnClientClick="return deleteConfirm();" CommandName="Status" CommandArgument='<%#Eval("HolidayID") %>' runat="server" Text="删除" />
                        &nbsp;&nbsp;
                            <a href='HolidayView.aspx?id=<%#Eval("HolidayID") %>'>编辑</a>
                    </td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <uc1:Pager runat="server" ID="pager1" OnPageChanged="pager1_OnPageChanged" />
    </div>
</asp:Content>
