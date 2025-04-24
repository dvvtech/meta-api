using MetaApi.Core.Interfaces.Infrastructure;

namespace Meta.Infrastructure
{
    public class SystemTime : ISystemTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
