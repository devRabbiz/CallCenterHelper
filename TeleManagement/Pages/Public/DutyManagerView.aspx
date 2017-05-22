<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="DutyManagerView.aspx.cs" Inherits="Tele.Management.Pages.DutyManagerView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/formStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="DutyManagerList.aspx">值班经理维护列表</a></li>
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
                <td class="colLable">姓名<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtManagerName" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">电话<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtPhoneNo" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">值班日期<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtBeginDate" runat="server" CssClass="qqcalendar"></asp:TextBox></td>
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
