using AspireAvSample.AppHost;
using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);

var av = builder.AddClamAv("antivirus")
	.WithDataVolume("clamavdb"); // persist the av database on a volume or bindmount

builder.AddProject<Projects.AspireAvSample_Web>("webfrontend")
		.WithExternalHttpEndpoints()
		.WithReference(av);

builder.Build().Run();
