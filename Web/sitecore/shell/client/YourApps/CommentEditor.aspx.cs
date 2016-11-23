#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data;
using xDBCommentsManager;

#endregion

namespace Web.sitecore.shell.client.YourApps
{
    public partial class CommentEditor : Page
    {
        readonly ICommentRepository _objRepository = new CommentRepository();
        protected void Page_Load(object sender, EventArgs e)
        {
            GetCurrentItem();
            Pub = _objRepository.Retrieve(Result, null, null, null, "Descending").ToList();
            if (!IsPostBack)
            {
                LoadComments();
            }
        }

        /// <summary>
        /// Function for get the current item.
        /// </summary>
        public void GetCurrentItem()
        {
            var currentClientRawUrl = Sitecore.Context.RawUrl;
            if (currentClientRawUrl != null)
            {
                int pFrom = currentClientRawUrl.IndexOf("%7B", StringComparison.Ordinal) + "%7B".Length;
                int pTo = currentClientRawUrl.IndexOf("%7D", StringComparison.Ordinal);
                Result = "{" + currentClientRawUrl.Substring(pFrom, pTo - pFrom) + "}";
            }
        }

        /// <summary>
        /// Load all the comments in sitecore comment editor.
        /// </summary>
        private void LoadComments()
        {
            GetCommonSettings newGetCommonSettings = new GetCommonSettings();
            Database context = Sitecore.Context.ContentDatabase;
            var commonSetting = newGetCommonSettings.GetCommonSetting(context);
            if (Pub.Any())
            {
                lblcommentstatus.Visible = false;
                GridComments.PageSize = commonSetting.PageSize;
                GridComments.DataSource = Pub;
                GridComments.DataBind();
                btnPublish.Visible = true;
                btnDelete.Visible = true;
                CheckApproveComment();
            }
            else
            {
                lblcommentstatus.Visible = true;
                lblcommentstatus.Text = "No comments found";
            }
        }

        /// <summary>
        /// Function for checking approve status of comments.
        /// </summary>
        protected void CheckApproveComment()
        {

            if (Pub.Any())
            {
                foreach (GridViewRow row in GridComments.Rows)
                {
                    // Access the CheckBox
                    CheckBox cb = (CheckBox)row.FindControl("chkPublish");

                    if (cb != null)
                    {
                        var approveStatus = Pub[row.DataItemIndex].Approved;
                        if (approveStatus)
                        {
                            cb.Checked = true;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Function for update the approve status of comments.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApproveSelectedComment(object sender, EventArgs e)
        {

            if (Pub.Any())
            {
                foreach (GridViewRow row in GridComments.Rows)
                {
                    // Access the CheckBox
                    CheckBox cb = (CheckBox)row.FindControl("chkPublish");
                    if (cb != null && cb.Checked && !Pub[row.DataItemIndex].Approved)
                    {
                        Pub[row.DataItemIndex].Approved = true;
                        _objRepository.Update(Pub[row.DataItemIndex]);
                        string userEmail = Pub[row.DataItemIndex].Email;
                        Database context = Sitecore.Context.ContentDatabase;
                        GetSettings getSettings = new GetSettings();
                        var settingsValue = getSettings.GetSetting(Result, context);

                        if (settingsValue.UserEmail && !string.IsNullOrEmpty(settingsValue.FromEmail) && !string.IsNullOrEmpty(userEmail) &&
                            !string.IsNullOrEmpty(settingsValue.UserMsgSubject) &&
                            !string.IsNullOrEmpty(settingsValue.UserMsgBody))
                        {
                            Utility.SendMail(settingsValue.FromEmail, userEmail, settingsValue.UserMsgSubject,
                                settingsValue.UserMsgBody);

                        }
                    }
                    else if (cb != null && !cb.Checked && Pub[row.DataItemIndex].Approved)
                    {
                        Pub[row.DataItemIndex].Approved = false;
                        _objRepository.Update(Pub[row.DataItemIndex]);
                    }
                }
            }
        }

        /// <summary>
        /// Function for detete the comment from editor window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DeleteSelectedComment(object sender, EventArgs e)
        {
            if (Pub.Any())
            {
                foreach (GridViewRow row in GridComments.Rows)
                {
                    // Access the CheckBox
                    CheckBox cb = (CheckBox)row.FindControl("chkDelete");
                    Comment cmt = Pub[row.DataItemIndex];
                    if (cb != null && cb.Checked)
                    {
                        _objRepository.Delete(cmt);

                    }

                }
            }
        }


        /// <summary>
        /// Changing the index value on pagination.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridComments_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridComments.PageIndex = e.NewPageIndex;
            LoadComments();
        }

        /// <summary>
        /// Function for check all the check in editor window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CheckAll(object sender, EventArgs e)
        {

            CheckBox chkbox = (CheckBox)sender;
            String chkId = chkbox.ID;
            if (chkbox.Checked && chkId != null)
            {

                CheckAll_Click(chkId);
            }
            else if (!chkbox.Checked && chkId != null)
            {
                UncheckAll_Click(chkId);
            }
        }
        protected void CheckAll_Click(string id)
        {
            ToggleCheckState(true, id);
        }
        protected void UncheckAll_Click(string id)
        {
            ToggleCheckState(false, id);
        }

        /// <summary>
        /// Function for toggle the checkbox value in editor window.
        /// </summary>
        /// <param name="checkState"></param>
        /// <param name="id"></param>
        private void ToggleCheckState(bool checkState, string id)
        {
            string chkvalue = string.Empty;
            if (id == "chkboxSelectAll")
            {
                chkvalue = "chkPublish";
            }
            if (id == "chkboxDeleteAll")
            {
                chkvalue = "chkDelete";
            }
            // Iterate through the Products.Rows property
            foreach (GridViewRow row in GridComments.Rows)
            {
                // Access the CheckBox
                CheckBox cb = (CheckBox)row.FindControl(chkvalue);
                if (cb != null)
                    cb.Checked = checkState;
            }
        }
        public string Result { get; set; }

        public List<Comment> Pub { get; set; }
    }
}