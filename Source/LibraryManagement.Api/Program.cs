
using FluentValidation.AspNetCore;
using LibraryManagement.Api.AutoMapper;
using LibraryManagement.Api.Controllers;
using LibraryManagement.Application;
using LibraryManagement.Domain.Domains;
using LibraryManagement.Infrastructure;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register fluent validation
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

// Dependencies of  application layer
builder.Services.AddApplication().AddInfrstructure();


builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
