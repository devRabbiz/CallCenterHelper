<%@ Page Title="" Language="C#" MasterPageFile="~/Statics/Content.Master" AutoEventWireup="true" CodeBehind="DNISList.aspx.cs" Inherits="Tele.Management.Pages.Voice.TFNDNIS.DNISList" %>
<%@ Register Src="~/Statics/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Styles/listStyle.css" rel="stylesheet" />
    <script>
        $(function () {
            $('.item:odd').addClass('aitem');
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="DNISList.aspx">小号信息维护列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table class="searchTable">
            <tr>
                <td>小号</td>
                <td>
                    <asp:TextBox ID="txtDNISName" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'DNISView.aspx';"
                        value="新增" />
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
                        <th>编号</th>
                        <th>小号</th>
                        <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td><%#Eval("DNISID") %></td>
                    <td><%#Eval("DNISName") %></td>
                    <td>
                        <asp:LinkButton ID="btnDel" CommandName="Del" CommandArgument='<%#Eval("DNISID").ToString().Trim() %>' runat="server" Text="删除" OnClientClick="return deleteConfirm();" />
                        &nbsp;&nbsp;
                            <a href='DNISView.aspx?id=<%#Eval("DNISID").ToString().Trim() %>'>编辑</a>
                    </td>
                </tr>
            </ItemTemplate>

            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <uc1:Pager runat="server" ID="pager1" OnPageChanged="pager1_PageChanged" />
    </div>
</asp:Content>
