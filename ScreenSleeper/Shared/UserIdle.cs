using System;
using System.Runtime.InteropServices;

namespace Shared;

public static class UserIdle
{
	public static TimeSpan GetTimeSinceLastActivity()
	{
		LASTINPUTINFO lastInputInfo = LASTINPUTINFO.Create();
		if (!GetLastInputInfo(ref lastInputInfo))
		{
			return TimeSpan.Zero;
		}

		// Even though it says "ticks," it's really milliseconds.
		/// <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.environment.tickcount"/>
		uint msecEnvTicks = (uint)Environment.TickCount;
		uint lastInputTick = lastInputInfo.dwTime;
		uint idleTime = msecEnvTicks - lastInputTick;
		return TimeSpan.FromMilliseconds(idleTime);
	}


	/// <see cref="https://www.pinvoke.net/default.aspx/Structures/LASTINPUTINFO.html"/>
	[StructLayout(LayoutKind.Sequential)]
	private struct LASTINPUTINFO
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

		[MarshalAs(UnmanagedType.U4)]
		public UInt32 cbSize;
		[MarshalAs(UnmanagedType.U4)]
		public UInt32 dwTime;   // Even though it says "ticks," it's really milliseconds.

		public static LASTINPUTINFO Create() => new()
		{
			cbSize = (uint)SizeOf,
			dwTime = 0,
		};
	}

	/// <see cref="https://www.pinvoke.net/default.aspx/user32.GetLastInputInfo"/>
	/// <param name="plii"></param>
	/// <returns></returns>
	[DllImport("user32.dll")]
	private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
}
