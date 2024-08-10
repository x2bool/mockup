using Mockup;
using Mockup.Tests.Targets;
using Xunit;

[assembly: Mock<IEmptyService>]

namespace Mockup.Tests;

public class GenericMockAttributeTests
{
    [Fact]
    public void GenericMockAttributeProducesCorrectType()
    {
        var emptyService = new EmptyServiceMock()
            .Build();
        
        Assert.True(typeof(IEmptyService).IsAssignableFrom(emptyService.GetType()));
    }
}
