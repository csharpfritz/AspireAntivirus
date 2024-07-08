using AspireAvSample.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var av = builder.AddClamAv("antivirus");

builder.AddProject<Projects.AspireAvSample_Web>("webfrontend")
		.WithExternalHttpEndpoints()
		.WithReference(av);

builder.Build().Run();
