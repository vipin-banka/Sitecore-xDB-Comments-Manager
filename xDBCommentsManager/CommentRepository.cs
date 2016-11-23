#region

using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Sitecore.Collections;

#endregion

namespace xDBCommentsManager
{
    public class CommentRepository : ICommentRepository
    {
        /// <summary>
        /// Initilize datacontext object.
        /// </summary>
        readonly DataContext _blogContext = new DataContext();

        /// <summary>
        /// Function for retriving the comments from mongodb.
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="approve"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public IList<Comment> Retrieve(string postId, bool? approve, int? pageNumber, int? pageSize, string order)
        {
            // Setting up the comment display order.          
            IMongoQuery query = null;
            IMongoSortBy sortOrder = SortBy<Comment>.Ascending(x => x.Date);
            //if (order == "Ascending")
            //{
                //sort.Ascending();
                sortOrder = SortBy<Comment>.Ascending(x => x.Date);
            //}
            if (order == "Descending")
            {
                sortOrder = SortBy<Comment>.Descending(x => x.Date);
            }



            // Create filter according to postid
            if (!string.IsNullOrEmpty(postId))
            {
                query = Query<Comment>.EQ(p => p.PostId, postId);


            }

            // Append filter according to approve status.
            if (approve.HasValue)
            {

                var approveQuery = Query<Comment>.EQ(p => p.Approved, approve.Value);
                query = query == null ? approveQuery : Query.And(query, approveQuery);


            }

            // Retrieve the comment if pagesize and pagenumber are given.
            if (pageSize != null && pageNumber != null)
            {
                int pageSizeInt = (int)(pageSize);
                int offset = (int)(pageSize * (pageNumber - 1));

                return
                    _blogContext.Comments.Find(query)
                        .SetSortOrder(sortOrder)
                        .SetSkip(offset)
                        .SetLimit(pageSizeInt)
                        .ToList();
            }

            // Retrieve the comment if pagesize and pagenumber not given
            else
            {
                return
                    _blogContext.Comments.Find(query)
                     .SetSortOrder(sortOrder)
                     .ToList();
            }
        }

        /// <summary>
        /// Function for insert comment.
        /// </summary>
        /// <param name="comment"></param>
        public void Insert(Comment comment)
        {
            _blogContext.Comments.Insert(comment);
        }

        /// <summary>
        /// Function for delete comment.
        /// </summary>
        /// <param name="comment"></param>
        public void Delete(Comment comment)
        {

            var filter = Query<Comment>.EQ(x => x.Id, comment.Id);
            _blogContext.Comments.Remove(filter);
        }

        /// <summary>
        /// Function for update comment approve status.
        /// </summary>
        /// <param name="comment"></param>
        public void Update(Comment comment)
        {
            IMongoQuery filter = Query<Comment>.EQ(x => x.Id, comment.Id);
            IMongoUpdate update = Update<Comment>.Set(x => x.Approved, comment.Approved);
            _blogContext.Comments.Update(filter, update);
        }
    }
}
