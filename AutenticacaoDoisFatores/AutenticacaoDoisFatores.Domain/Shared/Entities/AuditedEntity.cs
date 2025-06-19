namespace AutenticacaoDoisFatores.Domain.Shared.Entities
{
    public abstract class AuditedEntity : BaseEntity
    {
        private readonly bool _isRegistered = false;
        private readonly Queue<AuditedUpdate> _updates = [];

        public DateTime CreatedAt { get; protected set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; protected set; }

        private AuditedEntity()
        {
            _isRegistered = false;
        }

        protected AuditedEntity(bool isRegistered = false)
        {
            _isRegistered = isRegistered;
        }

        public void RegisterUpdate(string field, string previousValue, string currentValue)
        {
            if (!_isRegistered)
                return;

            var fieldAlreadyRegistered = _updates.FirstOrDefault(a => a.Field.ToLower().Equals(field.ToLower()));
            if (fieldAlreadyRegistered is not null)
                fieldAlreadyRegistered.CurrentValue = currentValue;
            else
                _updates.Enqueue(new AuditedUpdate(field, previousValue, currentValue));

            UpdatedAt = DateTime.Now;
        }

        public AuditedUpdate[] GetUpdates()
        {
            return [.. _updates];
        }
    }

    public sealed class AuditedUpdate(string field, string previousValue, string currentValue)
    {
        public string Field { get; } = field;
        public string PreviousValue { get; } = previousValue;
        public string CurrentValue { get; set; } = currentValue;
    }
}
