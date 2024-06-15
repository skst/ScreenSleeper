using System;
using System.Threading;

namespace Shared
{
    public static class Application
    {
        private static readonly bool _isFirst;

        public static bool IsFirst => _isFirst;

        static Application()
        {
            _ = new Mutex(true, name: "{04CF3278-CC61-4716-8B67-955C0FD7C469}", createdNew: out _isFirst);
        }
    }
}
