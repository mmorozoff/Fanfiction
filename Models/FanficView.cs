
using System.Linq;

namespace Fanfiction.Models
{
    public class FanficView
    {
        public IQueryable<Fanfic> Fanfics { set; get; }
        public string Text { set; get; }
    }
}
