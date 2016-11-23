#region

using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.StringExtensions;

#endregion

namespace xDBCommentsManager
{
    public class GetSettings
    {
        EmailAttributes setting = new EmailAttributes();

        public EmailAttributes GetSetting(string str = "",Database db=null)
        {
            Database contextDatabase;
            Item currentItem = null;
            if (db != null)
            {
                contextDatabase = db;
            }
            else
            {
                contextDatabase = Context.Database;
            }
            currentItem = !str.IsNullOrEmpty() ? contextDatabase.GetItem(str) : Context.Item;
            //Database context = Context.Database;
            Item settingFolder = contextDatabase.GetItem("{04AF78C1-DECE-4A2F-B44C-5294774DB22C}");
            var settingItems =
                settingFolder.GetChildren()
                    .InnerChildren.Where(x => x.TemplateID.ToString() == "{0504D942-AFAE-4759-9051-4B99E651257C}")
                    .ToList();
            if (currentItem != null)
            {
                var settingItemwithItemId =
                    settingItems.FirstOrDefault(i => i.Fields["Reference Item"].Value == currentItem.ID.ToString());
                var settingItemwithTemplate =
                    settingItems.FirstOrDefault(
                        i => i.Fields["Reference Item"].Value == currentItem.TemplateID.ToString());

                if (settingItemwithItemId != null)
                {
                    return FillValue(settingItemwithItemId);
                }
                else if (settingItemwithTemplate != null)
                {
                    return FillValue(settingItemwithTemplate);
                }
                else
                {
                    foreach (var settingItem in settingItems)
                    {
                        var parentitems = currentItem.Axes.GetAncestors();
                        var item = contextDatabase.GetItem(settingItem.Fields["Reference Item"].Value);
                        if (item != null)
                        {
                            bool containItem = parentitems.Any(itm => itm.ID == item.ID);

                            if (containItem)
                            {
                                return FillValue(settingItem);

                            }
                        }
                        else
                        {
                            return GetSettingsFromConfig();
                        }
                    }
                }

                //return setting;
            }
            return setting;
        }

        private EmailAttributes GetSettingsFromConfig()
        {
            setting.FromEmail = Settings.GetSetting("EMAIL_FROM"); ;
            setting.ToEmail = Settings.GetSetting("EMAIL_TO");
            setting.AdminEmail = Settings.GetSetting("SEND_MAIL_TO_ADMIN_WHEN_NEW_COMMENT_POSTED").ToLower() == "true" ? true : false;
            setting.UserEmail = Settings.GetSetting("SEND_MAIL_TO_USER_WHEN_COMMENT_PUBLISHED").ToLower() == "true" ? true : false;
            setting.AdminMsgSubject = Settings.GetSetting("GREETING_MESSAGE_SUBJECT_FOR_ADMIN");
            setting.AdminMsgBody = Settings.GetSetting("GREETING_MESSAGE_BODY_FOR_ADMIN");
            setting.UserMsgSubject = Settings.GetSetting("GREETING_MESSAGE_SUBJECT_FOR_USER");
            setting.UserMsgBody = Settings.GetSetting("GREETING_MESSAGE_BODY_FOR_USER");
            setting.SiteKey = Settings.GetSetting("SITE_KEY");
            setting.SecretKey = Settings.GetSetting("SECRET_KEY");
            return setting;
        }

        private EmailAttributes FillValue(Item itm)
        {

            setting.FromEmail = itm.Fields["From Email Address"].Value;
            setting.ToEmail = itm.Fields["To Email Address"].Value;
            setting.AdminEmail = itm.Fields["Enable Admin Email"].Value == "1" ? true : false;
            setting.UserEmail = itm.Fields["Enable User Email"].Value == "1" ? true : false;
            setting.AdminMsgSubject = itm.Fields["Greeting Message Subject For Admin"].Value;
            setting.AdminMsgBody = itm.Fields["Greeting Message Body For Admin"].Value;
            setting.UserMsgSubject = itm.Fields["Greeting Message Subject For User"].Value;
            setting.UserMsgBody = itm.Fields["Greeting Message Body For User"].Value;
            setting.SiteKey = itm.Fields["Site Key"].Value;
            setting.SecretKey = itm.Fields["Secret Key"].Value;
            return setting;
        }
    }

    public class GetCommonSettings
    {
        public PageSizeAttributes GetCommonSetting(Database db = null)
        {
            Database context;
            if (db != null)
            {
                context = db;
            }
            else
            {
                context = Context.Database; 
            }

            Item commonSettingItem = context.GetItem("{D04761B7-7CDE-459C-83AD-0A439532D3EB}");
            PageSizeAttributes pageSizeAttributes = new PageSizeAttributes();
            if (!string.IsNullOrWhiteSpace(commonSettingItem.Fields["Comment Editor Page Size"].ToString()) &&
                !string.IsNullOrWhiteSpace(commonSettingItem.Fields["Comments On Page Load"].ToString()) &&
                !string.IsNullOrWhiteSpace(commonSettingItem.Fields["Comments On Load More Button Click"].ToString()))
            {
                pageSizeAttributes.PageSize = int.Parse(commonSettingItem.Fields["Comment Editor Page Size"].ToString());
                pageSizeAttributes.CommentOnPageLoad = int.Parse(commonSettingItem.Fields["Comments On Page Load"].ToString());
                pageSizeAttributes.CommentOnLoadMore = int.Parse(commonSettingItem.Fields["Comments On Load More Button Click"].ToString());
            }
            else
            {
                pageSizeAttributes.PageSize = int.Parse(Settings.GetSetting("COMMENT_EDITOR_PAGE_SIZE"));
                pageSizeAttributes.CommentOnPageLoad = int.Parse(Settings.GetSetting("COMMENTS_ON_PAGE_LOAD"));
                pageSizeAttributes.CommentOnLoadMore = int.Parse(Settings.GetSetting("COMMENTS_ON_LOAD_MORE_CLICK"));
            }
            return pageSizeAttributes;
        }
    }
    public class EmailAttributes
    {
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public bool AdminEmail { get; set; }
        public bool UserEmail { get; set; }
        public string AdminMsgSubject { get; set; }
        public string AdminMsgBody { get; set; }
        public string UserMsgSubject { get; set; }
        public string UserMsgBody { get; set; }
        public string SiteKey { get; set; }
        public string SecretKey { get; set; }

    }

    public class PageSizeAttributes
    {
        public int PageSize { get; set; }
        public int CommentOnPageLoad { get; set; }
        public int CommentOnLoadMore { get; set; }
    }
}
