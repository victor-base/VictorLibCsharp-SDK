using LibTest;
using TestFunc;
using VictorNative;

class Program
{
    static void Main(string[] args)
    {
        // Ejecutamos todas las pruebas desde TestRunner
        // TestRunner.RunTests();

        // var Lib = new TestLoadLibrary();

        // TestLoadLibrary.LibLoader();

        TestBinding.TestAllocIndex();
    }
}
