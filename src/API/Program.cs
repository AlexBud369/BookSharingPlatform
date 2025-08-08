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
    if (string.IsNullOrEmpty(jwtKey)) {
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

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly, typeof(Application.Features.Books.Commands.CreateBookCommand).Assembly));

builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(Application.DTOs.Book.BookCreateDto).Assembly);

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
builder.Services.AddScoped<IValidator<UploadBookCoverCommand>, UploadBookCoverCommandValidator>();
builder.Services.AddScoped<IValidator<CreateBookCommand>, CreateBookCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateBookCommand>, UpdateBookCommandValidator>();
builder.Services.AddScoped<IValidator<DeleteBookCommand>, DeleteBookCommandValidator>();
builder.Services.AddScoped<IValidator<GetAllBooksQuery>, GetAllBooksQueryValidator>();
builder.Services.AddScoped<IValidator<GetBookByIdQuery>, GetBookByIdQueryValidator>();
builder.Services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserCommandValidator>();
builder.Services.AddScoped<IValidator<ChangeRoleRequestDto>, ChangeRoleRequestValidator>();
builder.Services.AddScoped<IValidator<GetAllUsersQuery>, GetAllUsersQueryValidator>();
builder.Services.AddScoped<IValidator<GetUserByIdQuery>, GetUserByIdQueryValidator>();
builder.Services.AddScoped<IValidator<DeleteUserCommand>, DeleteUserCommandValidator>();
builder.Services.AddScoped<IValidator<BlockUserCommand>, BlockUserCommandValidator>();
builder.Services.AddScoped<IValidator<ChangeRoleCommand>, ChangeRoleCommandValidator>();
builder.Services.AddScoped<IValidator<CreateTagCommand>, CreateTagCommandValidator>();
builder.Services.AddScoped<IValidator<DeleteTagCommand>, DeleteTagCommandValidator>();
builder.Services.AddScoped<IValidator<GetAllTagsQuery>, GetAllTagsQueryValidator>();
builder.Services.AddScoped<IValidator<GetTagByIdQuery>, GetTagByIdQueryValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
builder.Services.AddScoped<IValidator<RefreshTokenDto>, RefreshTokenDtoValidator>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();