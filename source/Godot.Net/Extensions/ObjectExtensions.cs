namespace Godot.Net.Extensions;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot.Net.Core;
using Godot.Net.Core.Object;
using Godot.Net.Attributes;

public static class ObjectExtensions
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)]
    private static readonly Type godotObjectType = typeof(GodotObject);
    private static readonly Type objectType      = typeof(object);
    private static readonly Type stringType      = typeof(string);
    private static readonly Type typeType        = typeof(Type);

    private static readonly MethodInfo memberWiseClone = objectType.GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!;

    [UnconditionalSuppressMessage("Aot", "IL3050:RequiresDynamicCode", Justification = "The unfriendly method is not reachable with AOT")]
    private static object Clone(this object target, bool deep, Dictionary<object, object> references)
    {
        var type = target.GetType();

        if (type.IsValueType || type == stringType || type == typeType)
        {
            return target;
        }

        if (references.TryGetValue(target, out var reference))
        {
            return reference;
        }

        if (type.IsArray)
        {
            var array       = (Array)target;
            var clonedArray = Array.CreateInstance(type.GetElementType()!, array.Length);

            references[target] = clonedArray;

            for (var i = 0; i < array.Length; i++)
            {
                var value = array.GetValue(i);

                if (deep)
                {
                    clonedArray.SetValue(value?.Clone(deep, references), i);
                }
                else
                {
                    clonedArray.SetValue(value == target ? clonedArray : value, i);
                }
            }

            return clonedArray;
        }

        var clonedObject = memberWiseClone.Invoke(target, null)!;

        if (clonedObject is GodotObject)
        {
            godotObjectType.GetField($"<{nameof(GodotObject.InstanceId)}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(clonedObject, InstanceTracker.GetId(clonedObject));
        }

        references[target] = clonedObject;

        var next = type;

        while (next != null)
        {
            var fields = next.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.FieldType.IsValueType || field.FieldType == stringType || field.FieldType == typeType)
                {
                    continue;
                }

                var value = field.GetValue(target);

                ClonableAttribute? clonable = null;

                if ((deep || value is System.Collections.ICollection || (clonable = field.GetCustomAttribute<ClonableAttribute>()) != null) && field.GetCustomAttribute<NonClonableAttribute>() == null)
                {
                    field.SetValue(clonedObject, value?.Clone(deep || (clonable?.Deep ?? false), references));
                }
                else if (value == target)
                {
                    field.SetValue(clonedObject, clonedObject);
                }
            }

            next = next.BaseType == objectType ? null : next.BaseType;
        }

        return clonedObject;
    }

    public static object? Call(this object target, string method, params object[] args)
    {
        var type       = target.GetType();
        var methodInfo = type.GetMethod(method);

        return methodInfo == null
            ? throw new InvalidOperationException($"Method ${method} not founded on type {type.Name}")
            : methodInfo.Invoke(target, args);
    }

    public static object? CallIfExists(this object target, string method, params object[] args)
    {
        var methodInfo = target.GetType().GetMethod(method);

        return methodInfo?.Invoke(target, args);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static object Clone(this object target, bool deep = false) =>
        Clone(target, deep, new());

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T Clone<T>(this T target, bool deep = false) where T : notnull =>
        (T)Clone((object)target, deep);
}
