<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="HolidayRoleView.aspx.cs" Inherits="Tele.Management.Pages.HolidayRoleView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/formStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="HolidayList.aspx">工作时间配置列表</a></li>
            <li>-</li>
            <li>
                <asp:Literal ID="ltlCurrentPageType" runat="server"></asp:Literal>假期规则</li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>表单元素</h3>
        <table class="searchTable">
            <tr>
                <td class="colLable">假期规则名称<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtHolidayRoleName" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">开始日期<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtHolidayBegin" runat="server" CssClass="qqcalendar"></asp:TextBox></td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">结束日期<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtHolidayEnd" runat="server" CssClass="qqcalendar"></asp:TextBox></td>
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
