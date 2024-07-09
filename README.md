# AspireAntivirus

A proof of concept showing how to add ClamAV antivirus detection to a .NET Aspire application.

## Purpose

At some point in our applications, we want our users to be able to submit content for others to work with.  This might be text and it might be some files.  If you want to enable file uploads for your projects, there are some great samples on Microsoft Learn.  However, they ALL [recommend that you inspect and test the files for malware](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-8.0#security-considerations) before accepting and republishing their content.

In this sample, we've taken an original concept published by [Jeroen Verhaeghe on Medium](https://medium.com/@jeroenverhaeghe/building-a-malware-and-antivirus-scanner-in-net-core-e4309f2d429c) and extended it to work with .NET Aspire.  In Jeroen's original article, the [ClamAV antivirus scanner](https://www.clamav.net/) was introduced with the [nClam](https://github.com/tekmaven/nClam) library from [Ryan Hoffman](https://github.com/tekmaven).  Jeroen runs the ClamAV scanner in a Docker container, and in reading that I thought - why not let .NET Aspire manage the container for us?

That's what I did here. I've added a custom `ClamAvResource` to AppHost that will start the latest ClamAV container and pass the address for that container into the web project.

AppHost/Program.cs
```csharp
var builder = DistributedApplication.CreateBuilder(args);

var av = builder.AddClamAv("antivirus");

builder.AddProject<Projects.AspireAvSample_Web>("webfrontend")
		.WithExternalHttpEndpoints()
		.WithReference(av);

builder.Build().Run();
```

We can then scan uploaded content as shown in Program_Services.cs:

```csharp

var clamConfig = app.Configuration.GetConnectionString("antivirus");
if (string.IsNullOrEmpty(clamConfig)) throw new Exception("Missing antivirus configuration");
ClamUri = new Uri(clamConfig);

...

var clam = new ClamClient(ClamUri.Host, ClamUri.Port);
var scanResult = await clam.SendAndScanFileAsync(stream);

...

ErrorMessage = scanResult.Result switch { 
	ClamScanResults.VirusDetected => $"Virus {scanResult.InfectedFiles.First().VirusName} detected",
	ClamScanResults.Error => $"Error with virus scanning.  Please try again",
	_ => string.Empty
},
```

You can test the scanner with a standard antivirus sample available for researchers at https://www.eicar.org/download-anti-malware-testfile/  

Please read ALL warnings carefully when downloading antivirus samples to test with.  We are not responsible for your download of samples to test against.
