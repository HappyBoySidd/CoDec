namespace SuperCoDec_Contracts
{
	public interface ICoDec
	{
		public bool Compress(DirectoryInfo strInputDirectory, DirectoryInfo outputDirectory);
		public bool Decompress(FileInfo fileToDecompress, DirectoryInfo outputDirectory);
	}
}
