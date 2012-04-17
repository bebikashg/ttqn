﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using CQ.SharePoint.QN.Core.Helpers;
using CQ.SharePoint.QN.Core.WebParts;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.HtmlControls;

namespace CQ.SharePoint.QN.Common
{
    public class Utilities
    {
        /// <summary>
        /// Create a standard SharePoint view and add to given list
        /// </summary>
        /// <param name="list">List instance that contains added view</param>
        /// <param name="viewTitle"></param>
        /// <param name="viewFields">Fields display on view</param>
        /// <param name="query">Filter and Groupby query</param>
        /// <param name="rowLimit"></param>
        /// <param name="makeDefaultView">TRUE to make view is default view of list</param>
        public static void AddStandardView(SPList list, string viewTitle, string[] viewFields, string query, int rowLimit, bool makeDefaultView)
        {
            AddStandardView(list, viewTitle, viewFields, query, rowLimit, makeDefaultView, String.Empty);
        }

        /// <summary>
        /// Create a standard SharePoint view and add to given list
        /// </summary>
        /// <param name="list">List instance that contains added view</param>
        /// <param name="viewTitle"></param>
        /// <param name="viewFields">Fields display on view</param>
        /// <param name="query">Filter and Groupby query</param>
        /// <param name="rowLimit"></param>
        /// <param name="makeDefaultView">TRUE to make view is default view of list</param>
        /// <param name="aggregations">Aggregation Expression String</param>
        public static void AddStandardView(SPList list, string viewTitle, string[] viewFields, string query, int rowLimit, bool makeDefaultView, String aggregations)
        {
            SPViewCollection availableViews = list.Views;
            SPView view;

            try
            {
                view = availableViews[viewTitle];

                // If view exsited, update new view fields

                if (viewFields != null)
                {
                    view.ViewFields.DeleteAll();
                    foreach (string viewField in viewFields)
                        view.ViewFields.Add(viewField);
                }
            }
            catch
            {
                // If view not exsited, 
                // Create new view

                var colViewCollection = new StringCollection();
                if (viewFields != null) colViewCollection.AddRange(viewFields);
                view = availableViews.Add(viewTitle, colViewCollection, null, (uint)rowLimit, true, makeDefaultView);
            }

            if (view != null)
            {
                // Update view filter

                if (!string.IsNullOrEmpty(query))
                {
                    view.Query = query;
                }

                view.RowLimit = (uint)rowLimit;
                view.DefaultView = makeDefaultView;
                if (!String.IsNullOrEmpty(aggregations))
                {
                    view.Aggregations = aggregations;
                    view.AggregationsStatus = "No";
                }

                view.Update();
            }
        }

        public static void AddForms(SPWeb web, SPList list, string userControlPath)
        {
            ContainerWebPart containerWebPart = WebPartHelper.GetContainerWebPart(web);

            if (containerWebPart != null)
            {
                containerWebPart.Title = string.Format(CultureInfo.InvariantCulture, "{0} - Custom Form", list.Title);
                containerWebPart.UserControlPath = userControlPath;
                WebPartHelper.AddWebPartToNewPage(web, list, containerWebPart);
                WebPartHelper.AddWebPartToEditPage(web, list, containerWebPart);
                WebPartHelper.AddWebPartToDisplayPage(web, list, containerWebPart);
            }

            WebPartHelper.HideDefaultWebPartOnNewPage(web, list);
            WebPartHelper.HideDefaultWebPartOnEditPage(web, list);
            WebPartHelper.HideDefaultWebPartOnDisplayPage(web, list);
        }

        /// <summary>
        /// Gets User of People Picker field (Single choice)
        /// </summary>
        /// <param name="item">Current item</param>
        /// <param name="fieldName">Field Name</param>
        /// <returns>Return SPUser</returns>
        public static SPUser GetUserByField(SPListItem item, string fieldName)
        {
            var value = Convert.ToString(item[fieldName], CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(value)) return null;

            var userFieldValue = new SPFieldUserValue(item.Web, value);
            if (userFieldValue != null && userFieldValue.User != null)
                return userFieldValue.User;

            return null;
        }

        /// <summary>
        /// Gets User of People Picker field (Multi )
        /// </summary>
        /// <param name="item">Current item</param>
        /// <param name="fieldName">Field Name</param>
        /// <returns>Return SPUserCollection</returns>
        public static SPFieldUserValueCollection GetUserValueCollectionByField(SPListItem item, string fieldName)
        {
            var value = Convert.ToString(item[fieldName], CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(value)) return null;

            var userFieldValueCollection = new SPFieldUserValueCollection(item.Web, value);
            if (userFieldValueCollection != null && userFieldValueCollection.Count > 0)
            {
                return userFieldValueCollection;
            }

            return null;
        }
        /// <summary>
        /// Log exception to SharePoint log
        /// </summary>
        /// <param name="value"></param>
        public static void LogToUls(string value)
        {
            //SPDiagnosticsService diagnosticsService = SPDiagnosticsService.Local;
            //SPDiagnosticsCategory category = diagnosticsService.Areas["SharePoint Foundation"].Categories["General"];
            //diagnosticsService.(93, category, TraceSeverity.High, "CQ.SharePoint.QN: " + value, null);
        }

        /// <summary>
        /// LogToULS
        /// </summary>
        /// <param name="ex"></param>
        public static void LogToUls(Exception ex)
        {
            //var diagnosticsService = SPDiagnosticsService.Local;
            //var category = diagnosticsService.Areas["SharePoint Foundation"].Categories["General"];
            //diagnosticsService.WriteTrace(93, category, TraceSeverity.High, "CQ.SharePoint.QN: " + ex, null);
        }

        /// <summary>
        /// Get all metadata of choice field from SP List
        /// </summary>
        /// <param name="list">Current List</param>
        /// <param name="fieldName">Field Name</param>
        /// <param name="defaultValue">Default value of field</param>
        /// <returns></returns>
        public static StringCollection GetMetaDataOfChoiceField(SPList list, string fieldName, out string defaultValue)
        {
            defaultValue = string.Empty;
            var values = new StringCollection();

            SPFieldCollection fields = list.Fields;

            if (fields.ContainsField(fieldName))
            {
                var fieldChoice = (SPFieldChoice)fields[fieldName];
                values = fieldChoice.Choices;

                defaultValue = fieldChoice.DefaultValue;
            }

            return values;
        }

        /// <summary>
        /// Apply permission for list
        /// </summary>
        /// <param name="web">Current Web</param>
        /// <param name="list">Current list</param>
        /// <param name="groupsAndPermissions">Text - Group Name; Array ListItem: Value - Permission Level</param>
        public static void AplyPermissionForLists(SPWeb web, SPList list, params ListItem[] groupsAndPermissions)
        {
            if (groupsAndPermissions.Length <= 0) return;

            if (!list.HasUniqueRoleAssignments)
            {
                web.AllowUnsafeUpdates = true;
                list.BreakRoleInheritance(false);
            }
            else
            {
                web.AllowUnsafeUpdates = true;
                list.ResetRoleInheritance();
                web.AllowUnsafeUpdates = true;
                list.BreakRoleInheritance(false);
            }

            SPRoleAssignmentCollection roleAssignments = list.RoleAssignments;

            foreach (ListItem item in groupsAndPermissions)
            {
                try
                {
                    SPRoleAssignment roleAssignment = new SPRoleAssignment(web.SiteGroups[item.Text]);
                    SPRoleDefinition roleDefinition = web.RoleDefinitions[item.Value];
                    roleAssignment.RoleDefinitionBindings.Add(roleDefinition);

                    roleAssignments.Add(roleAssignment);
                }
                catch
                {
                    continue;
                }
            }

            web.AllowUnsafeUpdates = true;
            list.Update();
        }

        /// <summary>
        /// AddPermissionForList
        /// </summary>
        /// <param name="web"></param>
        /// <param name="list"></param>
        /// <param name="groupName"></param>
        /// <param name="roleType"></param>
        public static void AddPermissionForList(SPWeb web, SPList list, string groupName, SPRoleType roleType)
        {
            if (!list.HasUniqueRoleAssignments)
            {
                web.AllowUnsafeUpdates = true;
                list.BreakRoleInheritance(false);
            }

            SPGroup group = web.SiteGroups[groupName];
            SPRoleDefinition roleDefinition = web.RoleDefinitions.GetByType(roleType);
            SPRoleAssignment roleAssignment = new SPRoleAssignment(group);
            roleAssignment.RoleDefinitionBindings.Add(roleDefinition);

            list.RoleAssignments.Add(roleAssignment);
            list.Update();
            web.Update();
        }

        /// <summary>
        /// Add Permission For Group
        /// </summary>
        /// <param name="web">current web</param>
        /// <param name="groupName">group name</param>
        /// <param name="permssionLevel">permission level name</param>
        public static void AddPermissionForGroup(SPWeb web, string groupName, string permssionLevel)
        {
            SPRoleDefinition roleDefinition = web.RoleDefinitions[permssionLevel];

            var assignment = new SPRoleAssignment(web.SiteGroups[groupName]);
            assignment.RoleDefinitionBindings.Add(roleDefinition);

            web.RoleAssignments.Add(assignment);
        }

        /// <summary>
        /// The method to add usercontrol to page
        /// </summary>
        /// <param name="web">Current web</param>
        /// <param name="pageName">Page Name: only name and except extension (.aspx)</param>
        /// <param name="pageTitle">Title of page</param>
        /// <param name="userControlName">UserControl name: only name and except extension (.ascx)</param>
        public static void AddUserControlToPage(SPWeb web, string pageName, string pageTitle, string userControlName)
        {
            ContainerWebPart containerWebPart = WebPartHelper.GetContainerWebPart(web);
            if (containerWebPart != null)
            {
                containerWebPart.Title = pageTitle;
                containerWebPart.UserControlPath = "UserControls/" + userControlName + ".ascx";
                WebPartHelper.AddWebPart(web, pageName + ".aspx", containerWebPart, "Main", 0);
            }
        }

        /// <summary>
        /// GetItemByField
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="typeValue"></param>
        /// <returns></returns>
        public static SPListItem GetItemByField(SPList list, string fieldName, string fieldValue, string typeValue)
        {
            var queryItem = new SPQuery
            {
                Query =
                    "<Where><Eq><FieldRef Name='" + fieldName + "' /><Value Type='" + typeValue + "'>" + fieldValue +
                    "</Value></Eq></Where>",
                RowLimit = 1
            };
            var itemArray = list.GetItems(queryItem);
            if (itemArray != null && itemArray.Count > 0)
            {
                return itemArray[0];
            }
            return null;
        }

        /// <summary>
        /// Add Event for lists
        /// </summary>
        /// <param name="list">Current list</param>
        /// <param name="eventReceiverType">Event Type parameter</param>
        /// <param name="className">className</param>
        public static void AddEventForList(SPList list, SPEventReceiverType eventReceiverType, string className)
        {
            list.EventReceivers.Add(eventReceiverType, typeof(Utilities).Assembly.FullName, "CQ.SharePoint.QN.EventReceivers." + className);
        }

        /// <summary>
        /// BreakItem
        /// </summary>
        /// <param name="web"></param>
        /// <param name="item"></param>
        public static void BreakItem(SPWeb web, SPListItem item)
        {
            if (!item.HasUniqueRoleAssignments)
            {
                web.AllowUnsafeUpdates = true;
                item.BreakRoleInheritance(false);
            }
            else
            {
                web.AllowUnsafeUpdates = true;
                item.ResetRoleInheritance();
                web.AllowUnsafeUpdates = true;
                item.BreakRoleInheritance(false);
            }
            web.AllowUnsafeUpdates = true;
            item.SystemUpdate();
        }

        /// <summary>
        /// ApplyPermissionForItem
        /// </summary>
        /// <param name="web"></param>
        /// <param name="item"></param>
        /// <param name="permissonLevel"></param>
        /// <param name="user"></param>
        public static void ApplyPermissionForItem(SPWeb web, SPListItem item, string permissonLevel, SPPrincipal user)
        {
            var roleAssignment = new SPRoleAssignment(user);
            SPRoleDefinition roleDefinition = web.RoleDefinitions[permissonLevel];
            roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
            item.RoleAssignments.Add(roleAssignment);
            web.AllowUnsafeUpdates = true;
            item.SystemUpdate();
        }

        /// <summary>
        /// Get item from list by userID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fieldName"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static SPListItem GetItemdByUserId(int userId, string fieldName, SPList list)
        {
            var spQuery = new SPQuery
                              {
                                  Query = string.Format(CultureInfo.InvariantCulture, "<Where><Eq><FieldRef Name='{0}' LookupId='TRUE' /><Value Type='User'>{1}</Value></Eq></Where>", fieldName, userId)
                              };
            var itemArray = list.GetItems(spQuery);
            if (itemArray != null && itemArray.Count > 0)
            {
                return itemArray[0];
            }
            return null;
        }

        /// <summary>
        /// format string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GoodStringForHtml(string value)
        {
            var result = value.Trim();
            result = result.Replace("'", "\'");
            result = result.Replace("\r\n", "");
            result = result.Replace("\n\r", "");
            result = result.Replace("\n", "");
            result = result.Replace("\r", "");
            return result;
        }

        /// <summary>
        /// format string
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>string</returns>
        public static string GoodStringForSql(string value)
        {
            var result = value.Trim();
            result = result.Replace("'", "''");
            return result;
        }

        /// <summary>
        /// GetListFromUrl
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public static DataTable GetListFromUrl(string listName)
        {
            DataTable table = null;
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var site = new SPSite(SPContext.Current.Web.Site.ID))
                {
                    using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        try
                        {
                            string listUrl = web.Url + "/Lists/" + listName;
                            var result = web.GetList(listUrl);
                            SPQuery query = new SPQuery();
                            var items = result.GetItems(query);
                            if (items != null && items.Count > 0)
                                table = items.GetDataTable();
                        }
                        catch (Exception ex)
                        {
                            Utilities.LogToUls(ex);
                        }
                    }

                }
            });
            return table;
        }

        /// <summary>
        /// GetListFromUrl
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        public static SPList GetListFromUrl(SPWeb web, string listName)
        {
            SPList result = null;
            try
            {
                string listUrl = web.Url + "/Lists/" + listName;
                result = web.GetList(listUrl);
            }
            catch (Exception ex)
            {
                Utilities.LogToUls(ex);
            }
            return result;
        }

        /// <summary>
        /// GetDocListFromUrl
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        public static SPList GetDocListFromUrl(SPWeb web, string listName)
        {
            SPList result = null;
            try
            {
                string listUrl = web.Url + "/" + listName;
                result = web.GetList(listUrl);
            }
            catch (Exception ex)
            {
                Utilities.LogToUls(ex);
            }
            return result;
        }

        public static DataTable GetNewsRecords(string query, uint newsNumber, string listName)
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
                            SPList list = Utilities.GetListFromUrl(web, listName);
                            if (list != null)
                            {
                                SPListItemCollection items = list.GetItems(spQuery);
                                if (items != null && items.Count > 0)
                                {
                                    table = items.GetDataTable();
                                }
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
        /// Get value in resource file by key
        /// </summary>
        /// <param name="resourceFileName">Resource File Name</param>
        /// <param name="resourceKey">Resource Key</param>
        /// <returns></returns>
        public static string GetMessageFromResourceFile(string resourceFileName, string resourceKey)
        {
            try
            {
                var lang = System.Threading.Thread.CurrentThread.CurrentUICulture.LCID;
                return SPUtility.GetLocalizedString(string.Format(CultureInfo.InvariantCulture, "$Resources:{0}", resourceKey), resourceFileName, (uint)lang);
            }
            catch (Exception ex)
            {
                Utilities.LogToUls(ex);
            }

            return "";
        }

        /// <summary>
        /// This method will return string not have spical character
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertReturnString(string value)
        {
            value = value.Trim();
            value = value.Replace("'", "\\'");
            value = value.Replace("\r\n", "");
            value = value.Replace("\n\r", "");
            value = value.Replace("\n", "");
            value = value.Replace("\r", "");
            return value;
        }

        /// <summary>
        /// GetDateString
        /// </summary>
        /// <param name="inputDate"></param>
        /// <returns></returns>
        public static string GetDateString(DateTime inputDate)
        {
            var month = Convert.ToString(inputDate.Month, CultureInfo.InvariantCulture);
            if (month.Length == 1)
            {
                month = "0" + month;
            }
            var day = Convert.ToString(inputDate.Day, CultureInfo.InvariantCulture);
            if (day.Length == 1)
            {
                day = "0" + day;
            }
            return Convert.ToString(inputDate.Year, CultureInfo.InvariantCulture) + "/" + month + "/" + day;
        }

        /// <summary>
        /// LoadJS
        /// </summary>
        /// <param name="web"></param>
        /// <param name="page"></param>
        /// <param name="name"></param>
        public static void LoadJS(SPWeb web, Page page, string name)
        {
            // Load Js
            SPListItem js = Utilities.GetResourceByType(web, FieldsName.CQQNResources.FieldValuesDefault.Type.JS, name);

            if (js != null)
            {
                string jsUrl = web.Url + "/" + js.Url;
                page.ClientScript.RegisterClientScriptInclude(js.ID + "js", jsUrl);
            }
        }

        /// <summary>
        /// LoadCSS
        /// </summary>
        /// <param name="web"></param>
        /// <param name="page"></param>
        /// <param name="name"></param>
        public static void LoadCSS(SPWeb web, Page page, string name)
        {
            // Load Css
            SPListItem css = Utilities.GetResourceByType(web, FieldsName.CQQNResources.FieldValuesDefault.Type.CSS, name);
            if (css != null)
            {
                string cssUrl = web.Url + "/" + css.Url;

                HtmlLink styleSheet = new HtmlLink
                {
                    Href = cssUrl,
                    ID = css.ID + "_Css",

                };
                styleSheet.Attributes["rel"] = "stylesheet";
                styleSheet.Attributes["type"] = "text/css";
                styleSheet.Attributes["media"] = "all";

                page.Header.Controls.Add(styleSheet);
            }
        }

        /// <summary>
        /// GetResourceByType
        /// </summary>
        /// <param name="web"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SPListItem GetResourceByType(SPWeb web, string type, string name)
        {
            SPList resource = null;

            try
            {
                string listUrl = web.Url + "/" + ListsName.English.CQQNResources;
                resource = web.GetList(listUrl);
            }
            catch (Exception ex)
            {
                Utilities.LogToUls(ex);
            }

            string CAML = @"<Where><And><Eq><FieldRef Name='{0}' /><Value Type='Text'>{1}</Value></Eq><Eq><FieldRef Name='{2}' /><Value Type='Text'>{3}</Value></Eq></And></Where>";

            SPQuery query = new SPQuery()
            {
                Query = string.Format(CultureInfo.InvariantCulture, CAML, FieldsName.CQQNResources.English.ResourceType, type, FieldsName.CQQNResources.English.FileLeafRef, name),
                RowLimit = 1
            };

            SPListItem item = null;

            try
            {
                item = resource.GetItems(query)[0];
            }
            catch (Exception ex)
            {
                Utilities.LogToUls(ex);
                throw ex;
            }

            return item;
        }
    }
}