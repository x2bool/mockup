namespace Mockup.Tests.Targets;

public interface IObjectService
{
    object ReadProperty { get; }
    
    object WriteProperty { set; }
    
    object ReadWriteProperty { get; set; }

    void SingleArgMethod(object arg);

    void MultipleArgsMethod(object arg1, object arg2);

    object ReturnMethod();

    object SingleArgReturnMethod(object arg);

    object MultipleArgsReturnMethod(object arg1, object arg2);
}
