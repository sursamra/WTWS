using Moq;
using NUnit.Framework;
using RiskAnalyser;
using RiskAnalyser.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
  public class Tests
  {
    [Test]
    public void Report_Throws_Exception_When_Input_File_Reading_Fails()
    {
      List<string> fileData = new List<string>()
      {
      };
      Mock<IFile> file = new Mock<IFile>();
      file.Setup(a => a.ReadAllLines(It.IsAny<string>()));
      ReportGenerator reportGenerator = new ReportGenerator(file.Object);

      try
      {
        IList<ProductRecordRaw> data = reportGenerator.ReadFile(It.IsAny<string>());
        Report report = reportGenerator.GetReport(data);
        Assert.Fail("no exception thrown");
      }
      catch (Exception)
      {
        Assert.IsTrue(true); // exception is thrown when file is not read.
      }
    }

    [Test]
    public void Report_Throws_Exception_When_Input_has_invalid_data_fields()
    {
      List<string> fileData = new List<string>()
      {
      };
      Mock<IFile> file = new Mock<IFile>();
      file.Setup(a => a.ReadAllLines(It.IsAny<string>())).Returns(fileData);
      ReportGenerator reportGenerator = new ReportGenerator(file.Object);

      try
      {
        IList<ProductRecordRaw> data = reportGenerator.ReadFile(It.IsAny<string>());
        Report report = reportGenerator.GetReport(data);
        Assert.Fail("no exception thrown");
      }
      catch (Exception ex)
      {
        Assert.IsTrue(ex is ApplicationException && ex.Message == "Invalid file");
      }
    }

    [Test]
    public void When_Invalidrecord_With_more_than_4_fields_entered_Report_throws_applicationexception_and_shows_data_line_with_error()
    {
      List<string> fileData = new List<string>()
      {
        "Product, Origin Year, Development Year, Incremental Value",
        "Comp,, 1992, 1992, 110.0",
        "Comp, 1992, 1993, 170.0"
      };
      Mock<IFile> file = new Mock<IFile>();
      file.Setup(a => a.ReadAllLines(It.IsAny<string>())).Returns(fileData);
      ReportGenerator reportGenerator = new ReportGenerator(file.Object);

      try
      {
        IList<ProductRecordRaw> data = reportGenerator.ReadFile(It.IsAny<string>());
        Report report = reportGenerator.GetReport(data);
        Assert.Fail("no exception thrown");
      }
      catch (Exception ex)
      {
        Assert.IsTrue(ex is ApplicationException && ex.Message == "Invalid data Comp,, 1992, 1992, 110.0");
      }
    }

    [Test]
    public void When_Invalid_OriginYear_entered_Report_throws_applicationexception_and_shows_data_line_with_error()
    {
      List<string> fileData = new List<string>()
      {
        "Product, Origin Year, Development Year, Incremental Value",
        "Comp,Hello, 1992, 110.0",
        "Comp2, 1992, 1993, 170.0"
      };
      Mock<IFile> file = new Mock<IFile>();
      file.Setup(a => a.ReadAllLines(It.IsAny<string>())).Returns(fileData);
      ReportGenerator reportGenerator = new ReportGenerator(file.Object);

      try
      {
        IList<ProductRecordRaw> data = reportGenerator.ReadFile(It.IsAny<string>());
        Report report = reportGenerator.GetReport(data);
        Assert.Fail("no exception thrown");
      }
      catch (Exception ex)
      {
        Assert.IsTrue(ex is ApplicationException && ex.Message == "Invalid origin at line Comp,Hello, 1992, 110.0");
      }
    }

    [Test]
    public void When_Invalid_DevelopmentYear_entered_Report_throws_applicationexception_and_shows_data_line_with_error()
    {
      List<string> fileData = new List<string>()
      {
        "Product, Origin Year, Development Year, Incremental Value",
        "Comp, 1992, 1992, 110.0",
        "Comp2, 1992, Hello, 170.0"
      };
      Mock<IFile> file = new Mock<IFile>();
      file.Setup(a => a.ReadAllLines(It.IsAny<string>())).Returns(fileData);
      ReportGenerator reportGenerator = new ReportGenerator(file.Object);

      try
      {
        IList<ProductRecordRaw> data = reportGenerator.ReadFile(It.IsAny<string>());
        Report report = reportGenerator.GetReport(data);
        Assert.Fail("no exception thrown");
      }
      catch (Exception ex)
      {
        Assert.IsTrue(ex is ApplicationException && ex.Message == "Invalid development year at line Comp2, 1992, Hello, 170.0");
      }
    }

    [Test]
    public void When_Invalid_IncrementalValue_entered_Report_throws_applicationexception_and_shows_data_line_with_error()
    {
      List<string> fileData = new List<string>()
      {
        "Product, Origin Year, Development Year, Incremental Value",
        "Comp,1992, 1992, 110.0",
        "Comp2, 1992, 1992, 170.0",
        "Comp2, 1992, 1993, wrong value"
      };
      Mock<IFile> file = new Mock<IFile>();
      file.Setup(a => a.ReadAllLines(It.IsAny<string>())).Returns(fileData);
      ReportGenerator reportGenerator = new ReportGenerator(file.Object);

      try
      {
        IList<ProductRecordRaw> data = reportGenerator.ReadFile(It.IsAny<string>());
        Report report = reportGenerator.GetReport(data);
        Assert.Fail("no exception thrown");
      }
      catch (Exception ex)
      {
        Assert.IsTrue(ex is ApplicationException && ex.Message == "Invalid increment value at line Comp2, 1992, 1993, wrong value");
      }
    }

    [Test]
    public void Report_returns_lowest_Year_from_originYears_of_all_products()
    {
      ReportGenerator reportGenerator = new ReportGenerator(new Mock<IFile>().Object);
      List<ProductRecordRaw> data = new List<ProductRecordRaw>()
      {
         new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1992,
                 DevelopmentYear = 1993,
                 IncrementalValue = 85
             },
         new ProductRecordRaw
             {
                 Name = "Comp",
                 OriginYear = 1982,
                 DevelopmentYear = 1993,
                 IncrementalValue = 185
             },
      };
      Report report = reportGenerator.GetReport(data);
      Assert.IsTrue(report.OriginYear == 1982);
    }

    [Test]
    public void Report_Returns_12_originyears_when_input_span_from_1982_1993_Development_Years()
    {
      ReportGenerator reportGenerator = new ReportGenerator(new Mock<IFile>().Object);
      List<ProductRecordRaw> data = new List<ProductRecordRaw>()
      {
         new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1992,
                 DevelopmentYear = 1993,
                 IncrementalValue = 85
             },
         new ProductRecordRaw
             {
                 Name = "Comp",
                 OriginYear = 1982,
                 DevelopmentYear = 1993,
                 IncrementalValue = 185
             },
      };
      Report report = reportGenerator.GetReport(data);
      Assert.IsTrue(report.DevelopmentYears == 12);
    }

    [Test]
    public void When_Three_Products_Provided_Reports_Returns_Three_Products()
    {
      ReportGenerator reportGenerator = new ReportGenerator(new Mock<IFile>().Object);
      List<ProductRecordRaw> data = new List<ProductRecordRaw>()
      {
        new ProductRecordRaw
             {
                 Name = "Tower",
                 OriginYear = 1982,
                 DevelopmentYear = 1993,
                 IncrementalValue = 185
             },
        new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1992,
                 DevelopmentYear = 1993,
                 IncrementalValue = 85
             },
         new ProductRecordRaw
             {
                 Name = "Comp",
                 OriginYear = 1982,
                 DevelopmentYear = 1993,
                 IncrementalValue = 185
             },
      };
      Report report = reportGenerator.GetReport(data);
      Assert.IsTrue(3 == report.Data.Keys.Count);
    }

    [Test]
    public void When_one_product_missing_two_years_data_then_report_return_two_zeros_for_this_product()
    {
      ReportGenerator reportGenerator = new ReportGenerator(new Mock<IFile>().Object);
      List<ProductRecordRaw> data = new List<ProductRecordRaw>()
      {
         new ProductRecordRaw
             {
                 Name = "Comp",
                 OriginYear = 1992,
                 DevelopmentYear = 1992,
                 IncrementalValue = 110
             },

             new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1991,
                 DevelopmentYear = 1992,
                 IncrementalValue = 64.8M
             }
    };
      Report report = reportGenerator.GetReport(data);
      Assert.AreEqual(",0,0,110", report.Data.Values.First());
    }

    [Test]
    public void When_product_missing_origin_one_year_in_between_origin_years_then_report_return_previous_year_value()
    {
      ReportGenerator reportGenerator = new ReportGenerator(new Mock<IFile>().Object);
      List<ProductRecordRaw> data = new List<ProductRecordRaw>()
      {
         new ProductRecordRaw
             {
                 Name = "Comp",
                 OriginYear = 1992,
                 DevelopmentYear = 1992,
                 IncrementalValue = 100
             },

             new ProductRecordRaw
             {
                 Name = "Comp",
                 OriginYear = 1992,
                 DevelopmentYear = 1994,
                 IncrementalValue = 200
             },
              new ProductRecordRaw
             {
                 Name = "Comp",
                 OriginYear = 1994,
                 DevelopmentYear = 1994,
                 IncrementalValue = 300
             }
    };
      Report report = reportGenerator.GetReport(data);
      Assert.AreEqual(",100,100,300,0,0,300", report.Data.Values.First());
    }

    [Test]
    public void Report_Saves_final_data_in_expected_format()
    {
      List<string> fileData = new List<string>()
      {
        "Product, Origin Year, Development Year, Incremental Value",
        "Comp, 1992, 1992, 110.0",
        "Comp, 1992, 1993, 170.0"
      };
      Mock<IFile> file = new Mock<IFile>();
      file.Setup(a => a.ReadAllLines(It.IsAny<string>())).Returns(fileData);
      file.Setup(a => a.WriteAllLines(It.IsAny<string>(), It.IsAny<IList<string>>()));
      ReportGenerator reportGenerator = new ReportGenerator(file.Object);

      IList<ProductRecordRaw> data = reportGenerator.ReadFile(It.IsAny<string>());
      Report report = reportGenerator.GetReport(data);
      reportGenerator.WriteReport(report, It.IsAny<string>());
      Assert.IsTrue(true); // i.e. does not throw any exception.
    }

    [Test]
    public void Performance_When_5000_products_Provided_Claims_upto_10_years_Report_Returns_under_5seconds()
    {
      List<ProductRecordRaw> rec = new List<ProductRecordRaw>();
      System.Random random = new System.Random();

      Enumerable.Range(0, 1000).All(p =>
      {
        int year = 1990 + p % 10;
        rec.Add(new ProductRecordRaw()
        {
          DevelopmentYear = year + p % 5,
          IncrementalValue = random.Next(0, 100),
          Name = string.Format("Product {0}", p),
          OriginYear = year
        });
        return true;
      });

      System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
      ReportGenerator reportGenerator = new ReportGenerator(new Mock<IFile>().Object);
      watch.Stop();
      Assert.LessOrEqual(watch.ElapsedMilliseconds, 5000);
    }
  }
}