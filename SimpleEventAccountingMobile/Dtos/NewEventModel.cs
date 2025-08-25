using System.ComponentModel.DataAnnotations;

namespace SimpleEventAccountingMobile.Dtos
{
    // Модель для формы создания события
    public class NewEventModel
    {
        [Required(ErrorMessage = "Название события обязательно.")]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Дата события обязательна.")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Стоимость события обязательна.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Стоимость должна быть положительной.")]
        public decimal Price { get; set; }
    }
}
