using System.ComponentModel.DataAnnotations;

namespace Fanfiction.ViewModels
{
    public class AddChapterModel
    {
        [Required(ErrorMessage = "Не указано название главы")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Длина названия должна быть до 20 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указан текст главы")]
        public string Text { get; set; }
    }
}
