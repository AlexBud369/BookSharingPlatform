using Amazon.S3;
using API.Middleware;
using Application.Common;
using Application.DTOs;
using Application.DTOs.Book;
using Application.DTOs.User;
using Application.Features.Books.Commands;
using Application.Features.Books.Queries;
using Application.Features.Tags.Commands;
using Application.Features.Tags.Queries;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Application.Interfaces;
using Application.Services;
using Application.Validators.Auth;
using Application.Validators.Books;
using Application.Validators.Files;
using Application.Validators.Tags;
using Application.Validators.Users;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Configuration;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Persistence.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = DomainConstants.User.PasswordMinLength;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json");
    }
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly, typeof(CreateBook).Assembly));

builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(BookCreateDto).Assembly);

builder.Services.AddLocalization(options => options.ResourcesPath = "Application/Common/Resources");

builder.Services.AddLogging(logging => logging.AddConsole());

builder.Services.Configure<SupabaseOptions>(builder.Configuration.GetSection("Supabase"));
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = sp.GetRequiredService<IOptions<SupabaseOptions>>().Value;
    var s3Config = new AmazonS3Config
    {
        ServiceURL = config.Url,
        ForcePathStyle = true
    };
    return new AmazonS3Client(config.PublicKey, config.SecretKey, s3Config);
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookAccessService, BookAccessService>();
builder.Services.AddScoped<IBookQueryService, BookQueryService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ITagQueryService, TagQueryService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ISignedUrlService, SignedUrlService>();

builder.Services.AddScoped<IValidator<BookCreateDto>, BookCreateDtoValidator>();
builder.Services.AddScoped<IValidator<BookUpdateDto>, BookUpdateDtoValidator>();
builder.Services.AddScoped<IValidator<BookFilterDto>, BookFilterDtoValidator>();
builder.Services.AddScoped<IValidator<CreateBook.Command>, CreateBookCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateBook.Command>, UpdateBookCommandValidator>();
builder.Services.AddScoped<IValidator<DeleteBook.Command>, DeleteBookCommandValidator>();
builder.Services.AddScoped<IValidator<GetAllBooks.Query>, GetAllBooksQueryValidator>();
builder.Services.AddScoped<IValidator<GetBookById.Query>, GetBookByIdQueryValidator>();
builder.Services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateUser>, UpdateUserValidator>();
builder.Services.AddScoped<IValidator<GetAllUsers.Query>, GetAllUsersValidator>();
builder.Services.AddScoped<IValidator<GetUserById.Query>, GetUserByIdValidator>();
builder.Services.AddScoped<IValidator<DeleteUser>, DeleteUserValidator>();
builder.Services.AddScoped<IValidator<BlockUser.Command>, BlockUserValidator>();
builder.Services.AddScoped<IValidator<ChangeRole>, ChangeRoleValidator>();
builder.Services.AddScoped<IValidator<CreateTag>, CreateTagValidator>();
builder.Services.AddScoped<IValidator<DeleteTag>, DeleteTagCommandValidator>();
builder.Services.AddScoped<IValidator<GetAllTags>, GetAllTagsValidator>();
builder.Services.AddScoped<IValidator<GetTagById>, GetTagByIdQueryValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
builder.Services.AddScoped<IValidator<RefreshTokenDto>, RefreshTokenDtoValidator>();
builder.Services.AddScoped<IValidator<DeleteBookCover.Command>, DeleteBookCoverValidator>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();