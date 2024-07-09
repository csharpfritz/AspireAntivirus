namespace AspireAvSample.AppHost;

public class ClamAvResource(string name) : ContainerResource(name), IResourceWithConnectionString
{

	internal const string PrimaryEndpointName = "tcp";
	private EndpointReference? _primaryEndpoint;

	public EndpointReference PrimaryEndpoint => _primaryEndpoint ??= new(this, PrimaryEndpointName);
	public ReferenceExpression ConnectionStringExpression =>
		ReferenceExpression.Create(
			$"tcp://{PrimaryEndpoint.Property(EndpointProperty.Host)}:{PrimaryEndpoint.Property(EndpointProperty.Port)}"
		);
}

public static class ClamAvResourceBuilderExtensions
{
	public static IResourceBuilder<ClamAvResource> AddClamAv(
			this IDistributedApplicationBuilder builder,
			string name,
			int? port = null)
	{
		var resource = new ClamAvResource(name);

		return builder.AddResource(resource)
									.WithImage("clamav/clamav")
									.WithImageRegistry("docker.io")
									.WithImageTag("latest")
									.WithEnvironment("CLAMAV_NO_FRESHCLAMD", "true")
									.WithEndpoint(port: port, name: ClamAvResource.PrimaryEndpointName, targetPort: 3310);
	}

    public static IResourceBuilder<ClamAvResource> WithDataVolume(this IResourceBuilder<ClamAvResource> builder, string name, bool isReadOnly = false)
        => builder.WithVolume(name, "/var/lib/clamav", isReadOnly);

    public static IResourceBuilder<ClamAvResource> WithDataBindMount(this IResourceBuilder<ClamAvResource> builder, string source, bool isReadOnly = false)
        => builder.WithBindMount(source, "/var/lib/clamav", isReadOnly);
}
