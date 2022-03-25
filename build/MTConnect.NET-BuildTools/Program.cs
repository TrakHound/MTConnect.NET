using System.Text.RegularExpressions;

var dir = @"D:\TrakHound\Source-Code\MTConnect.NET\src\MTConnect.NET\Observations\Samples\Values";
var files = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);

var regex = new Regex(@".*(namespace\sMTConnect\.Streams\.Samples).*");

foreach (var file in files)
{
    var contents = File.ReadAllText(file);

    var s = regex.Replace(contents, "namespace MTConnect.Observations.Samples.Values");
    File.WriteAllText(file, s);
    Console.WriteLine(file);
    //Console.ReadLine();
}