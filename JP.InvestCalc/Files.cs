using System;
using System.IO;
using System.Threading.Tasks;

namespace JP.Utils
{
	static class Files
	{
		public static async Task Copy(Stream origin, Stream destination, ushort bufferSize = 32*1024)
		{
			if(bufferSize <= 0) throw new ArgumentOutOfRangeException(nameof(bufferSize));

			var buffer = new byte[bufferSize];
			int stride;
			while(0 < (stride = await origin.ReadAsync(buffer, 0, buffer.Length)))
				await destination.WriteAsync(buffer, 0, stride);
		}

		public static void DeleteWithBackup(string filePath)
		{
			string backupPath = filePath + ".backup";
			File.Delete(backupPath);
			File.Move(filePath, backupPath);
		}

		/// <exception cref="FileNotFoundException" />
		public static void RestoreBackup(string filePath)
		{
			string backupPath = filePath + ".backup";
			File.Move(backupPath, filePath);
		}

		public static async Task<IOException?> TryCopy(Stream origin, string destinationFilePath)
		{
			bool replacing = File.Exists(destinationFilePath);
			if(replacing)
				DeleteWithBackup(destinationFilePath);

			try
			{
				using(var streamTo = new FileStream(destinationFilePath, FileMode.CreateNew))
					await Copy(origin, streamTo);
			}
			catch(IOException err)
			{
				File.Delete(destinationFilePath); // in case it was created but not completed
				RestoreBackup(destinationFilePath);
				return err;
			}
			return null;
		}
	}
}
