using System.Runtime.InteropServices;

namespace Shared;

public static class ScreenState
{
	public static void SetScreenOn() => SetScreenState(State.On);
	public static void SetScreenStandby() => SetScreenState(State.StandBy);
	public static void SetScreenOff() => SetScreenState(State.Off);

	private static void SetScreenState(State state)
	{
		// https://docs.microsoft.com/en-us/windows/win32/menurc/wm-syscommand
		_ = SendMessage(0xFFFF, Messages.WM_SYSCOMMAND, Constants.SC_MONITORPOWER, (int)state);
	}


	private static class Messages
	{
		public const int WM_SYSCOMMAND = 0x0112;
	}
	private static class Constants
	{
		public const int SC_MONITORPOWER = 0xF170;
	}

	private enum State
	{
		On = -1,
		StandBy = 1,
		Off = 2,
	}

	[DllImport("user32.dll")]
	private static extern int SendMessage(int hWnd, uint msg, int wParam, int lParam);
}
