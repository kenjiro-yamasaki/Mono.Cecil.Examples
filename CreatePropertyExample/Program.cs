using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Logging;
using System;
using System.Linq;

namespace CreatePropertyExample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var inputAssemblyFileName = "SoftCube.Assembly.exe";
                var outputAssemblyFileName = "SoftCube.Assembly_.exe";

                using (var assembly = AssemblyDefinition.ReadAssembly(inputAssemblyFileName, new ReaderParameters() { ReadSymbols = true, ReadWrite = false }))
                {
                    var module = assembly.Modules.Single(m => m.Name == "SoftCube.Assembly.exe");
                    var type = module.Types.Single(t => t.FullName == "SoftCube.Class");

                    var propertyTypeReference = module.ImportReference(typeof(string));
                    var backingStore = new FieldDefinition("property", FieldAttributes.Private, propertyTypeReference);
                    type.Fields.Add(backingStore);

                    var property = new PropertyDefinition("Property", PropertyAttributes.None, backingStore.FieldType);
                    property.HasThis = !backingStore.IsStatic;

                    var attributes = MethodAttributes.Private | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
                    if (backingStore.IsStatic)
                    {
                        attributes |= MethodAttributes.Static;
                    }

                    //
                    {
                        var getter = new MethodDefinition("get_" + property.Name, attributes, backingStore.FieldType);
                        getter.SemanticsAttributes = MethodSemanticsAttributes.Getter;

                        var processor = getter.Body.GetILProcessor();
                        if (backingStore.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldsfld, backingStore);
                            processor.Emit(OpCodes.Ret);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg_0);
                            processor.Emit(OpCodes.Ldfld, backingStore);
                            processor.Emit(OpCodes.Ret);
                        }

                        type.Methods.Add(getter);
                        property.GetMethod = getter;
                    }

                    //
                    {
                        var setter = new MethodDefinition("set_" + property.Name, attributes, module.TypeSystem.Void);
                        setter.SemanticsAttributes = MethodSemanticsAttributes.Setter;

                        ParameterDefinition valueArg;
                        setter.Parameters.Add(valueArg = new ParameterDefinition(backingStore.FieldType));

                        var processor = setter.Body.GetILProcessor();
                        if (backingStore.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg_0);
                            processor.Emit(OpCodes.Stsfld, backingStore);
                            processor.Emit(OpCodes.Ret);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg_0);
                            processor.Emit(OpCodes.Ldarg, valueArg);
                            processor.Emit(OpCodes.Stfld, backingStore);
                            processor.Emit(OpCodes.Ret);
                        }

                        type.Methods.Add(setter);

                        property.SetMethod = setter;
                    }

                    type.Properties.Add(property);

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
