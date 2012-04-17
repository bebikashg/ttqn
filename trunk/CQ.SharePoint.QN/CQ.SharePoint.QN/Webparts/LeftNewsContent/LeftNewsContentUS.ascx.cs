﻿using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using CQ.SharePoint.QN.Common;
using Microsoft.SharePoint.WebPartPages;

namespace CQ.SharePoint.QN.Webparts
{
    /// <summary>
    /// QNHeaderUS
    /// </summary>
    public partial class LeftNewsContentUS : UserControl
    {
        public LeftNewsContent WebpartParent;
        private string _newsCategoryTitle;
        public string NewsUrl = string.Empty;

        /// <summary>
        /// Page on Load
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    NewsUrl = string.Format("{0}/{1}.aspx?NewsId=", SPContext.Current.Web.Url,Constants.PageInWeb.DetailNews);
                    string newsGroupQuery = string.Format("<Where><Eq><FieldRef Name='{0}' /><Value Type='Lookup'>{1}</Value></Eq></Where><OrderBy><FieldRef Name='Created' Ascending='False' /></OrderBy>", FieldsName.NewsRecord.English.CategoryName, SPHttpUtility.HtmlEncode(WebpartParent.GroupName));
                    var newsGroups = Utilities.GetNewsRecords(newsGroupQuery, Convert.ToUInt16(WebpartParent.NumberOfNews), ListsName.English.NewsRecord);
                    if (newsGroups != null && newsGroups.Rows.Count > 0)
                    {
                        lblHeader.Text = Convert.ToString(newsGroups.Rows[0][FieldsName.Title]);
                        lblShortContent.Text = Convert.ToString(newsGroups.Rows[0][FieldsName.NewsRecord.English.ShortContent]);
                        rptCaiCachThuTucHanhChinh.DataSource = newsGroups;
                        rptCaiCachThuTucHanhChinh.DataBind();
                    }
                    
                    string newsTitle =string.Format("<Where><Eq><FieldRef Name='{0}' /><Value Type='Number'>{1}</Value></Eq></Where><OrderBy><FieldRef Name='{2}' Ascending='True' /></OrderBy>", FieldsName.NewsCategory.English.ParentId, WebpartParent.NewsGroupID, FieldsName.Title);
                    var newsTitleItems = Utilities.GetNewsRecords(newsTitle, 4, ListsName.English.NewsCategory);
                    if (newsTitleItems != null && newsTitleItems.Rows.Count > 0)
                    {
                        rptNewsGroup.DataSource = newsTitleItems;
                        rptNewsGroup.DataBind();
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        /// <summary>
        /// OnpreRender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            WebPartManager webPartManager = WebPartManager.GetCurrentWebPartManager(Page);
        }
    }
}
