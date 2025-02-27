//See https://aka.ms/new-console-template for more information
using static System.Net.Mime.MediaTypeNames;
using System.Security.Principal;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Template.Repository.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;
using Template.Domain.Common.IUnitOfWork;
using Template.Repository.UnitOfWork;
using Template.Service.IService;
using Template.Service;
using AutoMapper;
using Template.Service.Profiles;
using Template.Domain.Entities;
using Template.Service.DTOs;
using System.Collections.Generic;
using System.Reflection.Emit;
using Template.Service;
using Template.Service.Email;

var services = new ServiceCollection();
    string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("ConnectionStrings:DefaultConnection").Value;

    #region AutoMapper
    var mapperConfig = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile(new MappingProfile());
    });

    IMapper mapper = mapperConfig.CreateMapper();
    services.AddSingleton(mapper);
    #endregion

    //Register dependencies
    services.AddSingleton<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IDepartmentService, DepartmentService>();

    services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
    // Set up Dependency Injection

    ServiceProvider serviceProvider;
    //Keep the service provider alive for the application's lifetime
    serviceProvider = services.BuildServiceProvider();

//    // Resolve and run the main form
//    var department = serviceProvider.GetRequiredService<IDepartmentService>();


//    DepartmentDto dto = new DepartmentDto()
//    {
//        //Name = "Ahmed"
//    };


//department.Delete(1);
//department.SaveChanges();
//Console.WriteLine(dto.Name);

var senderEmail = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("EmailConfiguration:Email").Value;
var appPasswords = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("EmailConfiguration:GmailAppPassword").Value;
var stmpServer = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("EmailConfiguration:SmtpServer").Value;

EmailService eamil = new EmailService(senderEmail, appPasswords, stmpServer);

await eamil.VerifyEmailAsync("bdalzyzalbrnawy47@gmail.com", "Congruat u have been a developer", "Hi me i want to test if it's work", "Abdulaziz");
