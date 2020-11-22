using System;
using System.Collections.Generic;

namespace Fanfiction.Models
{
    
    public class Fanfic
    {
        public int ID { set; get; }
        public string AuthorName { set; get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Score { set; get; }
        public ICollection<Score> Scores { set; get; }
        public int Size { set; get; }
        public ICollection<Chapter> Chapters { set; get; } = new List<Chapter>();
        public ICollection<Tag> Tags { set; get; } = new List<Tag>();
        public ICollection<Genre> Genres { set; get; } = new List<Genre>();
        public DateTime CreateTime { get; set; }
    }

    public class Score
    {
        public int ID { set; get; }
        public int FanficID { set; get; }
        public string UserName { set; get; }
        public int Mark { set; get; }
    }
    public class ChScore
    {
        public int ID { set; get; }
        public int ChapterID { set; get; }
        public string UserName { set; get; }
    }
    public class Tag
    {
        public int ID { set; get; }
        public string Name { set; get; }
    }
    public class Genre
    {
        public int ID { set; get; }
        public string Name { set; get; }
    }
    public class Chapter
    {
        public int ID { set; get; }
        public int FanficID { set; get; }
        public string Name { set; get; }
        public int Score { set; get; }
        public ICollection<ChScore> Scores { set; get; }
        public string Text { set; get; }
        public ICollection<Comment> Comments { set; get; } = new List<Comment>();
    }
    public class Comment
    {
        public int ID { set; get; }
        public int ChapterID { set; get; }
        public string Author { set; get; }
        public string Content { set; get; }
    }
}

