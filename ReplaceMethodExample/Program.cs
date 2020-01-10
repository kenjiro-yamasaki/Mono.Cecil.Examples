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

                    var method = type.Methods.Single(m => m.Name == "Method");

                    // 新たなメソッド (Method?) を作成し、元々のメソッド (Method) の内容を移動します。
                    var movedMethod = new MethodDefinition(method.Name + "?", method.Attributes, method.ReturnType);
                    movedMethod.Body = method.Body;
                    type.Methods.Add(movedMethod);

                    // 元々のメソッド (Method) の内容を、新たなメソッド (Method?) を呼び出すコードに書き換えます。
                    method.Body = new MethodBody(method);

                    var processor = method.Body.GetILProcessor();
                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(OpCodes.Call, movedMethod));
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
