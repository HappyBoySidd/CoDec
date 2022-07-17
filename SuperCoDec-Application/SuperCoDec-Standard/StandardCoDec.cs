using SuperCoDec_Contracts;
using System.IO.Compression;

namespace SuperCoDec_Standard
{
	public class StandardCoDec : ICoDec
	{
		public bool Compress(DirectoryInfo inputDirectory, DirectoryInfo outputDirectory)
		{
			try
			{
				#region Assessing Input
				var strInputPath = inputDirectory.FullName;
				var strOutputPath = outputDirectory.FullName;
				#endregion

				#region Validation
				if (string.IsNullOrWhiteSpace(strInputPath) || string.IsNullOrWhiteSpace(strOutputPath))
					throw new ArgumentException("Unable to retrive directory details correctly");
				#endregion

				#region Action
				ZipFile.CreateFromDirectory(strInputPath, strOutputPath);
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

		public bool Decompress(FileInfo fileToDecompress, DirectoryInfo outputDirectory)
		{
			try
			{
				#region Assessing Input
				var strZipPath = fileToDecompress.FullName;
				var strExtractPath = outputDirectory.FullName;
				#endregion

				#region Validation
				if (string.IsNullOrWhiteSpace(strZipPath))
					throw new ArgumentException("Unable to retrive directory details of the file to be decompressed");
				#endregion

				#region Action
				ZipFile.ExtractToDirectory(strZipPath, strExtractPath);
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
