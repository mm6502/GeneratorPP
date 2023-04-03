using System;
using System.IO;
using System.Threading.Tasks;
using GeneratorPP.Core;
using GeneratorPP.Core.Implementation;
using GeneratorPP.WPF.Misc;

namespace GeneratorPP.WPF.Mvvm.Page2;

/// <summary>
/// Page2: generating output.
/// </summary>
public partial class Page2ViewModel : BaseViewModel
{
    /// <summary>
    /// Construct the default input template filename.
    /// </summary>
    public string GetOutputTemplateFileName()
    {
        return $"{Constants.OutputTemplateFileName}.{Constants.TemplateFileExtension}";
    }

    /// <summary>
    /// Returns path to the template for output file.
    /// First tries a file beside executable. If it does not exist, saves the embedded one beside the
    /// executable and returns its path.
    /// </summary>
    public string GetOutputTemplateFilePath()
    {
        // try file beside the application
        var file = Path.Combine(AppContext.BaseDirectory, nameof(Resources), GetOutputTemplateFileName());
        if (File.Exists(file))
            return file;

        // otherwise use the embedded one
        File.WriteAllBytes(file, Resources.platobne_predpisy);

        return file;
    }

    /// <summary>
    /// Processes the input file and generates the output file.
    /// </summary>
    public async Task GenerateAsync(string inputFilePath)
    {
        var bySquareEncoder  = new BySquareInternalEncoder();
        var imageManipulator = new ImageManipulator();
        var progressReporter = new Progress<int>((i) => CurrentProgress = i);
        var excelManipulator = new ExcelManipulator(bySquareEncoder, imageManipulator, progressReporter);

        (this.PaymentPurpose, var payments) = await Task.Run(() => excelManipulator.ReadPaymentList(inputFilePath));

        var outputTemplateFilePath = GetOutputTemplateFilePath();
        var outputFilePath         = Path.GetTempFileName();

        this.OutputFilePath = await Task.Run(
            () => excelManipulator.CreateOutputFile(outputTemplateFilePath, outputFilePath, this.PaymentPurpose, payments)
        );

        await Task.Delay(TimeSpan.FromSeconds(1));

        NextStep.Invoke(this, EventArgs.Empty);
    }
}
