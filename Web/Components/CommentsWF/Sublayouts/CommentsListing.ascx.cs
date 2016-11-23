#region

using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data;
using xDBCommentsManager;

#endregion

namespace Web.Components.CommentsWF.Sublayouts
{
    public partial class CommentEditor : UserControl
    {
        readonly ICommentRepository _objRepository = new CommentRepository();
        private readonly ID _currentItem = Sitecore.Context.Item.ID;
        int _resultOnPageLoad, _resultPerClick;

        /// <summary>
        /// Display the comments on page load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            GetCommonSettings objCommonSettings = new GetCommonSettings();
            var setting = objCommonSettings.GetCommonSetting();
            _resultOnPageLoad = setting.CommentOnPageLoad;
            _resultPerClick = setting.CommentOnLoadMore;
            hfGUID.Value = Sitecore.Context.Item.ID.ToString();
            hfResultsPerClick.Value = _resultPerClick.ToString();
            hftotalRecords.Value = _objRepository.Retrieve(_currentItem.ToString(), true, null, null, "Ascending").Count().ToString();
            hfresultOnPageLoad.Value = _resultOnPageLoad.ToString();
            if (_currentItem.ToString() != "")
            {
                var comments = _objRepository.Retrieve(_currentItem.ToString(), true, null, null, "Ascending");
                if (comments.Any())
                {
                    rptComments.DataSource = comments.Take(_resultOnPageLoad);
                    rptComments.DataBind();
                }
            }
        }

        /// <summary>
        /// Data bind of repeator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptComments_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var itm = (Comment)e.Item.DataItem;
                if (itm == null) return;
                Literal ltAuthor = (Literal)e.Item.FindControl("ltAuthor");
                Literal ltComment = (Literal)e.Item.FindControl("ltComment");
                Literal ltDate = (Literal)e.Item.FindControl("ltDate");
                if (ltAuthor != null)
                {
                    ltAuthor.Text = itm.Author;
                }
                if (ltAuthor != null)
                {
                    ltComment.Text = itm.Body;
                }
                if (ltAuthor != null)
                {
                    ltDate.Text = itm.Date.ToString(CultureInfo.InvariantCulture);
                }


            }
        }
    }
}