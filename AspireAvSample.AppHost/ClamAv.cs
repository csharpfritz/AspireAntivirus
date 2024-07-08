using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireAvSample.AppHost;


public class ClamAvResource(string name) : ContainerResource(name), IResourceWithServiceDiscovery
{

	private EndpointReference? _httpReference;

	public EndpointReference HttpEndpoint =>
			_httpReference ??= new(this, "http");

}

public static class ClamAvResourceBuilderExtensions
{
	public static IResourceBuilder<ClamAvResource> AddClamAv(
			this IDistributedApplicationBuilder builder,
			string name,
			int? httpPort = null)
	{
		var resource = new ClamAvResource(name);

		return builder.AddResource(resource)
									.WithImage("clamav/clamav")
									.WithImageRegistry("docker.io")
									.WithImageTag("latest")
									.WithEnvironment("CLAMAV_NO_FRESHCLAMD", "true")
									.WithHttpEndpoint(port: httpPort, targetPort: 3310, name: "http");

	}
}
