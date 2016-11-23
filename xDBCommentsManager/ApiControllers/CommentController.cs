#region

using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.SecurityModel;
using Sitecore.StringExtensions;

#endregion


namespace xDBCommentsManager.ApiControllers
{
    public class CommentController : Controller, IController
    {
        private readonly ICommentRepository _objRepository = new CommentRepository();
        public string BlogpostId = "";


        /// <summary>
        /// Action for adding new comment in mongodb database.
        /// </summary>
        /// <param name="commentAttributes"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewComment(CommentAttributes commentAttributes)
        {
            // Get item url
            var itmUrl = new UriBuilder(LinkManager.GetItemUrl(Context.Database.GetItem(commentAttributes.BlogPostId), new UrlOptions { AlwaysIncludeServerUrl = true }));
            BlogpostId = commentAttributes.BlogPostId;
            Recaptcha recaptcha = new Recaptcha();
            GetSettings objGetSettings = new GetSettings();
            var setting = objGetSettings.GetSetting(BlogpostId);
            commentAttributes.CaptchaResponse = Request["g-recaptcha-response"];
            try
            {
                Comment cmt = new Comment
                {
                    PostId = commentAttributes.BlogPostId,
                    Author = commentAttributes.AuthorName,
                    Email = commentAttributes.AuthorEmail,
                    Date = DateTime.Now,
                    Body = commentAttributes.AuthorComment
                };

                if (setting.SiteKey.IsNullOrEmpty())
                {
                    // Insert comment in comment in mongodb database.
                    _objRepository.Insert(cmt);
                    SendMailToAdmin(BlogpostId);
                    var uri = AddQuery(itmUrl, "status", "success");
                    Response.Redirect(uri.ToString());
                }
                if (!setting.SiteKey.IsNullOrEmpty())
                {
                    if (recaptcha.Validate(commentAttributes.CaptchaResponse, BlogpostId))
                    {
                        // Insert comment in comment in mongodb database.
                        _objRepository.Insert(cmt);
                        SendMailToAdmin(BlogpostId);
                        var uri = AddQuery(itmUrl, "status", "success");
                        Response.Redirect(uri.ToString());
                    }
                    else
                    {
                        Log.Error("Captcha not filled", this);
                        var errorUri = AddQuery(itmUrl, "status", "captchaerror");
                        Response.Redirect(errorUri.ToString());
                    }
                }



            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
                var errorUri = AddQuery(itmUrl, "status", "error");
                Response.Redirect(errorUri.ToString());
            }

            return Json("ok", JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Assign the comment editor to the templates on which item admin want the comment editor for approving the comments.
        /// </summary>
        /// <param name="templateIds"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Assign(TemplateIds templateIds)
        {
            // Get collection of sitecore template ids.
            string sitecoreTempItemIds = templateIds.Ids;

            // Split the ids.
            string[] itmIds = sitecoreTempItemIds.Split('|');
            StringBuilder sb = new StringBuilder();
            foreach (string itm in itmIds)
            {
                // Get sitecore master database
                Database master = Factory.GetDatabase("master");
                //Database master = Sitecore.Context.ContentDatabase;
                //Log.Error(master.ToString(),this);
                // Get sitecore template item by ID.
                Item item = master.GetItem(new ID(itm));

                try
                {
                    // Assign the sitecore item to TemplateItem.
                    TemplateItem itmTemplateItem = item;
                    if (itmTemplateItem.StandardValues != null)
                    {
                        // Get the standard value of item.
                        item = itmTemplateItem.StandardValues;

                        // Call the InsertEditor function.
                        InsertEditor(item);

                        // Add success message for item.
                        sb.Append("Editor Assigned Successfully on " + itmTemplateItem.DisplayName + "\n");
                    }

                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, this);
                }

            }
            // Return the success item list. 
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Insert editor function
        /// </summary>
        /// <param name="itm"></param>
        private void InsertEditor(Item itm)
        {
            // Disable the sitecore security for updating the sitecore item.
            using (new SecurityDisabler())
            {
                if (itm != null)
                {
                    itm.Editing.BeginEdit();

                    // Get the __editors field value.
                    var editorVal = itm.Fields["__Editors"].Value;

                    // Checking if editor is not already assign.
                    if (!editorVal.Contains("{5BA2FC20-45DD-4F32-B66A-85CBAC412DE9}"))
                    {
                        if (String.IsNullOrEmpty(editorVal))
                        {
                            //Set Editors without existing editors.
                            itm.Fields["__Editors"].Value = "{5BA2FC20-45DD-4F32-B66A-85CBAC412DE9}";

                        }
                        else
                        {
                            //Set Editors with existing editors.
                            itm.Fields["__Editors"].Value = editorVal + "|{5BA2FC20-45DD-4F32-B66A-85CBAC412DE9}";

                        }

                    }

                    itm.Editing.EndEdit();
                }
            }
        }


        /// <summary>
        /// Load more comment function.
        /// </summary>
        /// <param name="loadMore"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLoadMoreData(LoadMore loadMore)
        {
            try
            {
                // Get current item ID.
                var currentItem = loadMore.itemID;

                // Get total comment count.
                var totalcount = int.Parse(loadMore.totalcount);

                // Get item per page for display.
                var itemsperpage = int.Parse(loadMore.itemsperpage);

                // Retrieve the comments from mongodb database.
                var comments = _objRepository.Retrieve(currentItem, true, null, null, "");

                // Return the list of item for display.
                return Json(comments.Skip(totalcount).Take(itemsperpage));


            }
            catch (Exception e)
            {
                return Json(new { success = false, ex = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Uri builder function.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Uri AddQuery(UriBuilder uri, string name, string value)
        {
            var ub = new UriBuilder(uri.Uri);

            // decodes urlencoded pairs from uri.Query to HttpValueCollection.
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Add(name, value);

            // urlencodes the whole HttpValueCollection.
            ub.Query = httpValueCollection.ToString();

            return ub.Uri;
        }

        public void SendMailToAdmin(string blogpostid)
        {
            GetSettings getSettings = new GetSettings();
            var settingsValue = getSettings.GetSetting(blogpostid);
            if (settingsValue.AdminEmail && !string.IsNullOrEmpty(settingsValue.FromEmail) && !string.IsNullOrEmpty(settingsValue.ToEmail) && !string.IsNullOrEmpty(settingsValue.AdminMsgSubject) && !string.IsNullOrEmpty(settingsValue.AdminMsgBody))
            {
                Utility.SendMail(settingsValue.FromEmail, settingsValue.ToEmail, settingsValue.AdminMsgSubject, settingsValue.AdminMsgBody);
            }

        }
    }


    /// <summary>
    /// Class with attribute for insert comment.
    /// </summary>
    public class CommentAttributes
    {
        public string BlogPostId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string AuthorComment { get; set; }
        public string CaptchaResponse { get; set; }
    }

    /// <summary>
    /// Class with attribute for assign editor.
    /// </summary>
    public class TemplateIds
    {
        public string Ids { get; set; }
    }

    /// <summary>
    /// Class with attribute for load more comments.
    /// </summary>
    public class LoadMore
    {
        public string itemID { get; set; }
        public string itemsperpage { get; set; }
        public string totalcount { get; set; }
        public string pagenumber { get; set; }
    }
}