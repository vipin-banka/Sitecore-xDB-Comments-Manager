#region

using System.Collections.Generic;

#endregion

namespace xDBCommentsManager
{
    public interface ICommentRepository
    {
        void Insert(Comment comment);

        IList<Comment> Retrieve(string postId, bool? approve, int? pageNumber, int? pageSize, string order);

        void Update(Comment comment);

        void Delete(Comment comment);
    }
}