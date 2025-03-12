using Eshop.Api.Auth;
using Eshop.Api.Extensions;
using Eshop.Api.Data;
using Eshop.Api.Endpoints;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositories(builder.Configuration);

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddEshopAuthorization();

builder.Services.AddApiVersioning(options=>{
    options.DefaultApiVersion = new(1.0);
    options.AssumeDefaultVersionWhenUnspecified = true;
}).AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");

builder.Services.AddHttpLogging();
builder.Services.AddEshopCors(builder.Configuration);

builder.Services.AddSwaggerGen()
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
                .AddEndpointsApiExplorer();

var app = builder.Build();
app.UseExceptionHandler(exceptionHandler=>exceptionHandler.ConfigureExceptionHandler());

await app.Services.InitializeDbAsync();

app.UseHttpLogging();

app.MapProductsEndpoints();

app.UseEshopSwagger();

app.Run();
