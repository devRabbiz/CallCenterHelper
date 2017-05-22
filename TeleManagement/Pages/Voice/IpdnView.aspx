<%@ Page Title="" Language="C#" MasterPageFile="~/Statics/Content.Master" AutoEventWireup="true" CodeBehind="IpdnView.aspx.cs" Inherits="Tele.Management.Pages.Public.IpdnView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/formStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="IpdnList.aspx">IPDN列表</a></li>
            <li>-</li>
            <li>
                <asp:Literal ID="ltlCurrentPageType" runat="server"></asp:Literal></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>表单元素</h3>
        <table class="searchTable">
            <tr>
                <td class="colLable">Place<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtPlace" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="colLable">DN<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtDNNo" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="colLable">IP<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtPlaceIP" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="colLable">话机IP</td>
                <td>
                    <asp:TextBox ID="txtPhoneIP" runat="server" Text="0.0.0.0"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="colLable">绑定方式</td>
                <td>
                    <asp:DropDownList ID="ddlBindType" runat="server">
                        <asp:ListItem Text="IP绑定" Value="IP绑定"></asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td class="colLable">话机类型</td>
                <td>
                    <asp:DropDownList ID="ddlPhoneType" runat="server">
                        <asp:ListItem Text="普通话机" Value="普通话机"></asp:ListItem>
                        <asp:ListItem Text="IP电话" Value="IP电话"></asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td class="colLable">有效性</td>
                <td>
                    <asp:DropDownList ID="ddlIsValid" runat="server">
                        <asp:ListItem Text="有效" Value="1"></asp:ListItem>
                        <asp:ListItem Text="无效" Value="0"></asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td class="colLable">
                    是否报工号
                </td>
                <td>
                    <asp:DropDownList ID="ddlIsSayEmpNO" runat="server">
                        <asp:ListItem Text="是" Value="1"></asp:ListItem>
                        <asp:ListItem Text="否" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="colLable">
                    是否真实分机
                </td>
                <td>
                    <asp:DropDownList ID="ddlIsRealDN" runat="server">
                        <asp:ListItem Text="是" Value="1"></asp:ListItem>
                        <asp:ListItem Text="否" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="btnSave" runat="server" CssClass="btn100" Text="保存" OnClick="btnSave_Click" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
