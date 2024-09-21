namespace Mockup.Tests.Targets;

public abstract class ObjectWithAccessModifiersService
{
    public void InvokeProtectedMethod(object arg) => ProtectedMethod(arg);
    public void InvokeInternalMethod(object arg) => InternalMethod(arg);
    public void InvokeProtectedInternalMethod(object arg) => ProtectedInternalMethod(arg);
    
    public abstract void PublicMethod(object arg);
    protected abstract void ProtectedMethod(object arg);
    internal abstract void InternalMethod(object arg);
    protected internal abstract void ProtectedInternalMethod(object arg);
}
