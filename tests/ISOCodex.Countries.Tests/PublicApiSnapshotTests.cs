using System.Reflection;
using System.Text;

namespace ISOCodex.Countries.Tests;

public sealed class PublicApiSnapshotTests
{
    [Fact]
    public void Public_Api_Matches_Approved_Snapshot()
    {
        string actual = BuildSnapshot();
        string approved = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "approved-public-api.txt")).ReplaceLineEndings("\n");

        Assert.Equal(approved.TrimEnd(), actual.TrimEnd());
    }

    private static string BuildSnapshot()
    {
        StringBuilder builder = new();
        Type[] exportedTypes = typeof(CountryAlpha2Code).Assembly.GetExportedTypes()
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();

        foreach (Type type in exportedTypes)
        {
            builder.AppendLine(FormatType(type));

            if (type.IsEnum)
            {
                foreach (string name in Enum.GetNames(type))
                {
                    object value = Enum.Parse(type, name);
                    builder.AppendLine("  enum " + name + " = " + Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture).ToString(System.Globalization.CultureInfo.InvariantCulture));
                }

                builder.AppendLine();
                continue;
            }

            foreach (ConstructorInfo constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).OrderBy(FormatConstructor, StringComparer.Ordinal))
            {
                builder.AppendLine("  " + FormatConstructor(constructor));
            }

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).OrderBy(property => property.Name, StringComparer.Ordinal))
            {
                builder.AppendLine("  property " + FormatTypeName(property.PropertyType) + " " + property.Name);
            }

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                         .Where(method => !method.IsSpecialName || IsOperator(method))
                         .OrderBy(FormatMethod, StringComparer.Ordinal))
            {
                builder.AppendLine("  " + FormatMethod(method));
            }

            builder.AppendLine();
        }

        return builder.ToString().ReplaceLineEndings("\n");
    }

    private static string FormatType(Type type)
    {
        string kind = type.IsEnum
            ? "enum"
            : type.IsValueType
                ? "struct"
                : type.IsAbstract && type.IsSealed
                    ? "static class"
                    : "class";

        return kind + " " + type.FullName;
    }

    private static string FormatConstructor(ConstructorInfo constructor) =>
        "ctor " + constructor.DeclaringType!.Name + "(" + FormatParameters(constructor.GetParameters()) + ")";

    private static bool IsOperator(MethodInfo method) =>
        method.IsSpecialName && method.Name.StartsWith("op_", StringComparison.Ordinal);

    private static string FormatMethod(MethodInfo method) =>
        "method " + FormatTypeName(method.ReturnType) + " " + method.Name + "(" + FormatParameters(method.GetParameters()) + ")";

    private static string FormatParameters(ParameterInfo[] parameters) =>
        string.Join(", ", parameters.Select(parameter => FormatTypeName(parameter.ParameterType) + " " + parameter.Name));

    private static string FormatTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            string name = type.GetGenericTypeDefinition().FullName!;
            name = name[..name.IndexOf('`')];
            return name + "<" + string.Join(", ", type.GetGenericArguments().Select(FormatTypeName)) + ">";
        }

        if (type.IsByRef)
        {
            return FormatTypeName(type.GetElementType()!) + "&";
        }

        return type.FullName ?? type.Name;
    }
}
