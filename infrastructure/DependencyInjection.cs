using Application.Configuration;
using Application.Interfaces.AuthenticationToken;
using Application.Interfaces.Persistence;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using infrastructure.Background;
using infrastructure.Email;
using infrastructure.Persistence;
using infrastructure.Repository.Applications;
using infrastructure.Repository.AuthenticationToken;
using infrastructure.Repository.Company;
using infrastructure.Repository.Identity;
using infrastructure.Repository.Interviews;
using infrastructure.Repository.Job;
using infrastructure.Repository.Notifications;
using infrastructure.Repository.Resumes;
using infrastructure.Security;
using infrastructure.Services.Applications;
using infrastructure.Services.Candidate;
using infrastructure.Services.Company;
using infrastructure.Services.Interviews;
using infrastructure.Services.Job;
using infrastructure.Services.Notifications;
using infrastructure.Services.Resumes;
using infrastructure.Services.Security;
using infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDBcontext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")
                );
                options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IAccessTokenRepo, AccessTokenRepository>();
            services.AddScoped<IRefreshTokenRepo, Infrastructure.Persistence.Repositories.RefreshTokenRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IResumeRepository, ResumeRepository>();
            services.AddScoped<IInterviewRepository, InterviewRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            // SignalR
            services.AddSignalR();

            // Security / Authentication / Permissions
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            var jwtSecret = configuration["Jwt:SecretKey"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            if (!string.IsNullOrEmpty(jwtSecret))
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                        ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
                        ValidAudience = jwtAudience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            }

            services.AddScoped<IPasswordHasherServices, PasswordHasherServices>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IJwtService, JwtServices>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPermissionChecker, PermissionChecker>();
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            // Business Services
            services.AddScoped<ICandidateService, CandidateService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IResumeService, ResumeService>();
            services.AddScoped<IInterviewService, InterviewService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IRealTimeNotificationService, SignalRNotificationService>();
            services.AddScoped<IEmailService, SmtpEmailService>();

            // File storage & Background Tasks
            services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
            services.AddScoped<IFileService, LocalFileStorageService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<ResumeProcessingHostedService>();

            return services;
        }
    }
}