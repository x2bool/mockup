namespace Mockup.Tests.Targets;

public interface IArrayService
{
    int[] IntArrayProp { get; set; }
    
    object[] ObjectArrayProp { get; set; }
    
    string[] StringArrayProp { get; set; }

    int[] ReturnArrayMethod();

    void SingleArgArrayMethod(int[] arg);
    
    int[][] JaggedIntArrayProp { get; set; }
    
    object[][] JaggedObjectArrayProp { get; set; }
    
    string[][] JaggedStringArrayProp { get; set; }

    int[][] ReturnJaggedArrayMethod();

    void SingleArgJaggedArrayMethod(int[][] arg);
}
