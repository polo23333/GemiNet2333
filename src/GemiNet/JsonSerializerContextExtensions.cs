using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace GemiNet;

internal static class JsonSerializerContextExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsonTypeInfo<T>? GetTypeInfo<T>(this JsonSerializerContext context)
    {
        return (JsonTypeInfo<T>?)context.GetTypeInfo(typeof(T));
    }
}