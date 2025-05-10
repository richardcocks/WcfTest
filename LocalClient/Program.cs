using System.Diagnostics;
using RandomSource;

namespace LocalClient;

class Program
{
    static void Main(string[] args)
    {
        var sw = Stopwatch.StartNew();
        var rSource = new RandomProvider(32);
        
        using var r = rSource.GetEnumerator();
        
        var bag = new HashSet<long>();
        
        while (r.MoveNext())
        {
            if (bag.Add(r.Current.Value)) continue;
            
            sw.Stop();
            Console.WriteLine($"Found duplicate {r.Current.Sequence} - {r.Current.Value} in {sw.ElapsedMilliseconds}ms!");
            break;
        }
    }
}