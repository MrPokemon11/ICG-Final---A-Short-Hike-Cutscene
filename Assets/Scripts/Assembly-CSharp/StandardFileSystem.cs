using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class StandardFileSystem : FileSystem.IFileSystem
{
	protected string path;

	public bool canSave => true;

	public virtual void Initalize()
	{
		path = Application.persistentDataPath + "/";
	}

	public bool Exists(string fileName)
	{
		return File.Exists(path + fileName);
	}

	public T LoadObject<T>(string filename)
	{
		string text = path + filename;
		try
		{
			if (File.Exists(text))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				GlobalData.AddSerializationSurrogates(binaryFormatter);
				FileStream fileStream = File.Open(text, FileMode.Open, FileAccess.Read);
				T result = (T)binaryFormatter.Deserialize(fileStream);
				fileStream.Close();
				return result;
			}
		}
		catch (IOException exception)
		{
			Debug.LogError("Exception loading file: " + text);
			Debug.LogException(exception);
		}
		catch (SerializationException exception2)
		{
			Debug.LogError("File is invalid: " + text);
			Debug.LogException(exception2);
		}
		return default(T);
	}

	public void LoadObjectAsync<T>(string filename, Action<T> onFinish)
	{
		T obj = LoadObject<T>(filename);
		onFinish?.Invoke(obj);
	}

	public bool SaveObjectAsync(string filename, object obj, int saveDataSize, Action onFinish)
	{
		string text = path + filename;
		if (File.Exists(text))
		{
			string destFileName = text + "_backup";
			if (File.Exists(destFileName))
			{
				File.Delete(destFileName);
			}
			File.Move(text, destFileName);
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		GlobalData.AddSerializationSurrogates(binaryFormatter);
		FileStream fileStream = File.Create(text);
		binaryFormatter.Serialize(fileStream, obj);
		fileStream.Close();
		onFinish?.Invoke();
		return true;
	}

	public DateTime LastModified(string fileName)
	{
		return File.GetLastWriteTime(path + fileName);
	}
}
