namespace RiskAnalyser.DTO
{
  public sealed class ProductRecordRaw
  {
    public string Name { get; set; }
    public int OriginYear { get; set; }
    public int DevelopmentYear { get; set; }
    public decimal IncrementalValue { get; set; }
  }
}