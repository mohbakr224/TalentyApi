using Domain.Entities.Applications;
using Domain.Entities.AI;
using Domain.Entities.AuthenticationTokenEntities;
using Domain.Entities.AuthenticationToken;
using Domain.Entities.Candidates;
using Domain.Entities.Companies;
using Domain.Entities.Identity;
using Domain.Entities.Jobs;
using Domain.Entities.Interviews;
using Domain.Entities.Notifications;
using Domain.Entities.Resumes;
using Domain.Entities.Skills;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;
namespace infrastructure.Persistence
{
    public class AppDBcontext:DbContext
    {
        public AppDBcontext(
              DbContextOptions<AppDBcontext> options)
              : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<CandidateProfile> CandidateProfiles { get; set; }
        public DbSet<CandidateExperience> CandidateExperiences { get; set; }
        public DbSet<CandidateEducation> CandidateEducations { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        public DbSet<CandidateLanguage> CandidateLanguages { get; set; }
        public DbSet<CandidateProject> CandidateProjects { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyMember> CompanyMembers { get; set; }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }

        public DbSet<Domain.Entities.Applications.Application> Applications { get; set; }
        public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }

        public DbSet<Interview> Interviews { get; set; }
        public DbSet<InterviewSession> InterviewSessions { get; set; }
        public DbSet<InterviewQuestion> InterviewQuestions { get; set; }
        public DbSet<InterviewAnswer> InterviewAnswers { get; set; }
        public DbSet<InterviewerAvailability> InterviewerAvailabilities { get; set; }
        public DbSet<InterviewBooking> InterviewBookings { get; set; }
        public DbSet<InterviewEvaluation> InterviewEvaluations { get; set; }
        public DbSet<InterviewEvaluationCriteria> InterviewEvaluationCriteria { get; set; }
        public DbSet<InterviewReport> InterviewReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AiAnalysisJob> AiAnalysisJobs { get; set; }

        public DbSet<Resume> Resumes { get; set; }
        public DbSet<ResumeVersion> ResumeVersions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordResetToken> passwordResetTokens { get; set; }
        public DbSet<EmailVerificationTokens> EmailVerificationTokens { get; set; }
        public DbSet<AccessToken> AccessTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDBcontext).Assembly);
        }
    }
}
