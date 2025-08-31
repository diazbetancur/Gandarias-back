using CC.Domain.Enums;

namespace CC.Domain.Entities
{
    public class Signing : EntityBase<Guid>
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public SigningType TipoFichaje { get; set; }
        public string Observaciones { get; set; }
        public Guid? LastUpdateUserId { get; set; }

        public virtual User LastUpdateUser
        {
            get; set;
        }

        public DateTime? UpdatedAt { get; set; }
    }
}