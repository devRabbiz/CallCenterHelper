
<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="RoleModuleView.aspx.cs" Inherits="Tele.Management.Pages.RoleModuleView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/formStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="RoleModuleList.aspx">RoleModule列表</a></li>
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
                <td class="colLable">创建人<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtCreateBy" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">创建时间<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtCreateTime" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">修改人<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtUpdateBy" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">修改时间<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtUpdateTime" runat="server"></asp:TextBox></td>
                <td></td>
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
