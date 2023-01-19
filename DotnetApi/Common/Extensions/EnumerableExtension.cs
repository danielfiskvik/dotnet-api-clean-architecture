using System.Reflection;

namespace Common.Extensions;


public static class EnumerableExtension
{
    public static IEnumerable<TSource> WhereEntitiesImplementing<TSource>(this IEnumerable<TSource> source, Func<Type, bool> predicate)
    {
        return source.Where(entity => entity.IsImplementingFrom(predicate));
    }

    public static bool IsImplementingFrom<TEntity>(this TEntity? entity, Func<Type, bool> predicate)
    {
        return entity != null && entity
            .GetType()
            .GetInterfaces()
            .Any(predicate);
    }

    public static IEnumerable<TSource> WhereEntitiesHasProperty<TSource>(this IEnumerable<TSource> source, Func<string, bool> predicate)
    {
        return source.Where(x => x.HasProperty(predicate));
    }

    public static bool HasProperty<TEntity>(this TEntity? entity, Func<string, bool> predicate)
    {
        return entity != null && entity
            .GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(x => x.Name)
            .Any(predicate);
    }

    public static object? GetValue<TEntity>(this TEntity? entity, string property)
    {
       return entity?.GetType()
            .GetProperty(property)?
            .GetValue(entity, null);
    }
    
    public static bool ValueIs<TEntity, TType>(
        this TEntity entity,
        string property,
        Func<TType, bool> predicate)
    {

        var obj = entity
            .HasProperty(x => x == property)
            ? entity.GetValue(property)
            : default;
            
        var type = (TType?) obj;

        return type != null && predicate.Invoke(type);
    }
    
    public static TEntity? ChangeType<TEntity>(object? value) 
    {
        var t = typeof(TEntity);

        if (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(Nullable<>))
            return (TEntity?)Convert.ChangeType(value, t);
            
        if (value == null) return default; 

        t = Nullable.GetUnderlyingType(t);
        if (t is null) return default;

        return (TEntity)Convert.ChangeType(value, t);
    }
}