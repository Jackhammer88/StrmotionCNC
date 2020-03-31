using System.Text.RegularExpressions;

namespace RegexPrecompiler
{
    class Program
    {
        //static PrecompiledRegex.CNCRegex cNCRegex = new PrecompiledRegex.CNCRegex();
        static void Main()
        {
            Regex.CompileToAssembly(new RegexCompilationInfo[]
            {
                new RegexCompilationInfo(pattern: @"([GgMm][0-9]+[.,][0-9]+|[GgMm][0-9]+)|([AaBbCcUuVvWwXxYyZzRrIiJjKk][-+]?[0-9]+[.,]?\d*|[SsFfDdHh][0-9]+)|([Tt][0-9]+)",
                    RegexOptions.CultureInvariant, name: "CNCRegex", fullnamespace: "PrecompiledRegex", ispublic: true),
                new RegexCompilationInfo(pattern: @"([Gg][0-9]+[.,][0-9]+|[Gg][0-9]+)", RegexOptions.CultureInvariant, name: "CNCGCodes", fullnamespace: "PrecompiledRegex", ispublic: true),
                new RegexCompilationInfo(pattern: @"([Mm][0-9]+[.,][0-9]+|[Mm][0-9]+)", RegexOptions.CultureInvariant, name: "CNCMCodes", fullnamespace: "PrecompiledRegex", ispublic: true),
                new RegexCompilationInfo(pattern: @"([Tt][0-9]+[.,][0-9]+|[Tt][0-9]+)", RegexOptions.CultureInvariant, name: "CNCTCode", fullnamespace: "PrecompiledRegex", ispublic: true),
                new RegexCompilationInfo(pattern: @"([Ss][0-9]+[.,][0-9]+|[Ss][0-9]+)", RegexOptions.CultureInvariant, name: "CNCSCode", fullnamespace: "PrecompiledRegex", ispublic: true),
                new RegexCompilationInfo(pattern: @"([Ff][0-9]+[.,][0-9]+|[Ff][0-9]+)", RegexOptions.CultureInvariant, name: "CNCFCode", fullnamespace: "PrecompiledRegex", ispublic: true),
                new RegexCompilationInfo(pattern: @"([DdHh][-+]?[0-9]+[.,]?\d*)", RegexOptions.CultureInvariant, name: "CNCDHCode", fullnamespace: "PrecompiledRegex", ispublic: true),
                new RegexCompilationInfo(pattern: @"([AaBbCcUuVvWwXxYyZz][-+]?[0-9]+[.,]?\d*)", RegexOptions.CultureInvariant, name: "CNCCoordinates", fullnamespace: "PrecompiledRegex", ispublic: true),
                new RegexCompilationInfo(pattern: @"([IJKRijkr][-+]?[0-9]+[.,]?\d*)", RegexOptions.CultureInvariant, name: "CNCArcValues", fullnamespace: "PrecompiledRegex", ispublic: true),
            }, new System.Reflection.AssemblyName("CNCPrecompiledRegex"));


            //Regex regexInterpreted = new Regex(@"([GgMm][0-9]+[.,][0-9]+|[GgMm][0-9]+)|([AaBbCcUuVvWwXxYyZzRrIiJjKk][-+]?[0-9]+[.,]?\d*|[SsFfDdHh][0-9]+)|([Tt][0-9]+)");
            //Regex regexCompiled = new Regex(@"([GgMm][0-9]+[.,][0-9]+|[GgMm][0-9]+)|([AaBbCcUuVvWwXxYyZzRrIiJjKk][-+]?[0-9]+[.,]?\d*|[SsFfDdHh][0-9]+)|([Tt][0-9]+)", RegexOptions.Compiled);
            //Console.Write("Interpreted regex: ");
            //stopwatch.Start();
            //using(var outFile = new StreamWriter(@"C:\Users\Alexe\Documents\6V-purecopy1.cnc"))
            //using (var file = new StreamReader(@"C:\Users\Alexe\Documents\6V.cnc"))
            //{
            //    while(!file.EndOfStream)
            //    {
            //        foreach(var match in regexInterpreted.Matches(file.ReadLine()))
            //        {
            //            outFile.Write($"{match} ");
            //        }
            //        outFile.Write(Environment.NewLine);
            //    }
            //}
            //stopwatch.Stop();
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);
            //stopwatch.Reset();
            //Console.Write("Compiled regex: ");
            //stopwatch.Start();
            //using (var outFile = new StreamWriter(@"C:\Users\Alexe\Documents\6V-purecopy2.cnc"))
            //using (var file = new StreamReader(@"C:\Users\Alexe\Documents\6V.cnc"))
            //{
            //    while (!file.EndOfStream)
            //    {
            //        foreach (var match in regexCompiled.Matches(file.ReadLine()))
            //        {
            //            outFile.Write($"{match} ");
            //        }
            //        outFile.Write(Environment.NewLine);
            //    }
            //}
            //stopwatch.Stop();
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);
            //stopwatch.Reset();


            //Stopwatch stopwatch = new Stopwatch();
            //Console.Write("Precompiled regex: ");
            //stopwatch.Start();
            //using (var outFile = new StreamWriter(@"C:\Users\Alexe\Documents\6V-purecopy5.cnc"))
            //using (var file = new StreamReader(@"C:\Users\Alexe\Documents\6V.cnc"))
            //{
            //    while (!file.EndOfStream)
            //    {
            //        foreach (var match in cNCRegex.Matches(file.ReadLine()))
            //        {
            //            outFile.Write($"{match} ");
            //        }
            //        outFile.Write(Environment.NewLine);
            //    }
            //}
            //stopwatch.Stop();
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);
            //Console.ReadLine();
        }
    }
}
