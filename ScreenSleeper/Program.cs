using Shared;
using System;
using System.Threading.Tasks;

/*
	FEATURES

	When run from command line, delay before screen-off.
	Delay between standby and off.
	Wait for user-idle, delay, then standby/off.

	Command Line Arguments
		/? or /h
		-idle 10
		-delay 5
		-standby 10 [default 0 => no standby]
 */
namespace ScreenSleeper;

public class Program
{
	static void Main(string[] args)
	{
		var cmdline = new CommandLineArguments(args, caseSensitive: false);

		if ((cmdline["?"] is not null) || (cmdline["h"] is not null))
		{
			ConsoleWindow.CreateConsoleWindow();
			ApplicationInformation info = new();
			Console.WriteLine($"{info.Name} {info.Version}");
			Console.WriteLine(info.Company);
			Console.WriteLine(info.Copyright);
			Console.WriteLine();
			Console.WriteLine("Usage:");
			Console.WriteLine("/? Display this information");
			Console.WriteLine("/h Display this information");
			Console.WriteLine("/idle <minutes> Wait until idle for this long");
			Console.WriteLine("/delay <seconds> Wait before entering standby (if /standby specified) or off");
			Console.WriteLine("/standby <seconds> Wait before turning off monitors");
			Console.WriteLine("/lock Lock the computer after turning off monitors");
			Console.WriteLine();
			Console.WriteLine("Closing in 15 seconds...");
			System.Threading.Thread.Sleep(15_000);
			return;
		}

		bool bLock = (cmdline["lock"] is not null);
		uint minutesIdle = cmdline["idle"]?.ValueAsUInt() ?? 0;
		var delayIdle = TimeSpan.FromMinutes(minutesIdle);
		uint secondsDelay = cmdline["delay"]?.ValueAsUInt() ?? 1;
		if (secondsDelay == 0)
		{
			// Wait at least one second before turning off monitors, so user can stop using HIDs.
			secondsDelay = 1;
		}
		var delay = TimeSpan.FromSeconds(secondsDelay);
		uint secondsStandby = cmdline["standby"]?.ValueAsUInt() ?? 0;
		var delayStandby = TimeSpan.FromSeconds(secondsStandby);

		ProcessScreenStates(delayIdle, delay, delayStandby, bLock).Wait();
	}

	private static async Task ProcessScreenStates(TimeSpan delayIdle, TimeSpan delay, TimeSpan delayStandby, bool bLock)
	{
		/// If user idle time is specified, wait for it.
		await WaitForUserIdle(delayIdle);

		if (delay != TimeSpan.Zero)
		{
			await Task.Delay(delay);
		}

		/// If a standby period is specified, set to standby and wait.
		if (delayStandby != TimeSpan.Zero)
		{
			ScreenState.SetScreenStandby();
			await Task.Delay(delayStandby);
		}

		// Note that we must lock before turning off the monitor because locking wakes it up.
		if (bLock)
		{
			LockWindows.Lock();
		}

		/// Finally, power off.
		ScreenState.SetScreenOff();
	}

	private static async Task WaitForUserIdle(TimeSpan delayIdle)
	{
		if (delayIdle == TimeSpan.Zero)
		{
			return;
		}

		while (true)
		{
			var remainingTime = delayIdle.Subtract(UserIdle.GetTimeSinceLastActivity());
			if (remainingTime > TimeSpan.Zero)
			{
				await Task.Delay(remainingTime);
			}
			else
			{
				break;
			}
		}
	}
}
