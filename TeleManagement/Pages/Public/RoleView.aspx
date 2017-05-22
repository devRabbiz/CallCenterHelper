<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="RoleView.aspx.cs" Inherits="Tele.Management.Pages.RoleView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Scripts/globalScript.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.tree.js" type="text/javascript"></script>
    <link href="../../Styles/formStyle.css" rel="stylesheet" />
    <link href="../../Styles/jquery.tree.css" rel="stylesheet" />
    <script type="text/javascript">
        var sel;
        $(document).ready(function () {
            var aryContent = $("#ContentPlaceHolder1_hdTree").val().replace(/!/g, '"');
            if (aryContent.length > 0) aryContent = eval('(' + aryContent + ')');
            var o = {
                showcheck: true,
                cbiconpath: "../../Styles/tree/",
                emptyiconpath: "../../Styles/tree/s.gif",
                theme: "bbit-tree-lines",
                data: aryContent,
                onnodeclick: function (item) {
                    //假如有子节点，展开
                    if (item.h && !item.isexpand)
                        $(this).children("img").click();
                    else
                        $(this).children("img[id^=tree_]").click();
                },
                oncheckboxclick: function (item) {
                    //假如有子节点，展开
                    if (item.h && !item.isexpand)
                        $(this).parent().children("img").click();
                }
            };
            $("#tree").treeview(o);
        });

        function setReturnValue() {
            var selValue = $("#tree").getTSVsAll();
            $("#ContentPlaceHolder1_hdSelectedOrg").val(selValue);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <input id="hdTree" type="hidden" value="" runat="server" />
        <input id="hdSelectedOrg" type="hidden" runat="server" />
    </div>
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="RoleList.aspx">角色列表</a></li>
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
                <td class="colLable">角色名称<span>*</span></td>
                <td>
                    <asp:TextBox ID="txtRoleName" runat="server"></asp:TextBox></td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">角色权限<span>*</span></td>
                <td>
                    <div id="tree">
                    </div>
                </td>
                <td></td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="btnSave" runat="server" CssClass="btn100" Text="保存" OnClientClick="setReturnValue();" OnClick="btnSave_Click" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
