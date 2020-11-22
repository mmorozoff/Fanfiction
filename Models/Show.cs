using System.Collections.Generic;

namespace Fanfiction.Models
{
    public class Show
    {
        public int ID { set; get; }
        public int FID { set; get; }
        public string AuthorName { set; get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Score { set; get; }
        public int Size { set; get; }
        public int Prev { set; get; }
        public int Next { set; get; }
        public ICollection<Tag> Tags { set; get; }
        public ICollection<Genre> Genres { set; get; }
        public string ChName { set; get; }
        public string Text { set; get; }
        public int ChScore { set; get; }
        public string CommentText { set; get; }
        public ICollection<Comment> Comments { set; get; } = new List<Comment>();
    }
}
