using System.Runtime.InteropServices;

namespace Shared
{
	public static class LockWindows
	{
		public static void Lock() => LockWorkStation();


		[DllImport("user32.dll")]
		private static extern bool LockWorkStation();
	}
}
