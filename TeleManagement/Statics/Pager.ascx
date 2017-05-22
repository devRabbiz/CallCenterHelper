<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Pager.ascx.cs" Inherits="Tele.Management.Statics.Pager" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<style type="text/css">
    .pagerbox {
        width: 500px;
        margin-left: 50px;
        line-height: 28px;
        font-size: 12px;
        line-height: 28px;
        background-color: White;
        color: #000;
    }
</style>
<div class="pagerbox ">
    <webdiyer:AspNetPager ID="AspNetPager1" runat="server" ShowPageIndex="False" CssClass="pager"
        FirstPageText="首页" PrevPageText="前一页" NextPageText="下一页" LastPageText="尾页"
        ShowPageIndexBox="Always" PageIndexBoxType="TextBox" TextBeforePageIndexBox="第" TextAfterPageIndexBox="页" SubmitButtonText="Go"
        ShowCustomInfoSection="Right" AlwaysShow="True" CustomInfoTextAlign="Center"
        ShowMoreButtons="true" CustomInfoHTML="第 %CurrentPageIndex% 页 / 共 %PageCount% 页 %RecordCount% 条数据">
    </webdiyer:AspNetPager>
</div>
