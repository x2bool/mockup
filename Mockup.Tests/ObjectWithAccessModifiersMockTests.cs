using System;
using Mockup;
using Mockup.Tests.Targets;
using Xunit;

[assembly: Mock(typeof(ObjectWithAccessModifiersService))]

namespace Mockup.Tests;

public class ObjectWithAccessModifiersMockTests
{
    [Fact]
    public void SetupForPublicMethodDelegatesToCallback()
    {
        object value = "Value";

        var objectService = new ObjectWithAccessModifiersServiceMock()
            .PublicMethod(v => value = v)
            .Build();
        
        var changedValue = "ChangedValue";
        
        objectService.PublicMethod(changedValue);
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForProtectedMethodDelegatesToCallback()
    {
        object value = "Value";

        var objectService = new ObjectWithAccessModifiersServiceMock()
            .ProtectedMethod(v => value = v)
            .Build();
        
        var changedValue = "ChangedValue";
        
        objectService.InvokeProtectedMethod(changedValue);
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForInternalMethodDelegatesToCallback()
    {
        object value = "Value";

        var objectService = new ObjectWithAccessModifiersServiceMock()
            .InternalMethod(v => value = v)
            .Build();
        
        var changedValue = "ChangedValue";
        
        objectService.InvokeInternalMethod(changedValue);
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForProtectedInternalMethodDelegatesToCallback()
    {
        object value = "Value";

        var objectService = new ObjectWithAccessModifiersServiceMock()
            .ProtectedInternalMethod(v => value = v)
            .Build();
        
        var changedValue = "ChangedValue";
        
        objectService.InvokeProtectedInternalMethod(changedValue);
        Assert.Equal(changedValue, value);
    }
}