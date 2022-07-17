using ICSharpCode.SharpZipLib.Tar;
using SuperCoDec_Contracts;
using System.IO.Compression;

namespace SuperCoDec_GZip
{
	public class GZipCoDec : ICoDec
	{
		//public bool Compress(DirectoryInfo inputDirectory, DirectoryInfo outputDirectory)
		//{
		//	try
		//	{
		//		#region Assessing Input
		//		var strInputPath = inputDirectory.FullName;
		//		var strOutputPath = outputDirectory.FullName;
		//		#endregion

		//		#region Validation
		//		if (string.IsNullOrWhiteSpace(strInputPath) || string.IsNullOrWhiteSpace(strOutputPath))
		//			throw new ArgumentException("Unable to retrive directory details correctly");
		//		#endregion

		//		#region Action
		//		foreach (FileInfo fileToCompress in inputDirectory.GetFiles())
		//		{
		//			using (FileStream originalFileStream = fileToCompress.OpenRead())
		//			{
		//				if ((File.GetAttributes(fileToCompress.FullName) &
		//				   FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
		//				{
		//					using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
		//					{
		//						using (GZipStream compressionStream = new GZipStream(compressedFileStream,
		//						   CompressionMode.Compress))
		//						{
		//							originalFileStream.CopyTo(compressionStream);
		//						}
		//					}
		//					FileInfo info = new FileInfo(inputDirectory.FullName + Path.DirectorySeparatorChar + fileToCompress.Name + ".gz");
		//					Console.WriteLine($"Compressed {fileToCompress.Name} from {fileToCompress.Length} to {info.Length} bytes.");
		//				}
		//			}
		//		}
		//		#endregion

		//		#region Return
		//		return true;
		//		#endregion
		//	}
		//	catch (Exception)
		//	{
		//		return false;
		//	}
		//}

		public bool Decompress(FileInfo fileToDecompress, DirectoryInfo outputDirectory)
		{
			try
			{
				#region Assessing Input
				var strZipPath = fileToDecompress.DirectoryName;
				var strExtractPath = outputDirectory.FullName;
				#endregion

				#region Validation
				if (string.IsNullOrWhiteSpace(strZipPath))
					throw new ArgumentException("Unable to retrive directory details of the file to be decompressed");
				#endregion

				#region Action
				using (FileStream originalFileStream = fileToDecompress.OpenRead())
				{
					string currentFileName = fileToDecompress.FullName;
					string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

					using (FileStream decompressedFileStream = File.Create(newFileName))
					{
						using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
						{
							decompressionStream.CopyTo(decompressedFileStream);
							Console.WriteLine($"Decompressed: {fileToDecompress.Name}");
						}
					}
				}
				#endregion

				#region Return
				return true;
				#endregion
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static string CreateTar(string strDirectoryToCompress, string strDestinationPath, string strTarFile)
		{
			string destDrive = strDestinationPath.Substring(0, strDestinationPath.IndexOf(@"\") + 1);
			Directory.SetCurrentDirectory(destDrive);
			string tarFilePath = Path.Combine(strDestinationPath, strTarFile);
			using (Stream fs = new FileStream(tarFilePath, FileMode.OpenOrCreate))
			{
				using (TarArchive ta = TarArchive.CreateOutputTarArchive(fs))
				{
					string[] files = Directory.GetFiles(strDirectoryToCompress);
					foreach (string file in files)
					{
						string entry = file.Substring(file.IndexOf(@"\") + 1);
						TarEntry te = TarEntry.CreateEntryFromFile(entry);
						ta.WriteEntry(te, false);
					}
				}
			}
			return tarFilePath;
		}

		public bool Compress(DirectoryInfo inputDirectory, DirectoryInfo outputDirectory)
		{
			try
			{
				#region Assessing Input
				var strInputPath = inputDirectory.FullName;
				var strOutputPath = outputDirectory.Parent?.FullName;
				#endregion

				#region Validation
				if (string.IsNullOrWhiteSpace(strInputPath) || string.IsNullOrWhiteSpace(strOutputPath))
					throw new ArgumentException("Unable to retrive directory details correctly");
				#endregion

				#region Action
				string file = CreateTar(strInputPath, strOutputPath, "Compressed.tar");
				string outputFile = outputDirectory.FullName;
				using (FileStream fs = new FileStream(outputFile, FileMode.CreateNew))
				using (GZipStream s = new GZipStream(fs, CompressionMode.Compress))
				{
					using (FileStream inputfs = new FileStream(file, FileMode.Open))
					{
						byte[] buffer = new byte[4096];
						int len = 0;
						while ((len = inputfs.Read(buffer, 0, buffer.Length)) > 0)
						{
							s.Write(buffer, 0, len);
						}
					}
				}
				#endregion

				#region Return
				return true;
				#endregion
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}