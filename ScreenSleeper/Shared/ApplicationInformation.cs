using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;


namespace Shared
{
	/// <summary>
	/// For additional assemblies, derive a class and add more FileVersionInfo members.
	/// </summary>
	/// <example>
	/// public FileVersionInfo MyLibraryExeInfo { get; private set; }
	/// 
	/// string pathLibraryDLL = System.IO.Path.Combine(pathExe, "MyLibrary.dll");
	/// LibraryDLLInfo = FileVersionInfo.GetVersionInfo(pathCoreDLL);
	/// </example>
	public class ApplicationInformation
	{
		public string Name { get; private set; }
		public string Company { get; private set; }
		public string Copyright { get; private set; }
		public string Version { get; private set; }

		public string FileTitle { get; private set; }
		public string FileVersion{ get; private set; }

		public string WebSiteURL { get; private set; } = @"http://12noon.com";


		/// <summary>
		/// Provides information about the version of the EXECUTING assembly.
		/// </summary>
		/// <remarks>
		/// If this is a DLL, this does NOT return information about THIS assembly.
		/// </remarks>
		public ApplicationInformation()
		{
			FileVersionInfo AppExeInfo = GetFileVersionInfo();

			//AssemblyName asmName = asm.GetName();

			//AssemblyTitleAttribute attrTitle = (AssemblyTitleAttribute)System.Attribute.GetCustomAttribute(asm, typeof(AssemblyTitleAttribute));
			//string Title = attrTitle.Title;

			//AssemblyCompanyAttribute attrCompany = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyCompanyAttribute));
			//Company = attrCompany.Company;

			Name = AppExeInfo.ProductName ?? string.Empty;
			Company = AppExeInfo.CompanyName ?? string.Empty;
			Copyright = AppExeInfo.LegalCopyright ?? string.Empty;
			Version = AppExeInfo.ProductVersion ?? string.Empty;

			FileTitle = AppExeInfo.FileDescription ?? string.Empty;
			FileVersion = AppExeInfo.FileVersion ?? string.Empty;
		}


		private static FileVersionInfo GetFileVersionInfo()
		{
			Assembly asm = Assembly.GetEntryAssembly() ?? throw new FileNotFoundException("Unable to get assembly.");
			var exeName = asm.GetName().Name;
			string pathExe = Path.Combine(GetAssemblyPath(), $"{exeName}.exe");

			// Get information about this assembly (not necessarily the executing assembly).
			return FileVersionInfo.GetVersionInfo(pathExe);
		}


		/// <summary>
		/// Return the path to the main (entry) assembly (.exe)
		/// (not including the filename of the executable).
		/// </summary>
		/// <example>
		/// C:\Path\To\Executable
		/// </example>
		/// <see cref="Assembly.GetEntryAssembly" />
		/// <seealso cref="Assembly.GetExecutingAssembly" />
		/// <returns>Path to the main (entry) assembly (.exe)</returns>
		public static string GetAssemblyPath()
		{
			/// assembly.Location is empty for single-file package, so we use this.
			//Assembly asm = Assembly.GetEntryAssembly() ?? throw new FileNotFoundException("Unable to get assembly.");
			//return Path.GetDirectoryName(asm.Location) ?? string.Empty;
			return AppContext.BaseDirectory;
		}
	}
}
