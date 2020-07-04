using System.Collections.Generic;

namespace RiskAnalyser.DTO
{
  public sealed class Report
  {
    public Report(Dictionary<string, string> pData, int pOriginYear, int pDevelopmentYears)
    {
      Data = pData;
      OriginYear = pOriginYear;
      DevelopmentYears = pDevelopmentYears;
    }

    public Dictionary<string, string> Data { get; }
    public int OriginYear { get; }
    public int DevelopmentYears { get; }
  }
}