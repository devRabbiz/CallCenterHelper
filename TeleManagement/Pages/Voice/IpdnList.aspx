<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="IpdnList.aspx.cs" Inherits="Tele.Management.Pages.Public.IpdnList" %>

<%@ Register Src="~/Statics/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<asp:Content ID="head1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="IpdnList.aspx">IPDN列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table class="searchTable">
            <tr>
                <td>Place</td>
                <td>
                    <asp:TextBox ID="txtPlace" runat="server"></asp:TextBox></td>
                <td>DN</td>
                <td>
                    <asp:TextBox ID="txtDNNo" runat="server"></asp:TextBox></td>
                <td>IP</td>
                <td>
                    <asp:TextBox ID="txtPlaceIP" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>有效性</td>
                <td>
                    <asp:DropDownList ID="ddlStatus" runat="server">
                        <asp:ListItem Text="有效" Value="1"></asp:ListItem>
                        <asp:ListItem Text="无效" Value="0"></asp:ListItem>
                    </asp:DropDownList></td>
                <td>绑定方式</td>
                <td>
                    <asp:DropDownList ID="ddlBindType" runat="server">
                        <asp:ListItem Text="IP绑定" Value="IP绑定"></asp:ListItem>
                    </asp:DropDownList></td>
                <td>话机类型</td>
                <td>
                    <asp:DropDownList ID="ddlPhoneType" runat="server">
                        <asp:ListItem Text="普通话机" Value="普通话机"></asp:ListItem>
                        <asp:ListItem Text="IP电话" Value="IP电话"></asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td>话机IP</td>
                <td>
                    <asp:TextBox ID="txtPhoneIP" runat="server"></asp:TextBox></td>
                <td></td>
                <td colspan="3">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'IpdnView.aspx';"
                        value="新增" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = '../Public/FileUpload.aspx?docid=Xlt001';"
                        value="导入" />
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
                        <th>Place</th>
                        <th>DN</th>
                        <th>IP</th>
                        <th>话机IP</th>
                        <th>有效性</th>
                        <th>绑定方式</th>
                        <th>话机类型</th>
                        <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td><%#Eval("Place") %></td>
                    <td><%#Eval("DNNo") %></td>
                    <td><%#Eval("PlaceIP") %></td>
                    <td><%#Eval("PhoneIP") %></td>
                    <td align="center"><%#Eval("IsValid").ToString() == "1" ? "有效" : "无效" %></td>
                    <td align="center"><%#Eval("BindType") %></td>
                    <td align="center"><%#Eval("PhoneType") %></td>
                    <td align="center">
                        <asp:LinkButton ID="btnStatus" CommandName="Status" CommandArgument='<%#Eval("ID") %>' runat="server" Text="禁用" />
                        &nbsp;&nbsp;
                            <a href='IpdnView.aspx?id=<%#Eval("ID") %>'>编辑</a>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="aitem">
                    <td><%#Eval("Place") %></td>
                    <td><%#Eval("DNNo") %></td>
                    <td><%#Eval("PlaceIP") %></td>
                    <td><%#Eval("PhoneIP") %></td>
                    <td align="center"><%#Eval("IsValid").ToString() == "1" ? "有效" : "无效" %></td>
                    <td align="center"><%#Eval("BindType") %></td>
                    <td align="center"><%#Eval("PhoneType") %></td>
                    <td align="center">
                        <asp:LinkButton ID="btnStatus" CommandName="Status" CommandArgument='<%#Eval("ID") %>' runat="server" Text="禁用" />
                        &nbsp;&nbsp;
                            <a href='IpdnView.aspx?id=<%#Eval("ID") %>'>编辑</a>
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
