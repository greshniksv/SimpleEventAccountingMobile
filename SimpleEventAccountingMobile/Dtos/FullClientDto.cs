using System.ComponentModel.DataAnnotations;

namespace SimpleEventAccountingMobile.Dtos
{
    public class FullClientDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
        public string Name { get; set; } = string.Empty;

        [Range(typeof(DateTime), "1900-01-01", "2099-12-31", ErrorMessage = "Дата рождения должна быть в диапазоне от 1900 до 2099")]
        public DateTime Birthday { get; set; } = DateTime.Today;

        public string Comment { get; set; } = string.Empty;
        public bool Subscription { get; set; }
        public int TrainingCount { get; set; } = 0;
        public int TrainingSkip { get; set; } = 0;
        public int TrainingFree { get; set; } = 0;
        public int CashAmount { get; set; } = 0;
    }
}
