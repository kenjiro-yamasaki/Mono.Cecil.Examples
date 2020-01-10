using Mono.Cecil.Cil;
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

                    var method = new MethodDefinition("Test", MethodAttributes.Private | MethodAttributes.Static, module.TypeSystem.Void);
                    type.Methods.Add(method);

                    var processor = method.Body.GetILProcessor();

                    processor.Append(processor.Create(OpCodes.Ldstr, "Hello!"));
                    processor.Append(processor.Create(OpCodes.Call, module.ImportReference(typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }))));
                    processor.Append(processor.Create(OpCodes.Ret));

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
