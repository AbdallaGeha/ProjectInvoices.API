using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TakalNew.Data;
using ProjectInvoices.API.Data.Repository;
using ProjectInvoices.API.Domain.IRepository;
using ProjectInvoices.API.Extensions;
using ProjectInvoices.API.Middleware;
using ProjectInvoices.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(x => x.AddDefaultPolicy(x => x.WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader()));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IBankRepository, BankRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProjectInvoiceRepository, ProjectInvoiceRepository>();
builder.Services.AddScoped<IProjectInvoiceService, ProjectInvoiceService>();
builder.Services.AddScoped<IProjectInvoiceQueries, ProjectInvoiceQueries>();

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    //Add validation errors as one concatinated string to 400 response
    options.InvalidModelStateResponseFactory = context =>
    {
        var validationErrors = new List<string>();

        foreach (var state in context.ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                validationErrors.Add($"{state.Key}: {error.ErrorMessage}");
            }
        }
        
        return new BadRequestObjectResult(string.Join(System.Environment.NewLine, validationErrors));
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//call extension method to add authentication
builder.AddAppAuthetication();

builder.Services.AddAuthorization();

var app = builder.Build();

//using our custom exception handling middlware class
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app, app.Environment.IsProduction()).GetAwaiter().GetResult();

app.Run();
