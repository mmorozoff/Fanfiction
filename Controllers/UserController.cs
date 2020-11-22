using System.Linq;
using System;
using Fanfiction.Data;
using Microsoft.AspNetCore.Mvc;
using Fanfiction.Models;
using Fanfiction.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Fanfiction.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext db;
        public UserController(ApplicationDbContext context)
        {
            db = context;
        }

        [Authorize(Policy = "Correct")]
        public async Task<IActionResult> MyFanfics(SortState sortOrder = SortState.TimeAsc)
        {
            if (User.IsInRole("blocked"))
            {
                return RedirectToAction("Account", "Identity", new { id = "Lockout" });
            }
            IQueryable<Fanfic> fanfics = db.Fanfics;

            ViewData["PopSort"] = sortOrder == SortState.ScoreAsc ? SortState.ScoreDesc : SortState.ScoreAsc;
            ViewData["TimeSort"] = sortOrder == SortState.TimeAsc ? SortState.TimeDesc : SortState.TimeAsc;
            ViewData["SizeSort"] = sortOrder == SortState.SizeAsc ? SortState.SizeDesc : SortState.SizeAsc;

            fanfics = sortOrder switch
            {
                SortState.ScoreDesc => fanfics.OrderByDescending(s => s.Score),
                SortState.ScoreAsc => fanfics.OrderBy(s => s.Score),
                SortState.TimeDesc => fanfics.OrderByDescending(s => s.CreateTime),
                SortState.TimeAsc => fanfics.OrderBy(s => s.CreateTime),
                SortState.SizeDesc => fanfics.OrderByDescending(s => s.Size),
                SortState.SizeAsc => fanfics.OrderBy(s => s.Size),
            };
            return View(await fanfics.AsNoTracking().Include(t => t.Tags).Include(g => g.Genres).ToListAsync());
        }
        public async Task<IActionResult> ShowContent(int id)
        {
            if (User.IsInRole("blocked"))
            {
                return RedirectToAction("Account", "Identity", new { id = "Lockout" });
            }
            return View(await db.Fanfics.Include(u => u.Chapters).Include(t => t.Tags).Include(g => g.Genres).FirstOrDefaultAsync(f => f.ID == id));
        }
        public async Task<IActionResult> ShowChapter(int id, int fid)
        {
            if (User.IsInRole("blocked"))
            {
                return RedirectToAction("Account", "Identity", new { id = "Lockout" });
            }
            Fanfic ff = await db.Fanfics.Include(t => t.Tags).Include(g => g.Genres).Include(u => u.Chapters).ThenInclude(c => c.Comments).FirstOrDefaultAsync(f => f.ID == fid);
            for (int i = 0; i < ff.Chapters.Count(); i++)
            {
                if (ff.Chapters.ElementAt(i).ID == id)
                {
                    Show n = new Show {Comments = ff.Chapters.ElementAt(i).Comments, ChName = ff.Chapters.ElementAt(i).Name, ChScore = ff.Chapters.ElementAt(i).Score, Text = ff.Chapters.ElementAt(i).Text, CommentText = "", FID = ff.ID, Prev = -1, Next = -1, Score = ff.Score, ID = id, AuthorName = ff.AuthorName, Description = ff.Description, Genres = ff.Genres, Name = ff.Name, Size = ff.Size, Tags = ff.Tags };
                    if (i != 0)
                        n.Prev = ff.Chapters.ElementAt(i - 1).ID;
                    if (i < ff.Chapters.Count() - 1)
                        n.Next = ff.Chapters.ElementAt(i + 1).ID;
                    return View(n);
                }
            }
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "Correct")]
        public IActionResult AddFanfic()
        {
            List<string> G = new List<string>() { "Романтика", "Ангст", "Драма", "AU", "Повседневность", "Юмор", "Психология", "Стихи", "Фантастика", "Ужасы", "Фэнтэзи",
            "Мистика", "Дарк", "Фантастика","Детектив", "Соулмэйты", "Приключения", "Постапокалиптика","Триллер","Выживание","Трагедия","Реализм", "Вестерн","Флафф","Экшн", "PWP"};

            List<string> T = new List<string> { "POV", "Нецензурная лексика", "Дружба", "Смерть основных персонажей", "Смерть второстепенных персонажей", "Любовь/Ненависть",
            "Насилие", "Философия", "ER", "Учебные заведения", "Underage", "UST", "Элементы гета", "Элементы фемслэша", "Элменты слэша", "Счастливый финал", "Несчастливый финал","Открытый финал","Алкоголь",
            "Кинки/фетиши","Соврменность","Жестокость","Беременность","Исторические эпохи","Подростки","Ксенофилия","Нездоровые отношения","Убийства","Психические расстройства","Полиамория",
            "Влюбленность","Преступный мир", "Невзаимные чувства","Развитие отношений","Магия","Кроссовер","Отношения втайне","Аристократия","Воспоминая","Вампиры","Семьи","Преканон","Постканон"};


            List<string> A = new List<string>();

            var U = db.Users.ToList();
            foreach (var item in U)
            {
                A.Add(item.UserName);
            }
            AddFanficModel afm = new AddFanficModel { T = T, G = G, A = A };
            return View(afm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Correct")]
        public async Task<IActionResult> AddFanfic(AddFanficModel model, List<string> genres, List<string> tags, string author = "-1")
        {

            if (ModelState.IsValid)
            {
                Fanfic f = new Fanfic { CreateTime = DateTime.Now, Name = model.Name, AuthorName = User.Identity.Name, Description = model.Description, Size = 0, Score = 0 };
                if (author != "-1")
                    f.AuthorName = author;
                for (int i = 0; i < genres.Count(); i++)
                    f.Genres.Add(new Genre { Name = genres[i] });
                for (int i = 0; i < tags.Count(); i++)
                    f.Tags.Add(new Tag { Name = tags[i] });
                db.Fanfics.Add(f);
                await db.SaveChangesAsync();
                return RedirectToAction("MyFanfics", "User");
            }
            return View(model);
        }
        [HttpGet]
        [Authorize(Policy = "Correct")]
        public IActionResult AddChapter()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddChapter(AddChapterModel model, int id)
        {
            if (ModelState.IsValid)
            {
                Fanfic ff = await db.Fanfics.Include(ch => ch.Chapters).FirstOrDefaultAsync(f => f.ID == id);
                Chapter n = new Chapter { Text = model.Text, Name = model.Name, FanficID = id, Score = 0 };
                ff.Chapters.Add(n);
                ff.Size++;
                ff.CreateTime = DateTime.Now;
                db.Chapters.Update(n);
                await db.SaveChangesAsync();
                return RedirectToAction("ShowContent", "User", new { id });
            }
            return View(model);
        }
        [Authorize(Policy = "Correct")]
        public async Task<IActionResult> DeleteFanfic(int id)
        {
            Fanfic ff = await db.Fanfics.Include(t => t.Tags).Include(g => g.Genres).Include(c => c.Chapters).FirstOrDefaultAsync(f => f.ID == id);
            db.Fanfics.Remove(ff);
            db.SaveChanges();
            return RedirectToAction("MyFanfics", "User");
        }
        [Authorize(Policy = "Correct")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            Chapter ch = await db.Chapters.FirstOrDefaultAsync(c => c.ID == id);
            Fanfic ff = await db.Fanfics.FirstOrDefaultAsync(f => f.ID == ch.FanficID);
            ff.Size--;
            db.Chapters.Remove(ch);
            db.SaveChanges();
            return RedirectToAction("ShowContent", "User", new { id = ff.ID });
        }
        [Authorize(Policy = "Correct")]
        public IActionResult EditChapter(string n, string t, int id)
        {
            Show ch = new Show { ChName = n, Text = t, ID = id };
            return View(ch);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChapter(Show model, int id)
        {
            if (ModelState.IsValid)
            {
                Chapter ch = await db.Chapters.FirstOrDefaultAsync(c => c.ID == id);
                ch.Name = model.ChName;
                ch.Text = model.Text;
                db.Chapters.Update(ch);
                await db.SaveChangesAsync();
                return RedirectToAction("ShowContent", "User", new { id = ch.FanficID });
            }
            return View(model);
        }
        [Authorize(Policy = "Correct")]
        public async Task<IActionResult> AddComment(Show model, int id)
        {
            if (ModelState.IsValid)
            {
                Chapter ch = await db.Chapters.FirstOrDefaultAsync(c => c.ID == id);
                Comment n = new Comment { Content = model.CommentText, Author = User.Identity.Name, ChapterID = id };
                ch.Comments.Add(n);
                db.Chapters.Update(ch);
                await db.SaveChangesAsync();
                return RedirectToAction("ShowChapter", "User", new { id = id, fid = ch.FanficID });
            }
            return View(model);
        }

        public IActionResult SearchGenre(string name)
        {
            List<Fanfic> FF = new List<Fanfic>();
            foreach (var item in db.Fanfics.Include(g => g.Genres).Include(t => t.Tags))
            {
                if (item.Genres.FirstOrDefault(a => a.Name == name) != null)
                {
                    FF.Add(item);
                }
            }
            return View(FF);
        }
        public IActionResult SearchTag(string name)
        {
            List<Fanfic> FF = new List<Fanfic>();
            foreach (var item in db.Fanfics.Include(g => g.Genres).Include(t => t.Tags))
            {
                if (item.Tags.FirstOrDefault(a => a.Name == name) != null)
                {
                    FF.Add(item);
                }
            }
            return View(FF);
        }
        [Authorize(Policy = "Correct")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            Comment c = await db.Comments.FirstOrDefaultAsync(c => c.ID == id);
            Chapter ch = await db.Chapters.FirstOrDefaultAsync(f => f.ID == c.ChapterID);
            db.Comments.Remove(c);
            db.SaveChanges();
            return RedirectToAction("ShowChapter", "User", new { id = c.ChapterID, fid =  ch.FanficID});
        }
        [Authorize(Policy = "Correct")]
        public async Task<IActionResult> MarkFanfic(int rating, int id)
        {
            Fanfic ff = await db.Fanfics.Include(s => s.Scores).FirstOrDefaultAsync(f => f.ID == id);
            Score sc = (from x in ff.Scores where x.UserName == User.Identity.Name select x).FirstOrDefault();
            if (sc != null)
            {
                if (ff.Scores.Count() == 1)
                    ff.Score = 0;
                else
                {
                    ff.Score = Math.Round((ff.Score * ff.Scores.Count() - sc.Mark) / (ff.Scores.Count() - 1), 2);
                }
                db.Scores.Remove(sc);
                db.SaveChanges();
            }

            if (ff.Scores.Count() == 0)
                ff.Score += rating;
            else
            {
                ff.Score = Math.Round((ff.Score * ff.Scores.Count() + rating) / (ff.Scores.Count() + 1), 2);
            }
            ff.Scores.Add(new Score { Mark = rating, UserName = User.Identity.Name, FanficID = ff.ID });

            db.Fanfics.Update(ff);
            await db.SaveChangesAsync();
            return RedirectToAction("ShowContent", "User", new { id = ff.ID });
        }
        [Authorize(Policy = "Correct")]
        public async Task<IActionResult> LikeChapter(int id)
        {
            Chapter ch = await db.Chapters.Include(s => s.Scores).FirstOrDefaultAsync(f => f.ID == id);
            ChScore sc = (from x in ch.Scores where x.UserName == User.Identity.Name select x).FirstOrDefault();
            if (sc != null)
            {
                ch.Score--;
                db.ChScores.Remove(sc);
                db.SaveChanges();
            }
            else
            {
                ch.Score++;
                ch.Scores.Add(new ChScore {UserName = User.Identity.Name, ChapterID = ch.ID });
            }
            db.Chapters.Update(ch);
            await db.SaveChangesAsync();
            return RedirectToAction("ShowChapter", "User", new { id = id, fid = ch.FanficID });
        }

    }
}
