#region

using System;
using MongoDB.Bson;

#endregion

namespace xDBCommentsManager
{
    /// <summary>
    /// Class for mongodb attributes.
    /// </summary>
    public class Comment
    {
        public ObjectId Id { get; set; }

        public string PostId { get; set; }

        public string Author { get; set; }

        public string Email { get; set; }

        public DateTime Date { get; set; }

        public string Body { get; set; }

        public bool Approved { get; set; }
    }
}
