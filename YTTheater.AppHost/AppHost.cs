var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("prod");

var endpoint = builder.AddParameter("RegistryEndpoint");
var repository = builder.AddParameter("RegistryRepository");

#pragma warning disable ASPIRECOMPUTE003
builder.AddContainerRegistry("DO", endpoint, repository);

var server = builder.AddProject<Projects.YTTheater_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
