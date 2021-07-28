using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared
{
	/// <summary>
	/// This class adds a console window to the process.
	/// This is primarily useful for a GUI application.
	/// </summary>
	public static class ConsoleWindow
	{
		public static void ShowConsoleWindow() => ShowWindow(GetConsoleWindow(), (int)ShowWindowState.SW_SHOW);
		public static void HideConsoleWindow() => ShowWindow(GetConsoleWindow(), (int)ShowWindowState.SW_HIDE);

		/// <summary>
		/// This method creates a console window and redirects standard-output to it.
		/// </summary>
		/// <remarks>
		/// This will not output to the console window if Visual Studio is attached to the process.
		/// </remarks>
		public static void CreateConsoleWindow()
		{
			// Only one console per process.
			if (AllocConsole() == 0)
			{
				return;
			}

			IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
			if ((stdHandle == INVALID_HANDLE_VALUE) || (stdHandle == IntPtr.Zero))
			{
				return;
			}

			SafeFileHandle safeFileHandle = new(stdHandle, true);
			FileStream fileStream = new(safeFileHandle, FileAccess.Write);
			StreamWriter standardOutput = new(fileStream);
			standardOutput.AutoFlush = true;

			Console.SetOut(standardOutput);

			// Now, Console.WriteLine() will output to the new console window (unless the Visual Studio debugger is attached).
		}


		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		private enum ShowWindowState
		{
			SW_HIDE = 0,
			SW_SHOWMINIMIZED = 2,
			SW_SHOW = 5,
		}


		[DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		private static extern int AllocConsole();

		[DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		private const int STD_OUTPUT_HANDLE = -11;
		private static readonly IntPtr INVALID_HANDLE_VALUE = IntPtr.Subtract(IntPtr.Zero, 1);
	}
}
