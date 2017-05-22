<%@ Page Title="" Language="C#" MasterPageFile="~/Statics/Content.Master" AutoEventWireup="true" CodeBehind="TFNList.aspx.cs" Inherits="Tele.Management.Pages.Voice.TFNDNIS.TFNList" %>

<%@ Register Src="~/Statics/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Styles/listStyle.css" rel="stylesheet" />
    <style>
        .formTable {
            min-width: 100%;
        }

        .searchTable {
            width: 100%;
        }

        .current {
            background-color: #f9ff6d;
        }
    </style>
    <script>
        function callSucc() {
            var id = $('#ContentPlaceHolder1_txtTFNID').val();
            try {
                $('.TFNName' + id + 'S a:first')[0].click();
            }
            catch (e) { }
            $('.TFNName' + id + 'S a:first').click();
        }

        function doCreate(t) {
            var id = $('#ContentPlaceHolder1_txtTFNID').val();
            if (id == '') {
                alert('请选择大号');
                return false;
            }
            var url = '';
            if (t == 'q') {
                url = '/pages/voice/tfndnis/TFNQView.aspx?TFN=' + id;
            }
            else if (t == 'dnis') {
                url = '/pages/voice/tfndnis/TFNDNISView.aspx?TFN=' + id;
            }
            if (url != '') {
                window.open(url, 'q', 'width=860,height=620,top=10,right=10');
            }
        }

        $(function () {
            var i = $('#ContentPlaceHolder1_txtTFNID').val();
            $('.TFNName' + i).parent().parent().addClass('current');
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="TFNList.aspx">大号信息列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>

    <asp:HiddenField ID="txtTFNID" runat="server" />
    <table class="formTable">
        <tr>
            <td valign="top" width="50%">
                <div class="formDiv">
                    <h3>查询条件</h3>
                    <table class="searchTable">
                        <tr>
                            <td>大号</td>
                            <td>
                                <asp:TextBox ID="txtTFNName" runat="server"></asp:TextBox></td>
                            <td>业务
                            </td>
                            <td>
                                <asp:DropDownList ID="txtTFNBusi" runat="server" AppendDataBoundItems="true">
                                    <asp:ListItem Value="" Text="==不限=="></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" align="right">
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                                &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'TFNView.aspx';"
                        value="新增" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="formDiv">
                    <h3>大号列表</h3>
                    <asp:GridView ID="gridTFN" CssClass="formTable" AutoGenerateColumns="false" runat="server" OnRowCommand="gridTFN_RowCommand" ShowHeaderWhenEmpty="True" CellSpacing="-1" GridLines="None">
                        <Columns>
                            <asp:TemplateField HeaderText="大号">
                                <ItemTemplate>
                                    <label class="TFNName<%#Eval("TFNID") %>">
                                        <%#Eval("TFNName") %>
                                    </label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="业务">
                                <ItemTemplate>
                                    <%#getTFNBusiName(Eval("TFNBusi")) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="操作">
                                <ItemTemplate>
                                    <label class='TFNName<%#Eval("TFNID") %>S'>
                                        <asp:LinkButton ID="lb" CommandArgument='<%#Eval("TFNID") %>' runat="server" CommandName="Select" Text="选择"></asp:LinkButton></label>
                                    &nbsp;&nbsp;
                                    <a href="TFNView.aspx?id=<%#Eval("TFNID") %>">编辑</a>
                                    &nbsp;&nbsp;
                                    <asp:LinkButton ID="LinkButton1" OnClientClick="return confirm('删除后无法恢复！\r\n确认删除？')" CommandArgument='<%#Eval("TFNID") %>' runat="server" CommandName="Del" Text="删除"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            没有找到！
                        </EmptyDataTemplate>
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="item" />
                        <AlternatingRowStyle CssClass="aitem" />
                    </asp:GridView>
                    <uc1:Pager runat="server" ID="pager1" OnPageChanged="pager1_PageChanged" />
                </div>
            </td>
            <td valign="top" width="50%">


                <div class="formDiv">
                    <h3>大号与队列关系</h3>
                    <div align="right">
                        <input type="button" class="btn100" onclick="return doCreate('q')"
                            value="新增" />
                    </div>
                    <asp:GridView ID="gridQ" ShowHeaderWhenEmpty="true" runat="server" AutoGenerateColumns="false" OnRowCommand="gridQ_RowCommand" CellSpacing="-1" CssClass="formTable" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="QueueName" HeaderText="队列" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lb" OnClientClick="return confirm('删除后无法恢复！\r\n确认删除？')" CommandArgument='<%#Eval("TFNID")+","+Eval("QueueID") %>' runat="server" CommandName="Del" Text="删除"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            没有找到！
                        </EmptyDataTemplate>
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="item" />
                        <AlternatingRowStyle CssClass="aitem" />
                    </asp:GridView>
                </div>

                <div class="formDiv">
                    <h3>大号与小号关系</h3>
                    <div align="right">
                        <input type="button" class="btn100" onclick="return doCreate('dnis')"
                            value="新增" />
                    </div>
                    <asp:GridView ID="gridDNIS" runat="server" ShowHeaderWhenEmpty="true" AutoGenerateColumns="false" OnRowCommand="gridDNIS_RowCommand" CssClass="formTable" CellSpacing="-1" GridLines="None">

                        <Columns>
                            <asp:BoundField DataField="DNISName" HeaderText="小号" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lb" OnClientClick="return confirm('删除后无法恢复！\r\n确认删除？')" CommandArgument='<%#Eval("TFNID")+","+Eval("DNISID") %>' runat="server" CommandName="Del" Text="删除"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            没有找到！
                        </EmptyDataTemplate>
                        <HeaderStyle CssClass="header" />
                        <RowStyle CssClass="item" />
                        <AlternatingRowStyle CssClass="aitem" />
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>






</asp:Content>
