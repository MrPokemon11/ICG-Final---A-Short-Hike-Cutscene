using System;

public class FileSystem
{
	public interface IFileSystem
	{
		bool canSave { get; }

		void Initalize();

		bool SaveObjectAsync(string filename, object obj, int maxBytes, Action onFinish);

		T LoadObject<T>(string filename);

		void LoadObjectAsync<T>(string filename, Action<T> onFinish);

		bool Exists(string fileName);

		DateTime LastModified(string fileName);
	}

	public const int MB_1 = 1000000;

	public const int KB_10 = 10000;

	public const int KB_20 = 20000;

	private static IFileSystem fileSystem;

	public static bool canSave
	{
		get
		{
			if (fileSystem != null)
			{
				return fileSystem.canSave;
			}
			return false;
		}
	}

	public static IFileSystem fileSystemImplementation => fileSystem;

	public static void Initalize()
	{
		if (fileSystem == null)
		{
			fileSystem = CrossPlatform.CreateFileSystem();
			fileSystem.Initalize();
		}
	}

	public static bool SaveObject(string filename, object obj, int maxBytes, Action onFinish)
	{
		if (fileSystem == null)
		{
			Initalize();
		}
		return fileSystem.SaveObjectAsync(filename, obj, maxBytes, onFinish);
	}

	public static T LoadObjectUnsafe<T>(string filename)
	{
		if (fileSystem == null)
		{
			Initalize();
		}
		return fileSystem.LoadObject<T>(filename);
	}

	public static void LoadObject<T>(string filename, Action<T> onFinish)
	{
		if (fileSystem == null)
		{
			Initalize();
		}
		fileSystem.LoadObjectAsync(filename, onFinish);
	}

	public static bool Exists(string fileName)
	{
		if (fileSystem == null)
		{
			Initalize();
		}
		return fileSystem.Exists(fileName);
	}

	public static DateTime LastModified(string fileName)
	{
		if (fileSystem == null)
		{
			Initalize();
		}
		return fileSystem.LastModified(fileName);
	}
}
