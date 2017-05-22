<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="BlackListTypeView.aspx.cs" Inherits="Tele.Management.Pages.BlackListTypeView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/formStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="BlackListTypeList.aspx">黑名单类型列表</a></li>
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
                <td class="colLable">黑名单类型<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtTypeName" runat="server"></asp:TextBox></td>
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
