using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace Keeper.Api.Models;

public class CalculatedStatistics
{
    private CalculatedStatistics(List<double> data)
    {
        var statistics = new DescriptiveStatistics(data);
        Count = statistics.Count;
        Sum = data.Sum(x => x);
        StandardDeviation = double.IsNaN(statistics.StandardDeviation) ? null : statistics.StandardDeviation;
        Mean = statistics.Mean;
        Minimum = statistics.Minimum;
        Maximum = statistics.Maximum;
        Median = data.Median();
        FirstQuartile = data.LowerQuartile();
        ThirdQuartile = data.UpperQuartile();
    }

    public static CalculatedStatistics Calculate(IEnumerable<double> data)
    {
        return new CalculatedStatistics(data.ToList());
    }

    public static CalculatedStatistics Calculate(IEnumerable<int?> data)
    {
        var list = data.Where(x => x.HasValue).Select(x => (double)x.Value).ToList();
        return list.Count == 0 ? null : new CalculatedStatistics(list);
    }

    public long Count { get; }
    
    public double Sum { get; }
    
    public double? StandardDeviation { get; }

    public double Mean { get; }

    public double Median { get; }

    public double Minimum { get; }
    
    public double Maximum { get; }

    public double FirstQuartile { get; }

    public double ThirdQuartile { get; }
}
