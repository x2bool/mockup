using System.Text;
using Mockup.Tests.Dependecies;

namespace Mockup.Tests.Targets;

public interface IDependencyService
{
    ClassDependency ClassProp { get; set; }

    ClassDependency ReturnClassMethod();

    void SingleArgClassMethod(ClassDependency arg);
    
    IDependency InterfaceProp { get; set; }

    IDependency ReturnInterfaceMethod();

    void SingleArgInterfaceMethod(IDependency arg);
    
    StructDependency StructProp { get; set; }

    StructDependency ReturnStructMethod();

    void SingleArgStructMethod(StructDependency arg);
    
    EnumDependency EnumProp { get; set; }

    EnumDependency ReturnEnumMethod();

    void SingleArgEnumMethod(EnumDependency arg);
    
    StringBuilder ExternalProp { get; set; }

    StringBuilder ReturnExternalMethod();

    void SingleArgExternalMethod(StringBuilder arg);
}
