namespace Mockup;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class MockAttribute : Attribute
{
    public Type Type { get; }
    
    public MockAttribute(Type type)
    {
        Type = type;
    }
}

// public class MockAttribute<T> : MockAttribute
// {
//     public MockAttribute()
//         : base(typeof(T))
//     {
//         
//     }
// }
