<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="BlackListList.aspx.cs" Inherits="Tele.Management.Pages.BlackListList" %>
<%@ Register Src="~/Statics/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="BlackListList.aspx">黑名单列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table cellspacing="0" cellpadding="0" class="searchTable">
            <tr>
                <td class="colLable">黑名单类型：</td>
                <td>
                    <asp:DropDownList ID="ddlBlackListType" runat="server">
                    </asp:DropDownList></td>
                <td class="colLable">关键字：</td>
                <td>
                    <asp:TextBox ID="txtBillNo" runat="server"></asp:TextBox>

                </td>
                <td colspan="2">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'BlackListView.aspx';"
                        value="新增" />
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
                        <th width="150">黑名单类型</th>
                        <th>值</th>
                        <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td align="center"><%# GetTypeNameByID(Eval("BlackListTypeID").ToString()) %></td>
                    <td><%#Eval("BillNo")%></td>
                    <td align="center">
                        <asp:LinkButton ID="btnStatus" OnClientClick="return deleteConfirm();" CommandName="Status" CommandArgument='<%#Eval("BlackListID") %>' runat="server" Text="删除" /></td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="aitem">
                    <td align="center"><%# GetTypeNameByID(Eval("BlackListTypeID").ToString()) %></td>
                    <td><%#Eval("BillNo")%></td>
                    <td align="center">
                        <asp:LinkButton ID="btnStatus" OnClientClick="return deleteConfirm();" CommandName="Status" CommandArgument='<%#Eval("BlackListID") %>' runat="server" Text="删除" /></td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <uc1:Pager runat="server" ID="pager1" OnPageChanged="pager1_OnPageChanged" />
    </div>
</asp:Content>
