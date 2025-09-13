using System.Reflection;

namespace CleanArchitecture.Persistance;

public static class AssemblyReference
{
    public static readonly Assembly assembly = typeof(Assembly).Assembly;
}
