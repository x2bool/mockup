using System.Text;
using Mockup;
using Mockup.Tests.Dependecies;
using Mockup.Tests.Targets;
using Xunit;

[assembly: Mock(typeof(IDependencyService))]

namespace Mockup.Tests;

public class DependencyServiceMockTests
{
    [Fact]
    public void SetupForClassPropDelegatesGetterAndSetterToCallback()
    {
        var value = new ClassDependency();

        var dependencyService = new DependencyServiceMock()
            .ClassProp(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(value, dependencyService.ClassProp);

        var changedValue = new ClassDependency();
        dependencyService.ClassProp = changedValue;
        Assert.Equal(changedValue, value);
        Assert.Equal(changedValue, dependencyService.ClassProp);
    }

    [Fact]
    public void SetupForReturnClassMethodDelegatesToCallback()
    {
        var value = new ClassDependency();

        var dependencyService = new DependencyServiceMock()
            .ReturnClassMethod(() => value)
            .Object;
        
        Assert.Equal(value, dependencyService.ReturnClassMethod());
    }

    [Fact]
    public void SetupForSingleArgClassMethodDelegatesToCallback()
    {
        var value = new ClassDependency();

        var dependencyService = new DependencyServiceMock()
            .SingleArgClassMethod(v => value = v)
            .Object;

        var changedValue = new ClassDependency();
        dependencyService.SingleArgClassMethod(changedValue);
        
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForInterfacePropDelegatesGetterAndSetterToCallback()
    {
        IDependency value = new ClassDependency();

        var dependencyService = new DependencyServiceMock()
            .InterfaceProp(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(value, dependencyService.InterfaceProp);

        IDependency changedValue = new ClassDependency();
        dependencyService.InterfaceProp = changedValue;
        Assert.Equal(changedValue, value);
        Assert.Equal(changedValue, dependencyService.InterfaceProp);
    }

    [Fact]
    public void SetupForReturnInterfaceMethodDelegatesToCallback()
    {
        IDependency value = new ClassDependency();

        var dependencyService = new DependencyServiceMock()
            .ReturnInterfaceMethod(() => value)
            .Object;
        
        Assert.Equal(value, dependencyService.ReturnInterfaceMethod());
    }

    [Fact]
    public void SetupForSingleArgInterfaceMethodDelegatesToCallback()
    {
        IDependency value = new ClassDependency();

        var dependencyService = new DependencyServiceMock()
            .SingleArgInterfaceMethod(v => value = v)
            .Object;

        var changedValue = new ClassDependency();
        dependencyService.SingleArgInterfaceMethod(changedValue);
        
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForStructPropDelegatesGetterAndSetterToCallback()
    {
        var value = new StructDependency(1);

        var dependencyService = new DependencyServiceMock()
            .StructProp(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(value, dependencyService.StructProp);

        var changedValue = new StructDependency(2);
        dependencyService.StructProp = changedValue;
        Assert.Equal(changedValue, value);
        Assert.Equal(changedValue, dependencyService.StructProp);
    }

    [Fact]
    public void SetupForReturnStructMethodDelegatesToCallback()
    {
        var value = new StructDependency(1);

        var dependencyService = new DependencyServiceMock()
            .ReturnStructMethod(() => value)
            .Object;
        
        Assert.Equal(value, dependencyService.ReturnStructMethod());
    }

    [Fact]
    public void SetupForSingleArgStructMethodDelegatesToCallback()
    {
        var value = new StructDependency(1);

        var dependencyService = new DependencyServiceMock()
            .SingleArgStructMethod(v => value = v)
            .Object;

        var changedValue = new StructDependency(2);
        dependencyService.SingleArgStructMethod(changedValue);
        
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForEnumPropDelegatesGetterAndSetterToCallback()
    {
        var value = EnumDependency.Value1;

        var dependencyService = new DependencyServiceMock()
            .EnumProp(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(value, dependencyService.EnumProp);

        var changedValue = EnumDependency.Value2;
        dependencyService.EnumProp = changedValue;
        Assert.Equal(changedValue, value);
        Assert.Equal(changedValue, dependencyService.EnumProp);
    }

    [Fact]
    public void SetupForReturnEnumMethodDelegatesToCallback()
    {
        var value = EnumDependency.Value1;

        var dependencyService = new DependencyServiceMock()
            .ReturnEnumMethod(() => value)
            .Object;
        
        Assert.Equal(value, dependencyService.ReturnEnumMethod());
    }

    [Fact]
    public void SetupForSingleArgEnumMethodDelegatesToCallback()
    {
        var value = EnumDependency.Value1;

        var dependencyService = new DependencyServiceMock()
            .SingleArgEnumMethod(v => value = v)
            .Object;

        var changedValue = EnumDependency.Value2;
        dependencyService.SingleArgEnumMethod(changedValue);
        
        Assert.Equal(changedValue, value);
    }
    
    [Fact]
    public void SetupForExternalPropDelegatesGetterAndSetterToCallback()
    {
        var value = new StringBuilder();

        var dependencyService = new DependencyServiceMock()
            .ExternalProp(
                () => value,
                v => value = v)
            .Object;
        
        Assert.Equal(value, dependencyService.ExternalProp);

        var changedValue = new StringBuilder();
        dependencyService.ExternalProp = changedValue;
        Assert.Equal(changedValue, value);
        Assert.Equal(changedValue, dependencyService.ExternalProp);
    }

    [Fact]
    public void SetupForReturnExternalMethodDelegatesToCallback()
    {
        var value = new StringBuilder();

        var dependencyService = new DependencyServiceMock()
            .ReturnExternalMethod(() => value)
            .Object;
        
        Assert.Equal(value, dependencyService.ReturnExternalMethod());
    }

    [Fact]
    public void SetupForSingleArgExternalMethodDelegatesToCallback()
    {
        var value = new StringBuilder();

        var dependencyService = new DependencyServiceMock()
            .SingleArgExternalMethod(v => value = v)
            .Object;

        var changedValue = new StringBuilder();
        dependencyService.SingleArgExternalMethod(changedValue);
        
        Assert.Equal(changedValue, value);
    }
}
