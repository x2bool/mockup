using Mockup;
using Mockup.Tests.Targets;
using Xunit;

[assembly: Mock(typeof(IStringService))]

namespace Mockup.Tests;

public class StringServiceMockTests
{
    [Fact]
    public void SetupForReadPropertyDelegatesGetterToCallback()
    {
        var value = "Value";

        var stringService = new StringServiceMock()
            .ReadProperty(() => value)
            .Build();
        
        Assert.Equal(value, stringService.ReadProperty);
    }

    [Fact]
    public void SetupForWritePropertyDelegatesSetterToCallback()
    {
        var value = "Value";

        var stringService = new StringServiceMock()
            .WriteProperty(v => value = v)
            .Build();
        
        var changedValue = "ChangedValue";
        
        stringService.WriteProperty = changedValue;
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesGetterToCallbacks()
    {
        var value = "Value";
        
        var stringService = new StringServiceMock()
            .ReadWriteProperty(() => value)
            .Build();
        
        Assert.Equal(value, stringService.ReadWriteProperty);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesSetterToCallbacks()
    {
        var value = "Value";
        
        var stringService = new StringServiceMock()
            .ReadWriteProperty(v => value = v)
            .Build();
        
        var changedValue = "ChangedValue";
        
        stringService.ReadWriteProperty = changedValue;
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesGetterAndSetterToCallbacks()
    {
        var value = "Value";
        
        var stringService = new StringServiceMock()
            .ReadWriteProperty(
                () => value,
                v => value = v)
            .Build();
        
        Assert.Equal(value, stringService.ReadWriteProperty);
        
        var changedValue = "ChangedValue";
        
        stringService.ReadWriteProperty = changedValue;
        Assert.Equal(changedValue, value);
        Assert.Equal(changedValue, stringService.ReadWriteProperty);
    }

    [Fact]
    public void SetupForSingleArgMethodDelegatesToCallback()
    {
        var value = "Value";

        var stringSevice = new StringServiceMock()
            .SingleArgMethod(v => value = v)
            .Build();
        
        var changedValue = "ChangedValue";
        
        stringSevice.SingleArgMethod(changedValue);
        Assert.Equal(changedValue, value);
    }

    [Fact]
    public void SetupForMultipleArgsMethodDelegatesToCallback()
    {
        var value = "Value";

        var stringSevice = new StringServiceMock()
            .MultipleArgsMethod((a, b) => value = a + b)
            .Build();

        stringSevice.MultipleArgsMethod("Changed", "Value");
        Assert.Equal("ChangedValue", value);
    }

    [Fact]
    public void SetupForReturnMethodDelegatesToCallback()
    {
        var value = "Value";

        var stringService = new StringServiceMock()
            .ReturnMethod(() => value)
            .Build();

        var result = stringService.ReturnMethod();
        Assert.Equal(value, result);
    }

    [Fact]
    public void SetupForSingleArgReturnMethodDelegatesToCallback()
    {
        var stringService = new StringServiceMock()
            .SingleArgReturnMethod(v => "Changed" + v)
            .Build();

        var result = stringService.SingleArgReturnMethod("Value");
        Assert.Equal("ChangedValue", result);
    }

    [Fact]
    public void SetupForMultipleArgsReturnMethodDelegatesToCallback()
    {
        var stringService = new StringServiceMock()
            .MultipleArgsReturnMethod((a, b) => a + b)
            .Build();
        
        var result = stringService.MultipleArgsReturnMethod("Changed", "Value");
        Assert.Equal("ChangedValue", result);
    }
}
