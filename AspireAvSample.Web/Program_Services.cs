using AspireAvSample.Web;
using nClam;

public static class Program_Services 
{

  private static Uri ClamUri;

  public static WebApplication? MapServices(this WebApplication? app)
  {

    var clamConfig = app.Configuration["services:antivirus:http:0"];
    if (string.IsNullOrEmpty(clamConfig)) throw new Exception("Missing antivirus configuration");
    ClamUri = new Uri(clamConfig);

    app.MapPost("/upload", async Task<IResult> (HttpRequest request) =>
    {

      var form = await request.ReadFormAsync();

      if (form.Files.Any() == false)
        return Results.BadRequest("There are no files");

      var file = form.Files.FirstOrDefault();

      using var stream = file.OpenReadStream();

      var clam = new ClamClient(ClamUri.Host, ClamUri.Port);
      var scanResult = await clam.SendAndScanFileAsync(stream);

      var result = new UploadResult() {
        ErrorCode = scanResult.Result switch {
          ClamScanResults.VirusDetected => 100,
          ClamScanResults.Error => 1,
          _ => 0
        },
        ErrorMessage = scanResult.Result switch { 
          ClamScanResults.VirusDetected => $"Virus {scanResult.InfectedFiles.First().VirusName} detected",
          ClamScanResults.Error => $"Error with virus scanning.  Please try again",
          _ => string.Empty
        },
        Uploaded = (scanResult.Result == ClamScanResults.Clean),
        FileName = file.FileName,
        StoredFileName = file.FileName
      };

      return Results.Ok(new[] { result });
    });

    return app;

  }

}