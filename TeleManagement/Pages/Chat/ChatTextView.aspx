<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="ChatTextView.aspx.cs" Inherits="Tele.Management.Pages.ChatTextView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/formStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="ChatTextList.aspx">文本话术设置列表</a></li>
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
                <td class="colLable">队列名称<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtQueueName" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">话术类型<span>*</span></td>
                <td>
                    <asp:DropDownList ID="ddlChatTextType" runat="server"></asp:DropDownList></td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">排序号<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtSortID" runat="server" Text="100"></asp:TextBox></td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">话术内容<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtChatContent" runat="server" TextMode="MultiLine" Width="500" Height="60"></asp:TextBox></td>
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
