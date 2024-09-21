using Mockup;
using Mockup.Tests.Targets;
using Xunit;

[assembly: Mock(typeof(IObjectService))]

namespace Mockup.Tests;

public class ObjectServiceMockTests
{
    [Fact]
    public void SetupForReadPropertyDelegatesGetterToCallback()
    {
        object value = "Value";

        var objectService = new ObjectServiceMock()
            .ReadProperty(() => value)
            .Build();
        
        Assert.Equal(value, objectService.ReadProperty);
    }

    [Fact]
    public void SetupForWritePropertyDelegatesSetterToCallback()
    {
        object value = "Value";

        var objectService = new ObjectServiceMock()
            .WriteProperty(v => value = v)
            .Build();
        
        var changedValue = "ChangedValue";
        
        objectService.WriteProperty = changedValue;
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesGetterToCallbacks()
    {
        object value = "Value";
        
        var objectService = new ObjectServiceMock()
            .ReadWriteProperty(() => value)
            .Build();
        
        Assert.Equal(value, objectService.ReadWriteProperty);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesSetterToCallbacks()
    {
        object value = "Value";
        
        var objectService = new ObjectServiceMock()
            .ReadWriteProperty(v => value = v)
            .Build();
        
        var changedValue = "ChangedValue";
        
        objectService.ReadWriteProperty = changedValue;
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesGetterAndSetterToCallbacks()
    {
        object value = "Value";
        
        var objectService = new ObjectServiceMock()
            .ReadWriteProperty(
                () => value,
                v => value = v)
            .Build();
        
        Assert.Equal(value, objectService.ReadWriteProperty);
        
        var changedValue = "ChangedValue";
        
        objectService.ReadWriteProperty = changedValue;
        Assert.Equal(changedValue, value);
        Assert.Equal(changedValue, objectService.ReadWriteProperty);
    }

    [Fact]
    public void SetupForSingleArgMethodDelegatesToCallback()
    {
        object value = "Value";

        var objectService = new ObjectServiceMock()
            .SingleArgMethod(v => value = v)
            .Build();
        
        var changedValue = "ChangedValue";
        
        objectService.SingleArgMethod(changedValue);
        Assert.Equal(changedValue, value);
    }

    [Fact]
    public void SetupForMultipleArgsMethodDelegatesToCallback()
    {
        object value = "Value";

        var objectService = new ObjectServiceMock()
            .MultipleArgsMethod((a, b) => value = (string)a + (string)b)
            .Build();

        objectService.MultipleArgsMethod("Changed", "Value");
        Assert.Equal("ChangedValue", value);
    }

    [Fact]
    public void SetupForReturnMethodDelegatesToCallback()
    {
        object value = "Value";

        var objectService = new ObjectServiceMock()
            .ReturnMethod(() => value)
            .Build();

        var result = objectService.ReturnMethod();
        Assert.Equal(value, result);
    }

    [Fact]
    public void SetupForSingleArgReturnMethodDelegatesToCallback()
    {
        var objectService = new ObjectServiceMock()
            .SingleArgReturnMethod(v => "Changed" + v)
            .Build();

        var result = objectService.SingleArgReturnMethod("Value");
        Assert.Equal("ChangedValue", result);
    }

    [Fact]
    public void SetupForMultipleArgsReturnMethodDelegatesToCallback()
    {
        var objectService = new ObjectServiceMock()
            .MultipleArgsReturnMethod((a, b) => (string)a + (string)b)
            .Build();
        
        var result = objectService.MultipleArgsReturnMethod("Changed", "Value");
        Assert.Equal("ChangedValue", result);
    }
}
