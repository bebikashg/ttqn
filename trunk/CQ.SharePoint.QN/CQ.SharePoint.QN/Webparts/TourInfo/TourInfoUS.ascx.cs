﻿using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using CQ.SharePoint.QN.Common;

namespace CQ.SharePoint.QN.Webparts
{
    /// <summary>
    /// QNHeaderUS
    /// </summary>
    public partial class TourInfoUS : UserControl
    {
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
                    //Bind data to latest news
                    string latestNewsQuery = string.Format("<OrderBy><FieldRef Name='Title' Ascending='TRUE' /></OrderBy>");
                    rptTourInfo.DataSource = GetNewsRecords(latestNewsQuery);
                    rptTourInfo.DataBind();
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// Get news record form NewsRecord table
        /// </summary>
        /// <param name="query">SPquery for query items</param>
        /// <returns>News record Datatable</returns>
        public DataTable GetNewsRecords(string query)
        {
            DataTable table = new DataTable();
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var site = new SPSite(SPContext.Current.Web.Site.ID))
                {
                    using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        try
                        {
                            SPQuery spQuery = new SPQuery
                            {
                                Query = query
                            };
                            SPList list = Utilities.GetListFromUrl(web, ListsName.English.ServiceInfo);
                            if (list != null)
                            {
                                SPListItemCollection items = list.GetItems(spQuery);
                                table = items.GetDataTable();
                            }
                        }
                        catch (Exception ex)
                        {
                            table = null;
                        }
                    }

                }
            });
            return table;
        }

        /// <summary>
        /// rptMenu_OnItemDataBound
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        protected void rptTourInfo_OnItemDataBound(object Sender, RepeaterItemEventArgs e)
        {

            // This event is raised for the header, the footer, separators, and items.

            // Execute the following logic for Items and Alternating Items.
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
            }
        }   
    }
}
