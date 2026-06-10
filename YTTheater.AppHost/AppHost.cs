#pragma warning disable ASPIRECOMPUTE003
#pragma warning disable ASPIREPIPELINES003

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("prod");

var endpoint = builder.AddParameter("RegistryEndpoint");
var repository = builder.AddParameter("RegistryRepository");
var imageTag = Environment.GetEnvironmentVariable("IMAGE_TAG") ?? "dev";

var registry = builder.AddContainerRegistry("DO", endpoint, repository);

var server = builder.AddProject<Projects.YTTheater_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithContainerRegistry(registry)
    .WithImagePushOptions(x => x.Options.RemoteImageTag = imageTag)
    .WithExternalHttpEndpoints();

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
