using TrelloClone.Utilities.DBInitilization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using TrelloClone.DataAccess;
using TrelloClone.Models;
using TrelloClone.Utilities;

namespace TrelloClone;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDpContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
        .AddEntityFrameworkStores<AppDpContext>()
        .AddDefaultTokenProviders();

        var jwtSection = builder.Configuration.GetSection("Jwt");
        var jwtKey = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
            };
        });

        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IRepository<WorkspaceMember>, Repository<WorkspaceMember>>();
        builder.Services.AddScoped<IRepository<BoardMember>, Repository<BoardMember>>();
        builder.Services.AddScoped<IRepository<CardMember>, Repository<CardMember>>();
        builder.Services.AddScoped<IRepository<CardLabel>, Repository<CardLabel>>();
        builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
        builder.Services.AddScoped<IBoardRepository, BoardRepository>();
        builder.Services.AddScoped<IBoardListRepository, BoardListRepository>();
        builder.Services.AddScoped<ICardRepository, CardRepository>();
        builder.Services.AddScoped<ILabelRepository, LabelRepository>();
        builder.Services.AddScoped<IChecklistRepository, ChecklistRepository>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();
        builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
        builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
        builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
        builder.Services.AddScoped<IChecklistItemRepository, ChecklistItemRepository>();
        builder.Services.AddScoped<ICardWatcherRepository, CardWatcherRepository>();

        builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
        builder.Services.AddScoped<IBoardService, BoardService>();
        builder.Services.AddScoped<IBoardListService, BoardListService>();
        builder.Services.AddScoped<ICardService, CardService>();
        builder.Services.AddScoped<ILabelService, LabelService>();
        builder.Services.AddScoped<IChecklistService, ChecklistService>();
        builder.Services.AddScoped<IChecklistItemService, ChecklistItemService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IAttachmentService, AttachmentService>();
        builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<IInvitationService, InvitationService>();
        builder.Services.AddScoped<ICardWatcherService, CardWatcherService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IDBInitilizer, DBInitilizer>();
        builder.Services.AddScoped<IEmailSender, EmailSender>();

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

       

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<IDBInitilizer>();
            await initializer.Initialize();
        }

        app.MapControllers();
        app.Run();
    }
}


