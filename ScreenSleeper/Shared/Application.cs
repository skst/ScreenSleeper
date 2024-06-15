using System;
using System.Threading;

namespace Shared
{
    public static class Application
    {
        private static readonly Mutex _mutex = new(true, name: "{04CF3278-CC61-4716-8B67-955C0FD7C469}");

        public static bool IsRunning =>
            !_mutex.WaitOne(TimeSpan.Zero, true);
    }
}
