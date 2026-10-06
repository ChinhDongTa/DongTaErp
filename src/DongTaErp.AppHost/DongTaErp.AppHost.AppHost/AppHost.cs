Console.OutputEncoding = System.Text.Encoding.UTF8;
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DongTaErp_Web>("dongtaerp-web");

builder.Build().Run();
