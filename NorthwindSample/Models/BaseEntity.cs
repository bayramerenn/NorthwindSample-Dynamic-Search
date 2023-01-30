using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindSample.Models
{
    public abstract class BaseEntity
    {
        private DateTime dateTime;

        [NotMapped]
        public DateTime CreatedTime
        { get { this.dateTime = DateTime.Now; return dateTime; } }
    }
}