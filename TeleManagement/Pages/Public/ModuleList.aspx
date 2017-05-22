<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="ModuleList.aspx.cs" Inherits="Tele.Management.Pages.ModuleList" %>
<%@ Register Src="~/Statics/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="ModuleList.aspx">模块列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table cellspacing="0" cellpadding="0" class="searchTable">
            <tr>
                <td class="colLable">所属父模块：</td>
                <td>
                    <asp:DropDownList ID="ddlParentModule" runat="server"></asp:DropDownList>
                </td>
                <td class="colLable">模块名称：</td>
                <td>
                    <asp:TextBox ID="txtModuleName" runat="server"></asp:TextBox>

                </td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">页面链接：</td>
                <td colspan="3">
                    <asp:TextBox ID="txtModuleUrl" runat="server" Width="345"></asp:TextBox>
                </td>
                <td colspan="2">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'ModuleView.aspx';"
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
                        <th>所属父模块</th>
                        <th>模块名称</th>
                        <th>模块链接</th>
                        <th>创建人</th>
                        <th>创建时间</th>
                        <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td><%# GetModuleNameByID(Eval("ParentModuleID").ToString()) %></td>
                    <td><%#Eval("ModuleName")%></td>
                    <td><%#Eval("ModuleUrl")%></td>
                    <td><%#Eval("CreateBy")%></td>
                    <td><%#Eval("CreateTime","{0:yyyy-MM-dd}")%></td>
                    <td>
                        <asp:LinkButton ID="btnStatus" OnClientClick="return deleteConfirm();" CommandName="Status" CommandArgument='<%#Eval("ModuleID") %>' runat="server" Text="删除" />
                        &nbsp;&nbsp;
                            <a href='ModuleView.aspx?id=<%#Eval("ModuleID") %>'>编辑</a></td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="aitem">
                    <td><%# GetModuleNameByID(Eval("ParentModuleID").ToString()) %></td>
                    <td><%#Eval("ModuleName")%></td>
                    <td><%#Eval("ModuleUrl")%></td>
                    <td><%#Eval("CreateBy")%></td>
                    <td><%#Eval("CreateTime","{0:yyyy-MM-dd}")%></td>
                    <td>
                        <asp:LinkButton ID="btnStatus" OnClientClick="return deleteConfirm();" CommandName="Status" CommandArgument='<%#Eval("ModuleID") %>' runat="server" Text="删除" />
                        &nbsp;&nbsp;
                            <a href='ModuleView.aspx?id=<%#Eval("ModuleID") %>'>编辑</a></td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <uc1:Pager runat="server" ID="pager1" OnPageChanged="pager1_OnPageChanged" />
    </div>
</asp:Content>
