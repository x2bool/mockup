using Mockup;
using Mockup.Tests.Targets;
using Xunit;

[assembly: Mock(typeof(IArrayService))]

namespace Mockup.Tests;

public class ArrayServiceMockTests
{
    [Fact]
    public void SetupForIntArrayPropDelegatesGetterAndSetterToCallback()
    {
        var arr = new int[1];

        var arrayService = new ArrayServiceMock()
            .IntArrayProp(
                () => arr,
                v => arr = v)
            .Build();
        
        Assert.Equal(arr, arrayService.IntArrayProp);

        var changedArr = new int[2];

        arrayService.IntArrayProp = changedArr;
        Assert.Equal(changedArr, arr);
        Assert.Equal(changedArr, arrayService.IntArrayProp);
    }
    
    [Fact]
    public void SetupForObjectArrayPropDelegatesGetterAndSetterToCallback()
    {
        var arr = new object[1];

        var arrayService = new ArrayServiceMock()
            .ObjectArrayProp(
                () => arr,
                v => arr = v)
            .Build();
        
        Assert.Equal(arr, arrayService.ObjectArrayProp);

        var changedArr = new object[2];

        arrayService.ObjectArrayProp = changedArr;
        Assert.Equal(changedArr, arr);
        Assert.Equal(changedArr, arrayService.ObjectArrayProp);
    }
    
    [Fact]
    public void SetupForStringArrayPropDelegatesGetterAndSetterToCallback()
    {
        var arr = new string[1];

        var arrayService = new ArrayServiceMock()
            .StringArrayProp(
                () => arr,
                v => arr = v)
            .Build();
        
        Assert.Equal(arr, arrayService.StringArrayProp);

        var changedArr = new string[2];

        arrayService.StringArrayProp = changedArr;
        Assert.Equal(changedArr, arr);
        Assert.Equal(changedArr, arrayService.StringArrayProp);
    }
    
    [Fact]
    public void SetupForReturnArrayMethodDelegatesToCallback()
    {
        var arr = new int[1];

        var stringService = new ArrayServiceMock()
            .ReturnArrayMethod(() => arr)
            .Build();

        var result = stringService.ReturnArrayMethod();
        Assert.Equal(arr, result);
    }
    
    [Fact]
    public void SetupForSingleArgArrayMethodDelegatesToCallback()
    {
        var arr = new int[1];

        var stringSevice = new ArrayServiceMock()
            .SingleArgArrayMethod(v => arr = v)
            .Build();

        var changedArr = new int[2];

        stringSevice.SingleArgArrayMethod(changedArr);
        Assert.Equal(changedArr, arr);
    }
    
    [Fact]
    public void SetupForIntJaggedArrayPropDelegatesGetterAndSetterToCallback()
    {
        var arr = new int[1][];

        var arrayService = new ArrayServiceMock()
            .JaggedIntArrayProp(
                () => arr,
                v => arr = v)
            .Build();
        
        Assert.Equal(arr, arrayService.JaggedIntArrayProp);

        var changedArr = new int[2][];

        arrayService.JaggedIntArrayProp = changedArr;
        Assert.Equal(changedArr, arr);
        Assert.Equal(changedArr, arrayService.JaggedIntArrayProp);
    }
    
    [Fact]
    public void SetupForObjectJaggedArrayPropDelegatesGetterAndSetterToCallback()
    {
        var arr = new object[1][];

        var arrayService = new ArrayServiceMock()
            .JaggedObjectArrayProp(
                () => arr,
                v => arr = v)
            .Build();
        
        Assert.Equal(arr, arrayService.JaggedObjectArrayProp);

        var changedArr = new object[2][];

        arrayService.JaggedObjectArrayProp = changedArr;
        Assert.Equal(changedArr, arr);
        Assert.Equal(changedArr, arrayService.JaggedObjectArrayProp);
    }
    
    [Fact]
    public void SetupForStringJaggedArrayPropDelegatesGetterAndSetterToCallback()
    {
        var arr = new string[1][];

        var arrayService = new ArrayServiceMock()
            .JaggedStringArrayProp(
                () => arr,
                v => arr = v)
            .Build();
        
        Assert.Equal(arr, arrayService.JaggedStringArrayProp);

        var changedArr = new string[2][];

        arrayService.JaggedStringArrayProp = changedArr;
        Assert.Equal(changedArr, arr);
        Assert.Equal(changedArr, arrayService.JaggedStringArrayProp);
    }
    
    [Fact]
    public void SetupForReturnJaggedArrayMethodDelegatesToCallback()
    {
        var arr = new int[1][];

        var stringService = new ArrayServiceMock()
            .ReturnJaggedArrayMethod(() => arr)
            .Build();

        var result = stringService.ReturnJaggedArrayMethod();
        Assert.Equal(arr, result);
    }
    
    [Fact]
    public void SetupForSingleArgJaggedArrayMethodDelegatesToCallback()
    {
        var arr = new int[1][];

        var stringSevice = new ArrayServiceMock()
            .SingleArgJaggedArrayMethod(v => arr = v)
            .Build();

        var changedArr = new int[2][];

        stringSevice.SingleArgJaggedArrayMethod(changedArr);
        Assert.Equal(changedArr, arr);
    }
}
