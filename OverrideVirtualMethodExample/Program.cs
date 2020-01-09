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
                Logger.Info("Hello");

                var inputAssemblyFileName = "SoftCube.Assembly.exe";
                var outputAssemblyFileName = "SoftCube.Assembly_.exe";

                using (var assembly = AssemblyDefinition.ReadAssembly(inputAssemblyFileName, new ReaderParameters() { ReadSymbols = true, ReadWrite = false }))
                {
                    var module     = assembly.Modules.Single(m => m.Name == "SoftCube.Assembly.exe");
                    var baseType   = module.Types.Single(t => t.FullName == "SoftCube.Class");
                    var baseMethod = baseType.Methods.Single(m => m.Name == "VirtualMethod");

                    var @namespace = baseType.Namespace;
                    var derivedClass = new TypeDefinition(@namespace, "DerivedClass?", TypeAttributes.Class, baseType);
                    module.Types.Add(derivedClass);

                    var derivedMethod = new MethodDefinition(baseMethod.Name, (baseMethod.Attributes | MethodAttributes.CheckAccessOnOverride) & ~MethodAttributes.NewSlot, baseMethod.ReturnType);
                    var processor = derivedMethod.Body.GetILProcessor();
                    var instructions = derivedMethod.Body.Instructions;

                    instructions.Insert(0, processor.Create(OpCodes.Ldstr, "Hello!"));
                    instructions.Insert(1, processor.Create(OpCodes.Call, module.ImportReference(typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }))));
                    instructions.Add(processor.Create(OpCodes.Ret));

                    derivedClass.Methods.Add(derivedMethod);

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
