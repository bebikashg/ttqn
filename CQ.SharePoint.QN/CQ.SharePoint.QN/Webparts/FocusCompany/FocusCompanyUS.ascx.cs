﻿using System;
using System.Data;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using CQ.SharePoint.QN.Common;

namespace CQ.SharePoint.QN.Webparts
{
    /// <summary>
    /// QNHeaderUS
    /// </summary>
    public partial class FocusCompanyUS : UserControl
    {
        public FocusCompany WebpartParent;
        public string NewsUrl = string.Empty;
        /// <summary>
        /// Page on Load
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">EventArgs e</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string imagepath;
                string focusCompanyQuery = string.Format(@"<Where>
                                                              <And>
                                                                 <Eq>
                                                                    <FieldRef Name='{0}' />
                                                                    <Value Type='Boolean'>1</Value>
                                                                 </Eq>
                                                                 <And>
                                                                    <Neq>
                                                                       <FieldRef Name='{1}' />
                                                                       <Value Type='Boolean'>1</Value>
                                                                    </Neq>
                                                                    <Lt>
                                                                       <FieldRef Name='ArticleStartDate' />
                                                                       <Value IncludeTimeValue='TRUE' Type='DateTime'>{2}</Value>
                                                                    </Lt>
                                                                 </And>
                                                              </And>
                                                           </Where><OrderBy>
                                                              <FieldRef Name='ID' Ascending='False' />
                                                           </OrderBy>",
                                                                   FieldsName.CompanyRecord.English.FocusCompany,
                                                                   FieldsName.NewsRecord.English.Status,
                                                                   SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now));

                uint numberOfNews = 5;
                if (!string.IsNullOrEmpty(WebpartParent.NumberOfNews))
                {
                    try
                    {
                        numberOfNews = Convert.ToUInt16(WebpartParent.NumberOfNews);
                    }
                    catch (Exception ex)
                    { }
                }

                var focusNewsTable = Utilities.GetNewsRecordItems(focusCompanyQuery, Convert.ToUInt16(numberOfNews), ListsName.English.CompanyRecord);

                var companyList = focusNewsTable.GetDataTable();

                if (companyList.Rows.Count > 0)
                {
                    companyList.Columns.Add("LinkToItem", typeof(String));
                    for (int i = 0; i < companyList.Rows.Count; i++)
                    {
                        imagepath = Convert.ToString(companyList.Rows[i][FieldsName.NewsRecord.English.ThumbnailImage]);
                        if (imagepath.Length > 2)
                            companyList.Rows[i][FieldsName.NewsRecord.English.ThumbnailImage] = imagepath.Trim().Substring(0, imagepath.Length - 2);

                        companyList.Rows[i]["LinkToItem"] = string.Format("{0}/{1}.aspx?ListCategoryName={2}&ListName={3}&{4}={5}",
                                                               SPContext.Current.Web.Url,
                                                               Constants.PageInWeb.DetailNews,
                                                               ListsName.English.CompanyCategory,
                                                               ListsName.English.CompanyRecord,
                                                               Constants.NewsId,
                                                               Convert.ToString(companyList.Rows[i]["ID"]));
                    }
                    rptFocusCompany.DataSource = companyList;
                    rptFocusCompany.DataBind();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
