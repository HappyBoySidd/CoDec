using Microsoft.Extensions.Configuration;
using SuperCoDec_Application.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;
using SuperCoDec_Standard;
using SuperCoDec_Contracts;
using SuperCoDec_GZip;

namespace SuperCoDec_Application
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
            #region Read configuration values manually
            /*IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();
			Settings settings = config.GetRequiredSection("Settings").Get<Settings>();
			Console.WriteLine($"KeyOne = {settings.KeyOne}");
			Console.WriteLine($"KeyTwo = {settings.KeyTwo}");
			Console.WriteLine($"KeyThree:Message = {settings.KeyThree.Message}");*/
            #endregion

            #region Read configuration values using IHost
            using IHost host = Host.CreateDefaultBuilder(args).Build();
			IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
            /*string versionOne = config["Settings:KeyTwo"];
			Console.WriteLine($"KeyOne = {config["Settings:KeyOne"]}");
			Console.WriteLine($"KeyTwo = {config["Settings:KeyTwo"]}");
			Console.WriteLine($"KeyThree:Message = {config["Settings:KeyThree:Message"]}"); 
            await host.RunAsync();*/
            #endregion

            #region Extracting values from Configuration Settings
            string strStartPath = config["Settings:startPath"];
            string strZipPath = config["Settings:zipPath"];
            string strExtractPath = config["Settings:extractPath"];
            Modes mode = (Modes)Enum.Parse(typeof(Modes), config["Settings:Mode"]);
			#endregion

			ICoDec objCodec;
			switch (mode)
			{
				case Modes.GZip:
					objCodec = new GZipCoDec();
					strZipPath += ".tgz";
					objCodec.Compress(new DirectoryInfo(strStartPath), new DirectoryInfo(strZipPath));
					foreach (FileInfo fileToDecompress in new DirectoryInfo(strStartPath).Parent.GetFiles("*.tgz"))
					{
						objCodec.Decompress(new FileInfo(strZipPath), new DirectoryInfo(strExtractPath));
					}
					break;
				case Modes.Standard:
				default:
					objCodec = new StandardCoDec();
					strZipPath += ".zip";
					objCodec.Compress(new DirectoryInfo(strStartPath), new DirectoryInfo(strZipPath));
					objCodec.Decompress(new FileInfo(strZipPath), new DirectoryInfo(strExtractPath));
					break;
			}

			await host.RunAsync();
			Console.Read();
		}

        public enum Modes
		{
            Standard,
            GZip
		}
    }
}