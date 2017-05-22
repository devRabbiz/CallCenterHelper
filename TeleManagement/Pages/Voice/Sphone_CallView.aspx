
<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="Sphone_CallView.aspx.cs" Inherits="Tele.Management.Pages.Sphone_CallView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/formStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="Sphone_CallList.aspx">Sphone_Call列表</a></li>
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
                <td class="colLable">ITCode<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtEmployeeID" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">客户ID<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtCustomerID" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">开始时间<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtCallBeginTime" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">结束时间<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtCallEndTime" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">DeskTime<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtDeskTime" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">InteractionID<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtConnectionID" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">呼叫方向<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtInOut" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">当前队列<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtCurrentQueueName" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">来自队列<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtFromQueueName" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">转接队列<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtNextQueueName" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">ANI<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtANI" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">DNIS<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtDNIS" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">IP<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtPlaceIP" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">是否会议<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtIsConference" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">是否转接<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtIsTransfer" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
                        <tr>
                <td class="colLable">IsTransferEPOS<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtIsTransferEPOS" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
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
