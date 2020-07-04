using RiskAnalyser;

namespace ConsoleApp1
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      string inputFile = @"D:\Temp\inputFile.txt";
      string outputFile = @"D:\Temp\outputFile.txt";
      FileAdapter fileAdapter = new FileAdapter();
      ReportGenerator reportGenerator = new ReportGenerator(fileAdapter);
      reportGenerator.WriteReport
        (reportGenerator.GetReport
          (reportGenerator.ReadFile(inputFile)), outputFile);
    }
  }
}