using System;

[AttributeUsage(AttributeTargets.Class)]
public class ResourceSingletonAttribute : Attribute
{
	public readonly string resourceFilePath;

	public ResourceSingletonAttribute(string path)
	{
		resourceFilePath = path;
	}
}
