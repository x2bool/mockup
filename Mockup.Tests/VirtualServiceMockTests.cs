using System;
using Mockup;
using Mockup.Tests.Targets;
using Xunit;

[assembly: Mock(typeof(VirtualObjectService))]

namespace Mockup.Tests;

public class VirtualServiceMockTests
{
    [Fact]
    public void SetupForReadPropertyDelegatesGetterToCallback()
    {
        object value = "Value";

        var virtualObjectService = new VirtualObjectServiceMock()
            .ReadProperty(() => value)
            .Object;
        
        Assert.Equal(value, virtualObjectService.ReadProperty);
    }

    [Fact]
    public void DefaultImplOfReadPropertyDelegatesGetterToBase()
    {
        var virtualObjectService = new VirtualObjectServiceMock()
            .Object;

        Assert.Equal("base", virtualObjectService.ReadProperty);
    }
    
    [Fact]
    public void SetupForWritePropertyDelegatesSetterToCallback()
    {
        object value = "Value";

        var virtualObjectService = new VirtualObjectServiceMock()
            .WriteProperty(v => value = v)
            .Object;
        
        var changedValue = "ChangedValue";
        
        virtualObjectService.WriteProperty = changedValue;
        Assert.Equal(changedValue, value);
    }

    [Fact]
    public void DefaultImplOfWritePropertyDelegatesSetterToBase()
    {
        var virtualObjectService = new VirtualObjectServiceMock()
            .Object;

        virtualObjectService.WriteProperty = new object(); // any value
        Assert.Equal("base", virtualObjectService.WritePropertyValue);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesGetterToCallbacks()
    {
        object value = "Value";
        
        var virtualObjectService = new VirtualObjectServiceMock()
            .ReadWriteProperty(() => value)
            .Object;
        
        Assert.Equal(value, virtualObjectService.ReadWriteProperty);
    }
    
    [Fact]
    public void DefaultImplOfReadWritePropertyDelegatesGetterToBase()
    {
        object value = "Value";
        
        var virtualObjectService = new VirtualObjectServiceMock()
            .Object;
        
        Assert.Equal("base", virtualObjectService.ReadWriteProperty);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesSetterToCallbacks()
    {
        object value = "Value";
        
        var virtualObjectService = new VirtualObjectServiceMock()
            .ReadWriteProperty(v => value = v)
            .Object;
        
        var changedValue = "ChangedValue";
        
        virtualObjectService.ReadWriteProperty = changedValue;
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void DefaultImplOfReadWritePropertyDelegatesSetterToBase()
    {
        var virtualObjectService = new VirtualObjectServiceMock()
            .Object;
        
        virtualObjectService.ReadWriteProperty = new object(); // any value
        Assert.Equal("base", virtualObjectService.ReadWritePropertyValue);
    }
    
    [Fact]
    public void SetupForReadWritePropertyDelegatesGetterAndSetterToCallbacks()
    {
        object value = "Value";
        
        var virtualObjectService = new VirtualObjectServiceMock()
            .ReadWriteProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(value, virtualObjectService.ReadWriteProperty);
        
        var changedValue = "ChangedValue";
        
        virtualObjectService.ReadWriteProperty = changedValue;
        Assert.Equal(changedValue, value);
        Assert.Equal(changedValue, virtualObjectService.ReadWriteProperty);
    }

    [Fact]
    public void SetupForSingleArgMethodDelegatesToCallback()
    {
        object value = "Value";

        var virtualObjectService = new VirtualObjectServiceMock()
            .SingleArgMethod(v => value = v)
            .Object;
        
        var changedValue = "ChangedValue";
        
        virtualObjectService.SingleArgMethod(changedValue);
        Assert.Equal(changedValue, value);
    }

    [Fact]
    public void DefaultImplOfSingleArgMethodDelegatesToBase()
    {
        var virtualObjectService = new VirtualObjectServiceMock()
            .Object;
        
        virtualObjectService.SingleArgMethod(new object()); // any value
        Assert.Equal("base", virtualObjectService.SingleArgMethodValue);
    }

    [Fact]
    public void SetupForMultipleArgsMethodDelegatesToCallback()
    {
        object value = "Value";

        var virtualObjectService = new VirtualObjectServiceMock()
            .MultipleArgsMethod((a, b) => value = (string)a + (string)b)
            .Object;

        virtualObjectService.MultipleArgsMethod("Changed", "Value");
        Assert.Equal("ChangedValue", value);
    }

    [Fact]
    public void DefaultImplOfMultipleArgsMethodDelegatesToBase()
    {
        var virtualObjectService = new VirtualObjectServiceMock()
            .Object;

        virtualObjectService.MultipleArgsMethod(new object(), new object()); // any values
        Assert.Equal("base", virtualObjectService.MultipleArgsMethodValue);
    }

    [Fact]
    public void SetupForReturnMethodDelegatesToCallback()
    {
        object value = "Value";

        var virtualObjectService = new VirtualObjectServiceMock()
            .ReturnMethod(() => value)
            .Object;

        var result = virtualObjectService.ReturnMethod();
        Assert.Equal(value, result);
    }

    [Fact]
    public void DefaultImplOfReturnMethodDelegatesToBase()
    {
        var virtualObjectService = new VirtualObjectServiceMock()
            .Object;

        var result = virtualObjectService.ReturnMethod();
        Assert.Equal("base", result);
    }

    [Fact]
    public void SetupForSingleArgReturnMethodDelegatesToCallback()
    {
        var virtualObjectService = new VirtualObjectServiceMock()
            .SingleArgReturnMethod(v => "Changed" + v)
            .Object;

        var result = virtualObjectService.SingleArgReturnMethod("Value");
        Assert.Equal("ChangedValue", result);
    }

    [Fact]
    public void DefaultImplOfSingleArgReturnMethodDelegatesToBase()
    {
        var virtualObjectService = new VirtualObjectServiceMock()
            .Object;

        var result = virtualObjectService.SingleArgReturnMethod(new object()); // any value
        Assert.Equal("base", result);
    }

    [Fact]
    public void SetupForMultipleArgsReturnMethodDelegatesToCallback()
    {
        var virtualObjectService = new VirtualObjectServiceMock()
            .MultipleArgsReturnMethod((a, b) => (string)a + (string)b)
            .Object;
        
        var result = virtualObjectService.MultipleArgsReturnMethod("Changed", "Value");
        Assert.Equal("ChangedValue", result);
    }

    [Fact]
    public void DefaultImplOfMultipleArgsReturnMethodDelegatesToBase()
    {
        var virtualObjectService = new VirtualObjectServiceMock()
            .Object;
        
        var result = virtualObjectService.MultipleArgsReturnMethod(new object(), new object()); // any value
        Assert.Equal("base", result);
    }
}
