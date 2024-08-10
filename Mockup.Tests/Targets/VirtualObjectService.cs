namespace Mockup.Tests.Targets;

public class VirtualObjectService
{
    public object WritePropertyValue;
    public object ReadWritePropertyValue;
    public object SingleArgMethodValue;
    public object MultipleArgsMethodValue;
    
    public virtual object ReadProperty
    {
        get
        {
            return "base";
        }
    }

    public virtual object WriteProperty
    {
        set
        {
            WritePropertyValue = "base";
        }
    }

    public virtual object ReadWriteProperty
    {
        get
        {
            return "base";
        }
        
        set
        {
            ReadWritePropertyValue = "base";
        }
    }

    public virtual void SingleArgMethod(object arg)
    {
        SingleArgMethodValue = "base";
    }

    public virtual void MultipleArgsMethod(object arg1, object arg2)
    {
        MultipleArgsMethodValue = "base";
    }

    public virtual object ReturnMethod()
    {
        return "base";
    }

    public virtual object SingleArgReturnMethod(object arg)
    {
        return "base";
    }

    public virtual object MultipleArgsReturnMethod(object arg1, object arg2)
    {
        return "base";
    }
}
