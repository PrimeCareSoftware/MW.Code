namespace MedicSoft.CrossCutting.Identity
{
    public interface ITenantContext
    {
        string TenantId { get; }
        string? UserId { get; }
        void SetTenant(string tenantId);
        void SetUser(string userId);
    }

    public class TenantContext : ITenantContext
    {
        private string _tenantId = string.Empty;
        private string? _userId;

        public string TenantId => _tenantId;
        public string? UserId => _userId;

        public void SetTenant(string tenantId)
        {
            _tenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }

        public void SetUser(string userId)
        {
            _userId = userId;
        }
    }
}