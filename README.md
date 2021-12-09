# ScreenSleeper [![.NET](https://github.com/skst/ScreenSleeper/actions/workflows/dotnet.yml/badge.svg)](https://github.com/skst/ScreenSleeper/actions/workflows/dotnet.yml)

Turn off screen after user idle or delay.

[12noon.com](https://12noon.com)

## Features

Windows sometimes forgets to turn off your screens according
to your *Power & sleep* settings.
This application ensures your monitors are turned off on demand,
after user idle, or after a simple delay.

When you run this application, you can specify the delay before
it turns off the monitors. You can also specify that it sets
the monitors to standby first and waits an additional period of time
before turning off the monitors.

The application can also lock the computer when it turns off the monitors.

## Steps

These steps are how the application uses the values specified on the command line.

1. If `/idle` value is greater than zero, wait until computer has been idle this long.
2. If `/delay` value is greater than zero, wait this long.
3. If `/standby` value is greater than zero, turn monitor(s) to standby and wait this long.
4. If `/lock` is specified, lock Windows.
5. Turn off monitor(s).

The application always delays at least one second before turning off the monitors
so that the user has time to stop using the keyboard and mouse.
(Otherwise, they might unintentionally wake up the computer.)

## Tasks

### Turn off monitors immediately

`ScreenSleeper.exe`

### Turn off monitors in five seconds

`ScreenSleeper.exe /delay 5`

### Turn off monitors and lock Windows in five seconds

`ScreenSleeper.exe /delay 5 /lock`

### Turn off monitors and lock Windows when user has been idle for ten minutes

`ScreenSleeper.exe /idle 10 /lock`

### When user has been idle for ten minutes, wait three seconds and then turn off monitors and lock Windows.

`ScreenSleeper.exe /idle 10 /delay 3 /lock`

### Put monitors in standby in 5 seconds. Wait ten seconds and then turn them off.

`ScreenSleeper.exe /delay 5 /standby 10`


## Command Line

Key            | Action
:------------- | :-----
/? | Display this information
/h | Display this information
/idle \<minutes\> | Wait until the computer has been idle for this long
/delay \<seconds\> | Wait before entering standby (if /standby is specified) or off (Default = 1)
/standby \<seconds\> | Wait in standby before turning off monitors
/lock | Lock the computer after turning off monitors
