using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using System.Diagnostics;
using System.Linq;

namespace ScreenSleeper.UnitTests;

[TestClass]
public class CommandLineArgumentsTest
{
	[TestMethod]
	public void TestSimpleValues()
	{
		CommandLineArguments cmdline = new(new string[] { "One", }, caseSensitive: true);
		Debug.Assert(1 == cmdline.Values.Count);
		Debug.Assert("One" == cmdline.Values.Skip(0).First());

		cmdline = new(new string[] { "One", "Two", }, caseSensitive: true);
		Debug.Assert(2 == cmdline.Values.Count);
		Debug.Assert("One" == cmdline.Values.Skip(0).First());
		Debug.Assert("Two" == cmdline.Values.Skip(1).First());

		cmdline = new(new string[] { "18", "-520", }, caseSensitive: true);
		Debug.Assert(2 == cmdline.Values.Count);
		Debug.Assert("18" == cmdline.Values.Skip(0).First());
		Debug.Assert(18u == cmdline.ValueAsUInt(0));
		Debug.Assert("-520" == cmdline.Values.Skip(1).First());
		Debug.Assert(-520 == cmdline.ValueAsInt(1));
	}

	[TestMethod]
	public void TestValues()
	{
		CommandLineArguments cmdline = new(new string[] { "7", "/x", "24", }, caseSensitive: false);
		Debug.Assert(1 == cmdline.Values.Count);
		Debug.Assert("7" == cmdline.Values.First());
		Debug.Assert(1 == cmdline.Switches.Count);
		Debug.Assert(1 == cmdline["x"]?.Values.Count);
		Debug.Assert("24" == cmdline["x"]?.Values.First());
		Debug.Assert(24 == cmdline["x"]?.ValueAsInt());
		Debug.Assert(24u == cmdline["x"]?.ValueAsUInt());

		cmdline = new(new string[] { "-7", "/x", "-24", }, caseSensitive: false);
		Debug.Assert(1 == cmdline.Values.Count);
		Debug.Assert("-7" == cmdline.Values.First());
		Debug.Assert(1 == cmdline.Switches.Count);
		Debug.Assert(1 == cmdline["x"]?.Values.Count);
		Debug.Assert("-24" == cmdline["x"]?.Values.First());
		Debug.Assert(-24 == cmdline["x"]?.ValueAsInt());
		Debug.Assert(0u == cmdline["x"]?.ValueAsUInt());
	}

	[TestMethod]
	public void TestSwitches()
	{
		CommandLineArguments cmdline = new(new string[] { "/?", }, caseSensitive: false);
		Debug.Assert(1 == cmdline.Switches.Count);
		Debug.Assert(null != cmdline["?"]);
		Debug.Assert(0 == cmdline["?"]?.Values.Count);

		cmdline = new(new string[] { "-idle", "5", "/?", }, caseSensitive: false);
		Debug.Assert(2 == cmdline.Switches.Count);
		Debug.Assert(null != cmdline["?"]);
		Debug.Assert(0 == cmdline["?"]?.Values.Count);
		Debug.Assert(null != cmdline["idle"]);
		Debug.Assert(1 == cmdline["idle"]?.Values.Count);
		Debug.Assert(5 == cmdline["idle"]?.ValueAsInt());
		Debug.Assert(5u == cmdline["idle"]?.ValueAsUInt());

		cmdline = new(new string[] { "/#", }, caseSensitive: false);
		Debug.Assert(1 == cmdline.Switches.Count);
		Debug.Assert(null != cmdline["#"]);
		Debug.Assert(0 == cmdline["#"]?.Values.Count);

		cmdline = new(new string[] { "/", }, caseSensitive: false);
		Debug.Assert(0 == cmdline.Switches.Count);
		Debug.Assert(null == cmdline["/"]);
	}

	[TestMethod]
	public void TestSimpleSwitches()
	{
		CommandLineArguments cmdline = new(new string[] { "/f", "1", "2", "3", "/h", "/f", "4", "5", }, caseSensitive: false);
		Debug.Assert(2 == cmdline.Switches.Count);
		Debug.Assert(5 == cmdline["f"]?.Values.Count);
		Debug.Assert(5 == cmdline["F"]?.Values.Count);
		Debug.Assert(0 == cmdline["h"]?.Values.Count);
		Debug.Assert(0 == cmdline["H"]?.Values.Count);

		cmdline = new(new string[] { "/f", "1", "2", "3", "/H", "/f", "4", "5", }, caseSensitive: false);
		Debug.Assert(2 == cmdline.Switches.Count);
		Debug.Assert(5 == cmdline["f"]?.Values.Count);
		Debug.Assert(5 == cmdline["F"]?.Values.Count);
		Debug.Assert(0 == cmdline["h"]?.Values.Count);
		Debug.Assert(0 == cmdline["H"]?.Values.Count);

		cmdline = new(new string[] { "/f", "1", "2", "3", "/h", "/F", "4", "5", }, caseSensitive: true);
		Debug.Assert(3 == cmdline.Switches.Count);
		Debug.Assert(3 == cmdline["f"]?.Values.Count);
		Debug.Assert(2 == cmdline["F"]?.Values.Count);
		Debug.Assert(0 == cmdline["h"]?.Values.Count);
		Debug.Assert(null == cmdline["H"]);

		cmdline = new(new string[] { "/F", "1", "2", "3", "/H", "/f", "4", "5", }, caseSensitive: true);
		Debug.Assert(3 == cmdline.Switches.Count);
		Debug.Assert(3 == cmdline["F"]?.Values.Count);
		Debug.Assert(2 == cmdline["f"]?.Values.Count);
		Debug.Assert(0 == cmdline["H"]?.Values.Count);
		Debug.Assert(null == cmdline["h"]);
	}

	[TestMethod]
	public void TestValuesAndSwitches()
	{
		CommandLineArguments cmdline = new(new string[] { "Unnamed", "/f", "1", "2", "3", "/h", "/f", "4", "5", }, caseSensitive: false);
		Debug.Assert(1 == cmdline.Values.Count);
		Debug.Assert("Unnamed" == cmdline.Values.First());
		Debug.Assert(2 == cmdline.Switches.Count);
		Debug.Assert(5 == cmdline["f"]?.Values.Count);
		Debug.Assert(5 == cmdline["F"]?.Values.Count);
		Debug.Assert(0 == cmdline["h"]?.Values.Count);
		Debug.Assert(0 == cmdline["H"]?.Values.Count);

		cmdline = new(new string[] { "Unnamed", "/f", "1", "2", "3", "/H", "/f", "4", "5", }, caseSensitive: false);
		Debug.Assert(1 == cmdline.Values.Count);
		Debug.Assert("Unnamed" == cmdline.Values.First());
		Debug.Assert(2 == cmdline.Switches.Count);
		Debug.Assert(5 == cmdline["f"]?.Values.Count);
		Debug.Assert(5 == cmdline["F"]?.Values.Count);
		Debug.Assert(0 == cmdline["h"]?.Values.Count);
		Debug.Assert(0 == cmdline["H"]?.Values.Count);

		cmdline = new(new string[] { "One", "Two", "/f", "1", "2", "3", "/h", "/F", "4", "5", }, caseSensitive: true);
		Debug.Assert(2 == cmdline.Values.Count);
		Debug.Assert("One" == cmdline.Values.Skip(0).First());
		Debug.Assert("Two" == cmdline.Values.Skip(1).First());
		Debug.Assert(3 == cmdline.Switches.Count);
		Debug.Assert(3 == cmdline["f"]?.Values.Count);
		Debug.Assert(2 == cmdline["F"]?.Values.Count);
		Debug.Assert(0 == cmdline["h"]?.Values.Count);
		Debug.Assert(null == cmdline["H"]);

		cmdline = new(new string[] { "One", "Two", "/F", "1", "2", "3", "/H", "/f", "4", "5", }, caseSensitive: true);
		Debug.Assert(2 == cmdline.Values.Count);
		Debug.Assert("One" == cmdline.Values.Skip(0).First());
		Debug.Assert("Two" == cmdline.Values.Skip(1).First());
		Debug.Assert(3 == cmdline.Switches.Count);
		Debug.Assert(3 == cmdline["F"]?.Values.Count);
		Debug.Assert(2 == cmdline["f"]?.Values.Count);
		Debug.Assert(0 == cmdline["H"]?.Values.Count);
		Debug.Assert(null == cmdline["h"]);
	}
}
