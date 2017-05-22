<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="ChatTextList.aspx.cs" Inherits="Tele.Management.Pages.ChatTextList" %>

<%@ Import Namespace="Tele.Common" %>
<%@ Register Src="~/Statics/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="ChatTextList.aspx">文本话术设置列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table cellspacing="0" cellpadding="0" class="searchTable">
            <tr>
                <td class="colLable">队列名称：</td>
                <td>
                    <asp:TextBox ID="txtQueueName" runat="server"></asp:TextBox>

                </td>
                <td class="colLable">话术类型：</td>
                <td>
                    <asp:DropDownList ID="ddlChatTextType" runat="server"></asp:DropDownList>
                </td>
                <td colspan="2">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'ChatTextView.aspx';"
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
                        <th width="100">话术类型</th>
                        <th>队列名称</th>
                        <th>排序号</th>
                        <th>话术内容</th>
                        <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td align="center"><%# this.GetTypeNameByID(Eval("ChatTextTypeID").ToString())%></td>
                    <td><%#Eval("QueueName")%></td>
                    <td align="center"><%#Eval("SortID") %></td>
                    <td><%# Utils.SubstringPlus(Eval("ChatContent").ToString(), 50) %></td>
                    <td align="center">
                        <asp:LinkButton ID="btnStatus" OnClientClick="return deleteConfirm();" CommandName="Status" CommandArgument='<%#Eval("ChatTextID") %>' runat="server" Text="删除" />
                        &nbsp;&nbsp;
                            <a href='ChatTextView.aspx?id=<%#Eval("ChatTextID") %>'>编辑</a>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="aitem">
                    <td align="center"><%# this.GetTypeNameByID(Eval("ChatTextTypeID").ToString())%></td>
                    <td><%#Eval("QueueName")%></td>
                    <td align="center"><%#Eval("SortID") %></td>
                    <td><%# Utils.SubstringPlus(Eval("ChatContent").ToString(), 50) %></td>
                    <td align="center">
                        <asp:LinkButton ID="btnStatus" OnClientClick="return deleteConfirm();" CommandName="Status" CommandArgument='<%#Eval("ChatTextID") %>' runat="server" Text="删除" />
                        &nbsp;&nbsp;
                            <a href='ChatTextView.aspx?id=<%#Eval("ChatTextID") %>'>编辑</a>
                    </td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <uc1:Pager runat="server" ID="pager1" OnPageChanged="pager1_OnPageChanged" />
    </div>
</asp:Content>
