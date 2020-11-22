using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fanfiction.ViewModels
{
    public class AddFanficModel
    {
        [Required(ErrorMessage = "Не указано название фанфика")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Длина названия должна быть до 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указано описание фанфика")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Длина описания должна быть до 500 символов")]
        public string Description { get; set; }
        [NotMapped]
        public ICollection<string> T { set; get; }
        [NotMapped]
        public ICollection<string> G { set; get; }
        [NotMapped]
        public ICollection<string> A { set; get; }

    }
}
