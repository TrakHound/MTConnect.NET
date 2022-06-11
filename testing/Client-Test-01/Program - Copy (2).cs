using System.Text.RegularExpressions;


//static long GetSequenceBottom(long n, int interval = 1000)
//{
//    if (interval > 0)
//    {
//        var x = n / interval;
//        var y = x * interval;

//        return y;
//    }

//    return 0;
//}

static long GetSequenceTop(long n, int interval = 1000)
{
    if (interval > 0)
    {
        var x = n / interval;
        x = x * interval;

        var y = 0;
        if (n % interval > 0) y = interval;

        return x + y;
    }

    return 0;
}

static IEnumerable<string> GetFiles(IEnumerable<string> files, long from, long to, int interval = 1000)
{
    var found = new List<string>();
    var regex = new Regex("observations_([0-9]*)");

    var x = new Dictionary<long, string>();
    foreach (var file in files)
    {
        var match = regex.Match(file);
        if (match.Success && match.Groups.Count > 1)
        {
            var sequence = long.Parse(match.Groups[1].Value);
            x.Add(sequence, file);
        }
    }

    long m = GetSequenceTop(from);
    var n = GetSequenceTop(to);

    string s;
    if (x.TryGetValue(m, out s)) found.Add(s);

    do
    {
        m += interval;

        if (x.TryGetValue(m, out s)) found.Add(s);
    }
    while (m < n);

    return found;
}

var files = new List<string>();
files.Add("observations_100");
files.Add("observations_200");
files.Add("observations_300");
files.Add("observations_400");
files.Add("observations_500");
files.Add("observations_600");
files.Add("observations_700");
files.Add("observations_800");
files.Add("observations_900");
files.Add("observations_1000");

var found = GetFiles(files, 1000, 3453, 100);
foreach (var file in found)
{
    Console.WriteLine(file);
}

Console.ReadLine();