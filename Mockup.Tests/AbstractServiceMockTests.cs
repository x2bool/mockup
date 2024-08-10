using System;
using Mockup;
using Mockup.Tests.Targets;
using Xunit;

[assembly: Mock(typeof(AbstractObjectService))]

namespace Mockup.Tests;

public class AbstractServiceMockTests
{
    [Fact]
    public void SetupForReadPropertyDelegatesGetterToCallback()
    {
        object value = "Value";

        var abstractObjectService = new AbstractObjectServiceMock()
            .ReadProperty(() => value)
            .Object;
        
        Assert.Equal(value, abstractObjectService.ReadProperty);
    }
    
    [Fact]
    public void SetupForWritePropertyDelegatesSetterToCallback()
    {
        object value = "Value";

        var abstractObjectService = new AbstractObjectServiceMock()
            .WriteProperty(v => value = v)
            .Object;
        
        var changedValue = "ChangedValue";
        
        abstractObjectService.WriteProperty = changedValue;
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesGetterToCallbacks()
    {
        object value = "Value";
        
        var abstractObjectService = new AbstractObjectServiceMock()
            .ReadWriteProperty(() => value)
            .Object;
        
        Assert.Equal(value, abstractObjectService.ReadWriteProperty);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesSetterToCallbacks()
    {
        object value = "Value";
        
        var abstractObjectService = new AbstractObjectServiceMock()
            .ReadWriteProperty(v => value = v)
            .Object;
        
        var changedValue = "ChangedValue";
        
        abstractObjectService.ReadWriteProperty = changedValue;
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesGetterAndSetterToCallbacks()
    {
        object value = "Value";
        
        var abstractObjectService = new AbstractObjectServiceMock()
            .ReadWriteProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(value, abstractObjectService.ReadWriteProperty);
        
        var changedValue = "ChangedValue";
        
        abstractObjectService.ReadWriteProperty = changedValue;
        Assert.Equal(changedValue, value);
        Assert.Equal(changedValue, abstractObjectService.ReadWriteProperty);
    }

    [Fact]
    public void SetupForSingleArgMethodDelegatesToCallback()
    {
        object value = "Value";

        var abstractObjectService = new AbstractObjectServiceMock()
            .SingleArgMethod(v => value = v)
            .Object;
        
        var changedValue = "ChangedValue";
        
        abstractObjectService.SingleArgMethod(changedValue);
        Assert.Equal(changedValue, value);
    }

    [Fact]
    public void SetupForMultipleArgsMethodDelegatesToCallback()
    {
        object value = "Value";

        var abstractObjectService = new AbstractObjectServiceMock()
            .MultipleArgsMethod((a, b) => value = (string)a + (string)b)
            .Object;

        abstractObjectService.MultipleArgsMethod("Changed", "Value");
        Assert.Equal("ChangedValue", value);
    }

    [Fact]
    public void SetupForReturnMethodDelegatesToCallback()
    {
        object value = "Value";

        var abstractObjectService = new AbstractObjectServiceMock()
            .ReturnMethod(() => value)
            .Object;

        var result = abstractObjectService.ReturnMethod();
        Assert.Equal(value, result);
    }

    [Fact]
    public void SetupForSingleArgReturnMethodDelegatesToCallback()
    {
        var abstractObjectService = new AbstractObjectServiceMock()
            .SingleArgReturnMethod(v => "Changed" + v)
            .Object;

        var result = abstractObjectService.SingleArgReturnMethod("Value");
        Assert.Equal("ChangedValue", result);
    }

    [Fact]
    public void SetupForMultipleArgsReturnMethodDelegatesToCallback()
    {
        var abstractObjectService = new AbstractObjectServiceMock()
            .MultipleArgsReturnMethod((a, b) => (string)a + (string)b)
            .Object;
        
        var result = abstractObjectService.MultipleArgsReturnMethod("Changed", "Value");
        Assert.Equal("ChangedValue", result);
    }
}
