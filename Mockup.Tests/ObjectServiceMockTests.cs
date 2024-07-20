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

        var stringService = new ObjectServiceMock()
            .ReadProperty(() => value)
            .Object;
        
        Assert.Equal(value, stringService.ReadProperty);
    }

    [Fact]
    public void SetupForWritePropertyDelegatesSetterToCallback()
    {
        object value = "Value";

        var stringService = new ObjectServiceMock()
            .WriteProperty(v => value = v)
            .Object;
        
        var changedValue = "ChangedValue";
        
        stringService.WriteProperty = changedValue;
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesGetterToCallbacks()
    {
        object value = "Value";
        
        var stringService = new ObjectServiceMock()
            .ReadWriteProperty(() => value)
            .Object;
        
        Assert.Equal(value, stringService.ReadWriteProperty);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesSetterToCallbacks()
    {
        object value = "Value";
        
        var stringService = new ObjectServiceMock()
            .ReadWriteProperty(v => value = v)
            .Object;
        
        var changedValue = "ChangedValue";
        
        stringService.ReadWriteProperty = changedValue;
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesGetterAndSetterToCallbacks()
    {
        object value = "Value";
        
        var stringService = new ObjectServiceMock()
            .ReadWriteProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(value, stringService.ReadWriteProperty);
        
        var changedValue = "ChangedValue";
        
        stringService.ReadWriteProperty = changedValue;
        Assert.Equal(changedValue, value);
        Assert.Equal(changedValue, stringService.ReadWriteProperty);
    }

    [Fact]
    public void SetupForSingleArgMethodDelegatesToCallback()
    {
        object value = "Value";

        var stringSevice = new ObjectServiceMock()
            .SingleArgMethod(v => value = v)
            .Object;
        
        var changedValue = "ChangedValue";
        
        stringSevice.SingleArgMethod(changedValue);
        Assert.Equal(changedValue, value);
    }

    [Fact]
    public void SetupForMultipleArgsMethodDelegatesToCallback()
    {
        object value = "Value";

        var stringSevice = new ObjectServiceMock()
            .MultipleArgsMethod((a, b) => value = (string)a + (string)b)
            .Object;

        stringSevice.MultipleArgsMethod("Changed", "Value");
        Assert.Equal("ChangedValue", value);
    }

    [Fact]
    public void SetupForReturnMethodDelegatesToCallback()
    {
        object value = "Value";

        var stringService = new ObjectServiceMock()
            .ReturnMethod(() => value)
            .Object;

        var result = stringService.ReturnMethod();
        Assert.Equal(value, result);
    }

    [Fact]
    public void SetupForSingleArgReturnMethodDelegatesToCallback()
    {
        var stringService = new ObjectServiceMock()
            .SingleArgReturnMethod(v => "Changed" + v)
            .Object;

        var result = stringService.SingleArgReturnMethod("Value");
        Assert.Equal("ChangedValue", result);
    }

    [Fact]
    public void SetupForMultipleArgsReturnMethodDelegatesToCallback()
    {
        var stringService = new ObjectServiceMock()
            .MultipleArgsReturnMethod((a, b) => (string)a + (string)b)
            .Object;
        
        var result = stringService.MultipleArgsReturnMethod("Changed", "Value");
        Assert.Equal("ChangedValue", result);
    }
}
