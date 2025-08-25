using System.ComponentModel.DataAnnotations;

namespace SimpleEventAccountingMobile.Database.DbModels
{
	
	public abstract class BaseEntity
	{
		[Key]
		public Guid Id { get; set; }
	}
}
