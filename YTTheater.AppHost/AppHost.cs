#pragma warning disable ASPIRECOMPUTE003
#pragma warning disable ASPIREPIPELINES003

var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddProject<Projects.YTTheater_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithHttpEndpoint(name: "http", targetPort: 8080);

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

if (builder.ExecutionContext.IsPublishMode)
{
    builder.AddDockerComposeEnvironment("prod");

    var endpoint = builder.AddParameter("RegistryEndpoint");
    var repository = builder.AddParameter("RegistryRepository");
    var registry = builder.AddContainerRegistry("DO", endpoint, repository);
    var imageTag = Environment.GetEnvironmentVariable("IMAGE_TAG") ?? "dev";

    server
        .WithContainerRegistry(registry)
        .WithImagePushOptions(x => x.Options.RemoteImageTag = imageTag);

    server.PublishWithContainerFiles(webfrontend, "wwwroot");

    var tunnelToken = builder.AddParameter("CloudflareTunnelToken", secret: true);

    builder.AddContainer("cloudflared", "cloudflare/cloudflared", "latest")
        .WithArgs("tunnel", "--no-autoupdate", "run")
        .WithEnvironment("TUNNEL_TOKEN", tunnelToken)
        .WaitFor(server);
}

builder.Build().Run();
