<%@ Page Title="" Language="C#" MasterPageFile="~/Statics/Content.Master" AutoEventWireup="true" CodeBehind="TFNQView.aspx.cs" Inherits="Tele.Management.Pages.Voice.TFNDNIS.TFNQView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Styles/formStyle.css" rel="stylesheet" />

    <script src="/Scripts/globalScript.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.tree.js" type="text/javascript"></script>
    <link href="/Styles/formStyle.css" rel="stylesheet" />
    <link href="/Styles/jquery.tree.css" rel="stylesheet" />

    <script src="/Scripts/jquery-ui-1.10.1.js"></script>
    <script src="/Scripts/jquery.multiselect.js"></script>
    <script src="/Scripts/jquery.multiselect.filter.js"></script>

    <link href="/Styles/themes/base/jquery.ui.all.css" rel="stylesheet" />
    <link href="/Styles/themes/jquery.multiselect.css" rel="stylesheet" />
    <link href="/Styles/themes/jquery.multiselect.filter.css" rel="stylesheet" />

    <script>
        $(function () {

            $('select[id*="txtQ"]')
                .multiselect({
                    selectedList: 5,
                    minWidth: 600,
                    height: 300,
                    checkAllText: '全选',
                    uncheckAllText: '取消选中',
                    noneSelectedText: '选择队列',
                    selectedText: '# 个队列选中'
                })
                .multiselectfilter({
                    label: '筛选',
                    placeholder: '请输入关键字',
                    width: 200
                });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="DNISList.aspx">大号与队列关系</a></li>
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
                <td class="colLable">队列<span>*</span></td>
                <td>
                    <asp:ListBox ID="txtQ" SelectionMode="Multiple" runat="server"></asp:ListBox>
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


    <asp:Panel ID="panOK" Visible="false" runat="server">
        <script>
            if (window.opener.callSucc) {
                window.opener.callSucc();
                window.close();
            }
        </script>
    </asp:Panel>

</asp:Content>
