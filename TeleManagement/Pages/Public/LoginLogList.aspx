
<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="LoginLogList.aspx.cs" Inherits="Tele.Management.Pages.LoginLogList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="LoginLogList.aspx">LoginLog列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table cellspacing="0" cellpadding="0" class="searchTable">
            <tr>
                    					<td class="colLable">ITCode：</td>
                    <td>			        		<asp:TextBox ID="txtEmployeeID" runat="server"></asp:TextBox>
                             
                    </td>
                    								<td class="colLable">登录日期：</td>
                    <td>			        		<asp:TextBox ID="txtLoginDate" runat="server"></asp:TextBox>
                             
                    </td>
                                    </tr>
            								<td class="colLable">首次登录时间：</td>
                    <td>			        		<asp:TextBox ID="txtFirstloginTime" runat="server"></asp:TextBox>
                             
                    </td>
                                    </tr>
            			<tr>
                    					<td class="colLable">首次退出时间：</td>
                    <td>			        		<asp:TextBox ID="txtFirstlogoutTime" runat="server"></asp:TextBox>
                             
                    </td>
                    								<td class="colLable">上次退出时间：</td>
                    <td>			        		<asp:TextBox ID="txtLastlogout" runat="server"></asp:TextBox>
                             
                    </td>
                                    </tr>
            								<td class="colLable">创建人：</td>
                    <td>			        		<asp:TextBox ID="txtCreateBy" runat="server"></asp:TextBox>
                             
                    </td>
                                    </tr>
            			<tr>
                    					<td class="colLable">创建时间：</td>
                    <td>			        		<asp:TextBox ID="txtCreateTime" runat="server"></asp:TextBox>
                             
                    </td>
                    								<td class="colLable">修改人：</td>
                    <td>			        		<asp:TextBox ID="txtUpdateBy" runat="server"></asp:TextBox>
                             
                    </td>
                                    </tr>
            								<td class="colLable">修改时间：</td>
                    <td>			        		<asp:TextBox ID="txtUpdateTime" runat="server"></asp:TextBox>
                             
                    </td>
                                    </tr>
            						<tr>
                <td colspan="4"></td>
                <td colspan="2">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'LoginLogView.aspx';"
                        value="新增" />
                </td>
            </tr>
        </table>
    </div>
    <div class="formDiv">
        <h3>数据列表</h3>
        <asp:Repeater ID="rptList" runat="server">
            <HeaderTemplate>
                <table class="formTable">
                    <tr class="header">
                        			<th>ITCode</th>
                        			<th>登录日期</th>
                        			<th>首次登录时间</th>
                        			<th>首次退出时间</th>
                        			<th>上次退出时间</th>
                        			<th>创建人</th>
                        			<th>创建时间</th>
                        			<th>修改人</th>
                        			<th>修改时间</th>
                                                <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    			<td><%#Eval("EmployeeID")%></td>
                    			<td><%#Eval("LoginDate")%></td>
                    			<td><%#Eval("FirstloginTime")%></td>
                    			<td><%#Eval("FirstlogoutTime")%></td>
                    			<td><%#Eval("Lastlogout")%></td>
                    			<td><%#Eval("CreateBy")%></td>
                    			<td><%#Eval("CreateTime")%></td>
                    			<td><%#Eval("UpdateBy")%></td>
                    			<td><%#Eval("UpdateTime")%></td>
                                    </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
