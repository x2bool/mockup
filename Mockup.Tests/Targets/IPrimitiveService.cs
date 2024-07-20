namespace Mockup.Tests.Targets;

public interface IPrimitiveService
{
    public sbyte SByteProperty { get; set; }
    
    public byte ByteProperty { get; set; }
    
    public short ShortProperty { get; set; }
    
    public ushort UShortProperty { get; set; }
    
    public int IntProperty { get; set; }
    
    public uint UIntProperty { get; set; }
    
    public long LongProperty { get; set; }
    
    public ulong ULongProperty { get; set; }
    
    public char CharProperty { get; set; }

    public void VoidMethod();

    public void SByteSingleArgMethod(sbyte arg);

    public void SByteMultipleArgsMethod(sbyte arg1, sbyte arg2);

    public sbyte SByteReturnMethod();

    public sbyte SByteSingleArgReturnMethod(sbyte arg);

    public sbyte SByteMultipleArgsReturnMethod(sbyte arg1, sbyte arg2);
}
