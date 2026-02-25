using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Loader;

// 主动加载所有相关程序集
var dllFiles = Directory.GetFiles(
    AppContext.BaseDirectory,
    "CSharp.*.dll",
    SearchOption.TopDirectoryOnly);

foreach (var dll in dllFiles)
{
    try
    {
        AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);
    }
    catch { }
}

// 现在再获取所有程序集
var csharpAssemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(assembly =>
    {
        var assemblyName = assembly.GetName().Name;
        return assemblyName != null && assemblyName.AsSpan().StartsWith("CSharp.");
    });

// 获取所有带有DisplayAttribute的类
var classesWithDisplayAttribute = csharpAssemblies
    .SelectMany(assembly => assembly.GetTypes())
    .Where(type => type.GetCustomAttributes(typeof(DisplayAttribute), false).Length != 0);

// 执行每个类的静态Execute方法
foreach (var type in classesWithDisplayAttribute)
{
    var executeMethod = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
    if (executeMethod != null)
    {
        try
        {
            executeMethod.Invoke(null, null);
            Console.WriteLine($"Executed {type.FullName}.Execute()");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing {type.FullName}.Execute(): {ex.Message}");
        }

        Console.WriteLine();
    }
}

Console.ReadLine();