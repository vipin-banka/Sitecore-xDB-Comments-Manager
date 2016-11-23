#region

using System;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;
using xDBCommentsManager;

#endregion

namespace Web.Components.CommentsWF.Sublayouts
{
    public partial class NewComment : UserControl
    {
        readonly ICommentRepository _objRepository = new CommentRepository();
        private readonly ID _currentItem = Sitecore.Context.Item.ID;
        private string siteKey = "";
        GetSettings getSettings = new GetSettings();

        protected void Page_Load(object sender, EventArgs e)
        {
            // GetSettings getSettings=new GetSettings();
            var settingsValue = getSettings.GetSetting();
            siteKey = settingsValue.SiteKey;
            if (!siteKey.IsNullOrEmpty())
            {
                recaptchaPanel.Attributes.Add("data-sitekey", siteKey);
            }
        }

        /// <summary>
        /// Function for insert new comment.
        /// </summary>

        protected void InsertNewComment()
        {
            try
            {
                Comment cmt = new Comment();
                cmt.PostId = _currentItem.ToString();
                cmt.Author = txtauthorname.Text;
                cmt.Email = txtEmail.Text;
                cmt.Date = DateTime.Now;
                cmt.Body = txtComment.Text;
                _objRepository.Insert(cmt);
                SendMailToAdmin();
                txtauthorname.Text = txtEmail.Text = txtComment.Text = "";
                lblInfoText.Text = "Thank you for submiting your valuable comment";
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
                lblInfoText.Text = "Please try again later";
            }
        }

        public void ValidateCaptchaKey(object sender, EventArgs e)
        {
            Recaptcha recaptcha = new Recaptcha();
            string CaptchaResponse = Request["g-recaptcha-response"];
            if (recaptcha.Validate(CaptchaResponse) && !siteKey.IsNullOrEmpty())
            {
                InsertNewComment();
            }
            else
            {
                lblInfoText.Text = "Captcha not filled or not match";
            }
            if (siteKey.IsNullOrEmpty())
            {
                InsertNewComment();
            }
        }

        public void SendMailToAdmin()
        {
            var settingsValue = getSettings.GetSetting();
            if (settingsValue.AdminEmail && !string.IsNullOrEmpty(settingsValue.FromEmail) && !string.IsNullOrEmpty(settingsValue.ToEmail) && !string.IsNullOrEmpty(settingsValue.AdminMsgSubject) && !string.IsNullOrEmpty(settingsValue.AdminMsgBody))
            {
                Utility.SendMail(settingsValue.FromEmail, settingsValue.ToEmail, settingsValue.AdminMsgSubject, settingsValue.AdminMsgBody);
            }

        }

    }

}