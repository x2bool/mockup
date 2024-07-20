using Mockup;
using Mockup.Tests.Targets;
using Xunit;

[assembly: Mock(typeof(IPrimitiveService))]

namespace Mockup.Tests;

public class PrimitiveServiceMockTests
{
    [Fact]
    public void SetupForSBytePropertyDelegatesGetterAndSetterToCallbacks()
    {
        sbyte value = 0;

        var primitiveService = new PrimitiveServiceMock()
            .SByteProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(0, primitiveService.SByteProperty);

        primitiveService.SByteProperty = 1;
        Assert.Equal(1, primitiveService.SByteProperty);
    }
    
    [Fact]
    public void SetupForBytePropertyDelegatesGetterAndSetterToCallbacks()
    {
        byte value = 0;

        var primitiveService = new PrimitiveServiceMock()
            .ByteProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(0, primitiveService.ByteProperty);

        primitiveService.ByteProperty = 1;
        Assert.Equal(1, primitiveService.ByteProperty);
    }
    
    [Fact]
    public void SetupForShortPropertyDelegatesGetterAndSetterToCallbacks()
    {
        short value = 0;

        var primitiveService = new PrimitiveServiceMock()
            .ShortProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(0, primitiveService.ShortProperty);

        primitiveService.ShortProperty = 1;
        Assert.Equal(1, primitiveService.ShortProperty);
    }
    
    [Fact]
    public void SetupForUShortPropertyDelegatesGetterAndSetterToCallbacks()
    {
        ushort value = 0;

        var primitiveService = new PrimitiveServiceMock()
            .UShortProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(0, primitiveService.UShortProperty);

        primitiveService.UShortProperty = 1;
        Assert.Equal(1, primitiveService.UShortProperty);
    }
    
    [Fact]
    public void SetupForIntPropertyDelegatesGetterAndSetterToCallbacks()
    {
        int value = 0;

        var primitiveService = new PrimitiveServiceMock()
            .IntProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(0, primitiveService.IntProperty);

        primitiveService.IntProperty = 1;
        Assert.Equal(1, primitiveService.IntProperty);
    }
    
    [Fact]
    public void SetupForUIntPropertyDelegatesGetterAndSetterToCallbacks()
    {
        uint value = 0;

        var primitiveService = new PrimitiveServiceMock()
            .UIntProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(0u, primitiveService.UIntProperty);
        
        primitiveService.UIntProperty = 1;
        Assert.Equal(1u, primitiveService.UIntProperty);
    }
    
    [Fact]
    public void SetupForLongPropertyDelegatesGetterAndSetterToCallbacks()
    {
        long value = 0;

        var primitiveService = new PrimitiveServiceMock()
            .LongProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(0, primitiveService.LongProperty);

        primitiveService.LongProperty = 1;
        Assert.Equal(1, primitiveService.LongProperty);
    }
    
    [Fact]
    public void SetupForULongPropertyDelegatesGetterAndSetterToCallbacks()
    {
        ulong value = 0;
    
        var primitiveService = new PrimitiveServiceMock()
            .ULongProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(0ul, primitiveService.ULongProperty);
    
        primitiveService.ULongProperty = 1;
        Assert.Equal(1ul, primitiveService.ULongProperty);
    }

    [Fact]
    public void SetupForCharPropertyDelegatesGetterAndSetterToCallbacks()
    {
        char value = 'a';

        var primitiveService = new PrimitiveServiceMock()
            .CharProperty(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal('a', primitiveService.CharProperty);

        primitiveService.CharProperty = 'b';
        Assert.Equal('b', primitiveService.CharProperty);
    }

    [Fact]
    public void SetupForVoidMethodDelegatesToCallback()
    {
        bool called = false;

        var primitiveService = new PrimitiveServiceMock()
            .VoidMethod(() => called = true)
            .Object;
        
        primitiveService.VoidMethod();
        
        Assert.True(called);
    }

    [Fact]
    public void SetupFor1SByteArgumentDelegatesToCallback()
    {
        sbyte value = 0;

        var primitiveService = new PrimitiveServiceMock()
            .SByteSingleArgMethod(v => value = v)
            .Object;
        
        primitiveService.SByteSingleArgMethod(1);
        
        Assert.Equal(1, value);
    }

    [Fact]
    public void SetupFor2SByteArgumentDelegatesToCallback()
    {
        int value = 0;

        var primitiveService = new PrimitiveServiceMock()
            .SByteMultipleArgsMethod((a, b) => value = a + b)
            .Object;
        
        primitiveService.SByteMultipleArgsMethod(1, 2);
        
        Assert.Equal(3, value);
    }

    [Fact]
    public void SetupForSByteReturnMethodDelegatesToCallback()
    {
        sbyte value = 1;

        var primitiveService = new PrimitiveServiceMock()
            .SByteReturnMethod(() => value)
            .Object;

        var result = primitiveService.SByteReturnMethod();
        
        Assert.Equal(value, result);
    }

    [Fact]
    public void SetupForSByteSingleArgReturnMethodDelegatesToCallback()
    {
        var primitiveService = new PrimitiveServiceMock()
            .SByteSingleArgReturnMethod(a => (sbyte)(a + a))
            .Object;

        var result = primitiveService.SByteSingleArgReturnMethod(1);
        
        Assert.Equal(2, result);
    }

    [Fact]
    public void SetupForSByteMultipleArgsReturnMethodDelegatesToCallback()
    {
        var primitiveService = new PrimitiveServiceMock()
            .SByteMultipleArgsReturnMethod((a, b) => (sbyte)(a + b))
            .Object;

        var result = primitiveService.SByteMultipleArgsReturnMethod(1, 2);
        
        Assert.Equal(3, result);
    }
}