// Copyright 2024 Ivan Zakarlyuka.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Reflector;

using System.Dynamic;
using System.Reflection;
using System.Runtime.InteropServices;

/// <summary>
/// Class that implements reflector.
/// </summary>
public static class Reflector
{
    /// <summary>
    /// Writes fields, methods and nested classes of class in {class}.cs file.
    /// </summary>
    /// <param name="someClass">Class  to be printed.</param>
    public static void PrintStructure(Type someClass)
    {
        var fileName = $"{someClass.Name}.cs";
        using var writer = new StreamWriter(fileName);
        WriteClass(writer, someClass, string.Empty);
    }

    /// <summary>
    /// Prints functions and methods that differ in two classes.
    /// </summary>
    /// <param name="a">First type.</param>
    /// <param name="b">second type.</param>
    public static void DiffClasses(Type a, Type b)
    {
        Console.WriteLine("Different methods:");
        var methodsA = new HashSet<string>();
        var methodsB = new HashSet<string>();
        foreach (var method in a.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            methodsA.Add(method.Name);
        }

        foreach (var method in b.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            methodsB.Add(method.Name);
        }

        methodsA.SymmetricExceptWith(methodsB);
        foreach (var method in methodsA)
        {
            Console.WriteLine(method);
        }

        var fieldsA = new HashSet<string>();
        var fieldsB = new HashSet<string>();
        foreach (var field in a.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            fieldsA.Add(field.Name);
        }

        foreach (var field in b.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            fieldsB.Add(field.Name);
        }

        Console.WriteLine("Different fields:");

        fieldsA.SymmetricExceptWith(fieldsB);
        foreach (var field in fieldsA)
        {
            Console.WriteLine(field);
        }
    }

    private static void WriteClass(StreamWriter writer, Type someClass, string intendation)
    {
        writer.WriteLineAsync($"{intendation}{(someClass.IsPublic ? "public" : "private")} class {someClass.Name}");
        writer.WriteAsync(intendation);
        writer.WriteLineAsync("{");
        intendation += "    ";
        foreach (var field in someClass.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Instance))
        {
            _ = writer.WriteLineAsync($"{intendation}{(field.IsPublic ? "public" : "private")} {(field.IsStatic ? "static" : string.Empty)}{field.FieldType.Name} {field.Name};");
        }

        foreach (var method in someClass.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Instance))
        {
            Console.WriteLine(method.Name);

            writer.WriteAsync($"{intendation}{(method.IsPublic ? "public" : "private")} {(method.IsStatic ? "static" : string.Empty)}{method.ReturnType.Name} {method.Name}(");
            var parameters = method.GetParameters();
            writer.WriteAsync(string.Join(", ", parameters.Select(parameter => $"{parameter.ParameterType} {parameter.Name}")));

            writer.WriteLineAsync(");");
        }

        foreach (var anotherClass in someClass.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
        {
            WriteClass(writer, anotherClass, intendation);
        }

        writer.WriteAsync($"{intendation.Substring(0, intendation.Length - 4)}");
        writer.WriteLineAsync("}");
    }
}
