using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shared
{
	/// <summary>
	/// Command line is assumed to be of the form:
	///	[value [value ...]] [[/-]switch [value [value ...]]]
	/// A switch can have multiple values.
	/// Values can also be passed before switches.
	///
	/// Switches must start with a letter. (This avoids negative numbers
	/// being seen as switches instead of values.)
	/// Switches can be case-sensitive or -insensitive.
	///
	/// This class parses the passed command line and presents:
	///	Unnamed values
	///	Switches without values
	///	Switches with values
	///	Switches with multiple values
	///	Values can be numeric or text.
	/// </summary>
	/// <example>
	/// CommandLineArguments clargs = new(args);
	/// if (clargs.Switches["n"] is not null)
	/// {
	///	int x = clargs.Switches["n"].ValueAsInt();
	/// }
	/// </example>
	public class CommandLineArguments
	{
		private readonly bool CaseSensitive;

		private readonly CommandLineSwitch UnnamedValues = new("unnamed");
		public ReadOnlyCollection<string> Values => UnnamedValues.Values;
		public int ValueAsInt(int ix) => UnnamedValues.ValueAsInt(ix);
		public uint ValueAsUInt(int ix) => UnnamedValues.ValueAsUInt(ix);

		private readonly List<CommandLineSwitch> m_switches = new();
		public ReadOnlyCollection<CommandLineSwitch> Switches => m_switches.AsReadOnly();


		public CommandLineArguments(string[] args, bool caseSensitive)
		{
			CaseSensitive = caseSensitive;

			CommandLineSwitch? currentSwitch = null;
			foreach (var arg in args)
			{
				if (arg == "/")
				{
					continue;
				}

				// is this a switch or a value?
				if (((arg.First() == '/') || (arg.First() == '-')) && !Char.IsDigit(arg[1..2].First()))
				{
					/// Switch: If we already have it, add any subsequent value(s) to it.
					var normalizedArg = CaseSensitive ? arg[1..] : arg[1..].ToLower();
					currentSwitch = this[normalizedArg];
					if (currentSwitch is null)
					{
						currentSwitch = new CommandLineSwitch(normalizedArg);
						m_switches.Add(currentSwitch);
					}
				}
				else
				{
					/// Value: If we are not currently processing a switch, add it to the unnamed values.
					/// Else, add this to the list of values for the current switch.
					if (currentSwitch is null)
					{
						UnnamedValues.AddValue(arg);
					}
					else
					{
						currentSwitch.AddValue(arg);
					}
				}
			}
		}

		public CommandLineSwitch? this[string name]
		{
			get
			{
				string normalizedName = CaseSensitive ? name : name.ToLower();
				return Switches.SingleOrDefault(sw => sw.Name == normalizedName);
			}
		}
	}

	public class CommandLineSwitch
	{
		public string Name { get; private set; }

		private readonly List<string> m_values = new();
		public ReadOnlyCollection<string> Values => m_values.AsReadOnly();


		public CommandLineSwitch(string name)
		{
			Name = name;
		}

		public void AddValue(string value) => m_values.Add(value);

		public int ValueAsInt() => ValueAsInt(0);	// Default value for ease-of-use
		public int ValueAsInt(int ix)
		{
			return Int32.TryParse(Values[ix], out int value) ? value : 0;
		}

		public uint ValueAsUInt() => ValueAsUInt(0);	// Default value for ease-of-use
		public uint ValueAsUInt(int ix)
		{
			return UInt32.TryParse(Values[ix], out uint value) ? value : 0;
		}
	}
}
