using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiskAnalyser
{
    public interface IData
    {
        IList<string> GetReport(IList<string> data);
    }
    public interface IClaim
    {
        Report GetReport(IList<ProductRecordRaw> data);
    }
    
    public class ProductRecordRaw
    {
        public string Name { get; set; }
        public int OriginYear { get; set; }
        public int DevelopmentYear { get; set; }
        public decimal IncrementalValue { get; set; }
    }

    public sealed class Report
    {
        public Report(Dictionary<string, string> pData, int pOriginYear,int pDevelopmentYears)
        {
            Data = pData;
            OriginYear = pOriginYear;
            DevelopmentYears = pDevelopmentYears;
        }
        public  Dictionary<string,string> Data { get;  }
        public int OriginYear { get; }
        public int DevelopmentYears { get; }
    }

    public sealed class ClaimRingfence: IClaim
    { 
        public Report GetReport(IList<ProductRecordRaw> data)
        {
            var developmentYears = (from d in data
                                      select new
                                      {
                                          MinYear = data.Min(z => z.OriginYear),
                                          MaxYear = data.Max(z => z.DevelopmentYear)
                                      }).Distinct();

            var products = data.GroupBy(p => p.Name).Select(g => g.First().Name);

            var reportYears = new List<Tuple<int, int>>();

            for (int outerIndex = developmentYears.First().MinYear; outerIndex < developmentYears.First().MaxYear + 1; outerIndex++)
            {
                for (int innnerIndex = outerIndex; innnerIndex < developmentYears.First().MaxYear + 1; innnerIndex++)
                    reportYears.Add(new Tuple<int, int>(outerIndex, innnerIndex));
            }

    var templateReport = from year in reportYears
                         from prod in products
                         select new ProductRecordRaw()
                         {
                            Name = prod,
                            OriginYear = year.Item1,
                            IncrementalValue = 0,
                            DevelopmentYear = year.Item2,
                         };

    var fillMissingYears = from e in templateReport
                           join realData in data on
                                new { e.Name, e.OriginYear, e.DevelopmentYear } equals
                                new { realData.Name, realData.OriginYear, realData.DevelopmentYear } into g
                            from realData in g.DefaultIfEmpty()
                            select new ProductRecordRaw()
                            {
                                Name = e.Name,
                                DevelopmentYear = realData == null ? e.DevelopmentYear : realData.DevelopmentYear,
                                OriginYear = realData == null ? e.OriginYear : realData.OriginYear,
                                IncrementalValue = realData == null ? 0 : realData.IncrementalValue
                            };

     var runningTotals = from d in fillMissingYears
                         group d by new
                         {
                            d.Name,
                            d.OriginYear
                         } into grp
                         select new
                         {
                             Key = grp.Key,                           
                             running = grp.Aggregate(new List<decimal>(), (a, i) =>
                             {
                                a.Add(a.Count() == 0 ? i.IncrementalValue : a.Last() + i.IncrementalValue);
                                return a;
                             }),
                         };

            Dictionary<string, string> reportDict = new Dictionary<string, string>();
            products.ToList().ForEach( a => reportDict.Add(a, "") );

            runningTotals.ToList().ForEach(grp =>      
                          grp.running.ForEach(a => reportDict[grp.Key.Name] += string.Format("{0},", a)));

            reportDict.Keys.ToList().ForEach(a => reportDict[a] = reportDict[a].TrimEnd(','));

            Report report = new Report(reportDict, developmentYears.First().MinYear, developmentYears.First().MaxYear - developmentYears.First().MinYear+1);
            return report;
          
        }      

    }
}
