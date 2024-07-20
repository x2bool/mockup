namespace Mockup.Tests.Targets;

public interface IStringService
{
    string ReadProperty { get; }
    
    string WriteProperty { set; }
    
    string ReadWriteProperty { get; set; }

    void SingleArgMethod(string arg);

    void MultipleArgsMethod(string arg1, string arg2);

    string ReturnMethod();

    string SingleArgReturnMethod(string arg);

    string MultipleArgsReturnMethod(string arg1, string arg2);
}
