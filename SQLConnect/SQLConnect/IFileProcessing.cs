namespace SQLConnect
{
	public interface IFileProcessing
	{
		byte[] compress(byte[] bytes, string type);
	}
}
