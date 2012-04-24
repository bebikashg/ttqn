﻿<%@ Assembly Name="CQ.SharePoint.QN, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QNHeaderUS.ascx.cs" Inherits="CQ.SharePoint.QN.Webparts.QNHeaderUS" %>
<script type="text/javascript">
function setHomepage() 
{ 
 if (document.all) 
    { 
        document.body.style.behavior='url(#default#homepage)'; 
  document.body.setHomePage('http://serbackup:8888/default.aspx'); 
 

    } 
    else if (window.sidebar) 
    { 
    if(window.netscape) 
    { 
         try 
   { 
            netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect"); 
         } 
         catch(e) 
         { 
    alert("this action was aviod by your browser，if you want to enable，please enter about:config in your address line,and change the value of signed.applets.codebase_principal_support to true");

         } 
    } 
    var prefs = Components.classes['@mozilla.org/preferences-service;1'].getService(Components. interfaces.nsIPrefBranch);

    prefs.setCharPref('browser.startup.homepage','http://serbackup:8888/default.aspx');

 } 
} 
</script>
<div id="header">
	<div class="banner">
		<img src="images/Index_02.gif" alt="" />
	</div>
	<div class="top_menu">
		<div class="menu">
		    <ul id="nav">
				<li><a href="#" style="background:url(images/bg_menu_hover.jpg) top left repeat-x;">Trang chủ</a></li>
                <asp:Repeater ID="rptMenu" runat="server" OnItemDataBound="rptMenu_OnItemDataBound">
                    <ItemTemplate>
                        <li><a href='<%#Eval("Url") %>'><%#Eval("Title") %></a>
                            <asp:Repeater ID="rptSubMenu" runat="server">
                                <HeaderTemplate><ul></HeaderTemplate>
                                <ItemTemplate>
                                    <li><a href='<%#Eval("Url") %>'><%#Eval("Title") %></a></li>
                                </ItemTemplate>
                                <FooterTemplate></ul></FooterTemplate>
                            </asp:Repeater>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
		</div>
		<div class="search">
			<input type="text" id="txtData" name="q" onkeypress="return BBEnterPress();" style="border: 0px;"> <a href="#">Tìm kiếm</a>
		</div>
		<div class="language">
			<span><img src="images/english.jpg" /></span><span><a href="#">English</a></span>
		</div>
		<div class="cleaner"></div>
				
	</div>
	<div class="cleaner"></div>
	<div class="bg_bottom_top_menu">
		<div class="inner_content_bottom_topMenu">
			<div class="time_date">Hôm nay, ngày <%=DateTime.Now %></div>
			<div class="set_hompage"><a href="#" onclick="setHomepage();">Đặt làm trang chủ</a></div>
			<div class="RSS"><a href="#">RSS</a></div>
			<div class="cleaner"></div>
		</div>
	</div>
</div>