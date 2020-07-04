using RiskAnalyser.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RiskAnalyser
{
  /// <summary>
  /// IFile is interface to abstract out reading/wriring of report files.
  /// </summary>
  public interface IFile
  {
    IList<string> ReadAllLines(string path);

    void WriteAllLines(string path, IList<string> data);
  }

  /// <summary>
  /// real implementation of IFile interace
  /// </summary>
  public class FileAdapter : IFile
  {
    public IList<string> ReadAllLines(string path)
    {
      return File.ReadAllLines(path);
    }

    public void WriteAllLines(string path, IList<string> data)
    {
      File.WriteAllLines(path, data);
    }
  }

  /// <summary>
  /// ReportGenerator is class that read the input validate it and develop the report and save the report.
  ///
  /// </summary>
  public class ReportGenerator
  {
    private readonly IFile file;

    public ReportGenerator(IFile pfile)
    {
      file = pfile;
    }

    public IList<ProductRecordRaw> ReadFile(string path)
    {
      IList<string> lineData;
      try
      {
        lineData = file.ReadAllLines(path);
      }
      catch (Exception ex)
      {
        throw new ApplicationException("Reading input file failed", ex);
      }

      if (lineData.Count < 2)
      {
        throw new ApplicationException("Invalid file");
      }

      IList<ProductRecordRaw> productRecordRaws = new List<ProductRecordRaw>();
      //remove the header.
      lineData.Skip(1).All(eachLine =>
      {
        string[] line = eachLine.Split(',');
        if (line.Count() < 4 || line.Count() > 4)
        {
          throw new ApplicationException(string.Format("Invalid data {0}", eachLine));
        }

        if (!int.TryParse(line[1], out int origin))
        {
          throw new ApplicationException(string.Format("Invalid origin at line {0}", eachLine));
        }
        if (!int.TryParse(line[2], out int development))
        {
          throw new ApplicationException(string.Format("Invalid development year at line {0}", eachLine));
        }
        if (!decimal.TryParse(line[3], out decimal increment))
        {
          throw new ApplicationException(string.Format("Invalid increment value at line {0}", eachLine));
        }
        productRecordRaws.Add(new ProductRecordRaw()
        {
          Name = line[0],
          OriginYear = origin,
          DevelopmentYear = development,
          IncrementalValue = increment
        });
        return true;
      });
      return productRecordRaws;
    }

    public Report GetReport(IList<ProductRecordRaw> data)
    {
      var developmentYears = (from d in data
                              select new
                              {
                                MinYear = data.Min(z => z.OriginYear),
                                MaxYear = data.Max(z => z.DevelopmentYear)
                              }).Distinct();

      IEnumerable<string> products = data.GroupBy(p => p.Name).Select(g => g.First().Name);

      List<Tuple<int, int>> reportYears = new List<Tuple<int, int>>();

      for (int outerIndex = developmentYears.First().MinYear; outerIndex < developmentYears.First().MaxYear + 1; outerIndex++)
      {
        for (int innnerIndex = outerIndex; innnerIndex < developmentYears.First().MaxYear + 1; innnerIndex++)
        {
          reportYears.Add(new Tuple<int, int>(outerIndex, innnerIndex));
        }
      }

      IEnumerable<ProductRecordRaw> templateReport = from year in reportYears
                                                     from prod in products
                                                     select new ProductRecordRaw()
                                                     {
                                                       Name = prod,
                                                       OriginYear = year.Item1,
                                                       IncrementalValue = 0,
                                                       DevelopmentYear = year.Item2,
                                                     };

      IEnumerable<ProductRecordRaw> joinTemplateAndRealData = from e in templateReport
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

      var runningTotals = from d in joinTemplateAndRealData
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
                            })
                          };

      Dictionary<string, string> reportDict = new Dictionary<string, string>();
      products.All(a => { reportDict.Add(a, ""); return true; });

      runningTotals.All(grp =>
      {
        grp.running.ForEach(run => reportDict[grp.Key.Name] += string.Format(",{0}", run.ToString("0.##")));
        return true;
      });

      Report report = new Report(reportDict, developmentYears.First().MinYear, developmentYears.First().MaxYear - developmentYears.First().MinYear + 1);
      return report;
    }

    public void WriteReport(Report report, string filePath)
    {
      try
      {
        List<string> reportLines = new List<string>();
        report.Data.Keys.All(key =>
        {
          reportLines.Add(string.Format("{0}{1}", key, report.Data[key]));
          return true;
        });
        reportLines.Insert(0, string.Format("{0},{1}", report.OriginYear, report.DevelopmentYears));
        file.WriteAllLines(filePath, reportLines);
      }
      catch (Exception ex)
      {
        throw new ApplicationException("Saving report failed", ex);
      }
    }
  }
}