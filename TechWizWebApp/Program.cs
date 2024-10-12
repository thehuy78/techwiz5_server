using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using TechWizWebApp.Data;
using TechWizWebApp.Hubs;
using TechWizWebApp.Interface;
using TechWizWebApp.InterfaceCustomer;
using TechWizWebApp.Interfaces;
using TechWizWebApp.Repositories;
using TechWizWebApp.Repository;
using TechWizWebApp.RepositotyCustomer;
using TechWizWebApp.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DecorVistaDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowOrigin = builder.Configuration.GetSection("AllowOrigin").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("myAppCors", policy =>
    {
        policy.WithOrigins(allowOrigin).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["JwtSettings:Key"]!
            )),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<ISeedService, SeedService>();
builder.Services.AddTransient<IAuthAdmin, AuthAdminRepo>();
builder.Services.AddTransient<IProductAdmin, ProductAdminRepo>();
builder.Services.AddTransient<IDesignerAdmin, DesignerAdminRepo>();
builder.Services.AddTransient<IConsultationAdmin, ConsultationAdminRepo>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IGallery, GalleryRepo>();
builder.Services.AddTransient<IRoomType, TypeRoomRepo>();
builder.Services.AddTransient<IOrder, OrderRepo>();
builder.Services.AddTransient<IOrderDetails, OrderDetailRepo>();
builder.Services.AddTransient<IBlog, BlogRepo>();
builder.Services.AddTransient<IStoryAdmin, StoryAdminRepo>();
builder.Services.AddTransient<INotificationAdmin, NotificationAdminRepo>();

//user
builder.Services.AddTransient<IRoomTypeFE, RoomTypeFERepo>();
builder.Services.AddTransient<IProductFE, ProductFERepo>();
builder.Services.AddTransient<IGalleryFE, GalleryFERepo>();
builder.Services.AddTransient<IDesignerFE, DesignerFERepo>();
builder.Services.AddTransient<IBlogsFE, BlogsFERepo>();
builder.Services.AddTransient<IAuthFE,AuthFERepo>();
builder.Services.AddTransient<IReviewFE, ReviewFERepo>();
builder.Services.AddTransient<IOrderFE, OrderFERepo>();
builder.Services.AddTransient<IOrderdetailFE, OrderDetailFERepo>();
builder.Services.AddTransient<IBookingFE, BookingFERepo>();
builder.Services.AddTransient<IUserFE, UserFERepo>();

builder.Services.AddTransient<IBookmarkFE, BookmarkFERepo>();
builder.Services.AddTransient<INotificationFE, NotificationFERepo>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("myAppCors");


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Images")),
    RequestPath = "/Images"
});

app.MapHub<ChatHub>("/chat");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
