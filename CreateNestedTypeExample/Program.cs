using Mono.Cecil;
using SoftCube.Logging;
using System;
using System.Linq;

namespace Mono.Cecil.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var inputAssemblyFileName  = "SoftCube.Assembly.exe";
                var outputAssemblyFileName = "SoftCube.Assembly_.exe";

                using (var assembly = AssemblyDefinition.ReadAssembly(inputAssemblyFileName, new ReaderParameters() { ReadSymbols = true, ReadWrite = false }))
                {
                    var module = assembly.Modules.Single(m => m.Name == "SoftCube.Assembly.exe");
                    var type   = module.Types.Single(t => t.FullName == "SoftCube.Class");

                    var @namespace = type.Namespace;
                    var baseType   = module.TypeSystem.Object;
                    var nestedType = new TypeDefinition(@namespace, "NestedClass", TypeAttributes.Class, baseType);
                    type.NestedTypes.Add(nestedType);

                    assembly.Write(outputAssemblyFileName, new WriterParameters() { WriteSymbols = true });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace.ToString());
            }
        }
    }
}
