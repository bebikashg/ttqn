<%@ Assembly Name="CQ.SharePoint.QN, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeftNewsContentUS.ascx.cs"
    Inherits="CQ.SharePoint.QN.Webparts.LeftNewsContentUS" %>
<%@ Import Namespace="CQ.SharePoint.QN.Common" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="System.ComponentModel" %>
<div class="mod_News_external">
    <div class="bg_title_ModNews">
        <div class="title_cate_News">
            <div class="name_title_typ_News">
                <a href="/SubPage.aspx?IsChuyenDe=1&ListCategoryName=NewsCategory&ListName=NewsRecord&Page=1&CategoryId=<%=WebpartParent.NewsGroupID%>"
                    target="_blank">
                    <%=WebpartParent.GroupName%></a>
            </div>
            <div class="link_cate_more">
                <asp:Repeater ID="rptNewsGroup" runat="server">
                    <HeaderTemplate>
                        <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li><a href='<%= CategoryUrl%><%#Eval("ID") %>'>
                            <%#Eval("Title")%></a></li>|
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div class="cleaner">
            </div>
        </div>
        <div class="content_typ_News">
            <div class="hotnews_test">
                <%--<asp:Image ID="imgThumb" runat="server" />--%>
                <asp:Literal ID="ltrImage" runat="server"></asp:Literal>
                <h3>
                    <a href='<%= NewsFirstUrl1%>'>
                        <asp:Label ID="lblHeader" runat="server"></asp:Label></a></h3>
                <span>
                    <asp:Label ID="lblShortContent" runat="server"></asp:Label></span>
                <div class="cleaner">
                </div>
            </div>
            <div class="list_other_news">
                <asp:Repeater ID="rptCaiCachThuTucHanhChinh" runat="server">
                    <HeaderTemplate>
                        <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li><a href='<%= NewsUrl%><%#Eval("ID") %>&CategoryId=<%#Eval("CategoryId") %>'>
                            <%#Eval("Title")%></a></li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>
