<%@ Page Title="" Language="C#" MasterPageFile="~/Statics/Content.Master" AutoEventWireup="true" CodeBehind="stationView.aspx.cs" Inherits="Tele.Management.Pages.Voice.ivr_sugg.stationView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Styles/formStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="stationList.aspx">维修站信息列表</a></li>
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
                <td class="colLable">维修站编号<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtAGENCY_ID" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="colLable">产线<span>*</span></td>
                <td>
                    <asp:DropDownList ID="txtPRODUCT_TYPE" runat="server" AppendDataBoundItems="true">
                        <asp:ListItem Value="">请选择...</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="colLable">区县</td>
                <td>
                    <asp:TextBox ID="txtAGENCY_NAME" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="colLable">区号<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtAGENCY_COVER_AREA" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="colLable">地址</td>
                <td>
                    <asp:TextBox ID="txtAGENCY_INFO" TextMode="MultiLine" Height="100" Width="500" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="colLable">工作类型</td>
                <td>
                    <asp:DropDownList ID="txtADD_TYPE" runat="server">
                        <asp:ListItem Text="不选" Value=""></asp:ListItem>
                        <asp:ListItem Text="T" Value="T"></asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td class="colLable">排序</td>
                <td>
                    <asp:TextBox ID="txtCATEGORY" runat="server"></asp:TextBox>
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
