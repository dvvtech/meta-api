using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaApi.Core.Time
{
    public interface ISystemTime
    {
        DateTime UtcNow { get; }
    }

    public class SystemTime : ISystemTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
