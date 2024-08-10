namespace Mockup.Tests.Targets;

public abstract class AbstractObjectService
{
    public abstract object ReadProperty { get; }
    
    public abstract object WriteProperty { set; }
    
    public abstract object ReadWriteProperty { get; set; }

    public abstract void SingleArgMethod(object arg);

    public abstract void MultipleArgsMethod(object arg1, object arg2);

    public abstract object ReturnMethod();

    public abstract object SingleArgReturnMethod(object arg);

    public abstract object MultipleArgsReturnMethod(object arg1, object arg2);
}

public abstract class AbstractObjectServiceWithConstructor
{
    public object? EmptyValue;
    public object? ArgValue;
    public object? Arg1Value;
    public object? Arg2Value;
    
    public AbstractObjectServiceWithConstructor()
    {
        EmptyValue = "base";
    }

    public AbstractObjectServiceWithConstructor(object arg)
    {
        ArgValue = arg;
    }

    public AbstractObjectServiceWithConstructor(object arg1, object arg2)
    {
        Arg1Value = arg1;
        Arg2Value = arg2;
    }
}
