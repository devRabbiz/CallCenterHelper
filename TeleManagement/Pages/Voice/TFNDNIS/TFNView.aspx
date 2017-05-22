<%@ Page Title="" Language="C#" MasterPageFile="~/Statics/Content.Master" AutoEventWireup="true" CodeBehind="TFNView.aspx.cs" Inherits="Tele.Management.Pages.Voice.TFNDNIS.TFNView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Styles/formStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="TFNList.aspx">大号信息列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>

    <div class="formDiv">
        <h3>表单元素</h3>
        <table class="searchTable">
            <tr>
                <td class="colLable">大号<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtTFNName" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="colLable">业务<span>*</span></td>
                <td>
                    <asp:DropDownList ID="txtTFNBusi" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="colLable">应用文件名<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtTFNCallFlow" MaxLength="200" runat="server"></asp:TextBox>
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
