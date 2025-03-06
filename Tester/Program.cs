﻿//See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Template.Repository.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;
using Template.Domain.Common.IUnitOfWork;
using Template.Repository.UnitOfWork;
using Template.Service.Interfaces;
using Template.Domain.Entities;
using Template.Service.Implementations;
using Template.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Bogus;
using static Template.Service.Implementations.EmployeeService;
using Template.Service.DTOs.Admin;

var services = new ServiceCollection();
var buider = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("ConnectionStrings:DefaultConnection").Value;

services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(connectionString));

//Register dependencies
services.AddSingleton<IUnitOfWork, UnitOfWork>();
services.AddSingleton<ILog, LogService>();

services.AddLogging();

services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
{
    option.Password.RequiredLength = 4;
    option.Password.RequireDigit = false;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
;

ServiceProvider serviceProvider;
    serviceProvider = services.BuildServiceProvider();


ApplicationUser GenerateFakeUser()
{
    var faker = new Faker();

    return new ApplicationUser
    {
        UserName = faker.Internet.UserName(),
        NormalizedUserName = faker.Internet.UserName().ToUpper(),
        Email = faker.Internet.Email(),
        NormalizedEmail = faker.Internet.Email().ToUpper(),
        EmailConfirmed = faker.Random.Bool(),
        PasswordHash = faker.Internet.Password(),
        SecurityStamp = faker.Random.Guid().ToString(),
        ConcurrencyStamp = faker.Random.Guid().ToString(),
        PhoneNumber = faker.Phone.PhoneNumber(),
        PhoneNumberConfirmed = faker.Random.Bool(),
        TwoFactorEnabled = faker.Random.Bool(),
        LockoutEnd = faker.Random.Bool() ? DateTimeOffset.UtcNow.AddDays(7) : null,
        LockoutEnabled = faker.Random.Bool(),
        AccessFailedCount = faker.Random.Int(0, 5),
        FirstName = faker.Name.FirstName(),
        LastName = faker.Name.LastName()
    };
}

var logger = serviceProvider.GetRequiredService<ILog>();

logger.Log(new Exception(), System.Diagnostics.EventLogEntryType.Error);