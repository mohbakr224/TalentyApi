using Domain.Entities.Identity;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        AppDBcontext context)
    {
        // Apply pending migrations
        await context.Database.MigrateAsync();

        // ==========================================
        // 1. ROLES
        // ==========================================

        var rolesData = new[]
        {
            new
            {
                Name = "CANDIDATE",
                Description = "Candidate user"
            },
            new
            {
                Name = "RECRUITER",
                Description = "Recruiter user"
            },
            new
            {
                Name = "COMPANY_ADMIN",
                Description = "Company administrator"
            },
            new
            {
                Name = "ADMIN",
                Description = "System administrator"
            },
            new
            {
                Name = "HR",
                Description = "HR recruitment manager"
            }
        };

        var roles = new Dictionary<string, Role>();

        foreach (var roleData in rolesData)
        {
            var role = await context.Roles
                .FirstOrDefaultAsync(x => x.Name == roleData.Name);

            if (role == null)
            {
                role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleData.Name,
                    Description = roleData.Description,
                    CreatedDate = DateTime.UtcNow
                };

                context.Roles.Add(role);
            }

            roles[roleData.Name] = role;
        }

        await context.SaveChangesAsync();

        // ==========================================
        // 2. PERMISSIONS
        // ==========================================

        var permissionsData = new[]
        {
            // Candidate
            new
            {
                Name = "candidate.profile.read",
                Description = "Read candidate profile"
            },
            new
            {
                Name = "candidate.profile.update",
                Description = "Update candidate profile"
            },
            new
            {
                Name = "candidate.resume.upload",
                Description = "Upload candidate resume"
            },
            new
            {
                Name = "candidate.resume.read",
                Description = "Read candidate resume"
            },
            new
            {
                Name = "candidate.application.create",
                Description = "Create a job application"
            },
            new
            {
                Name = "candidate.application.read",
                Description = "Read candidate applications"
            },
            new
            {
                Name = "candidate.application.withdraw",
                Description = "Withdraw a job application"
            },

            // Jobs
            new
            {
                Name = "job.create",
                Description = "Create a job"
            },
            new
            {
                Name = "job.read",
                Description = "Read job information"
            },
            new
            {
                Name = "job.update",
                Description = "Update a job"
            },
            new
            {
                Name = "job.publish",
                Description = "Publish a job"
            },
            new
            {
                Name = "job.close",
                Description = "Close a job"
            },

            // Candidate Search
            new
            {
                Name = "candidate.search",
                Description = "Search candidates"
            },
            new
            {
                Name = "candidate.view",
                Description = "View candidate information"
            },

            // Applications
            new
            {
                Name = "application.read",
                Description = "Read applications"
            },
            new
            {
                Name = "application.manage",
                Description = "Manage applications"
            },
            // Additional application-specific permissions
            new
            {
                Name = "applications.view",
                Description = "View applications (permission)"
            },
            new
            {
                Name = "applications.change",
                Description = "Change application status"
            },

            // Company
            new
            {
                Name = "company.read",
                Description = "Read company information"
            },
            new
            {
                Name = "company.update",
                Description = "Update company information"
            },
            new
            {
                Name = "company.members.manage",
                Description = "Manage company members"
            },

            // Interviews
            new
            {
                Name = "interview.schedule",
                Description = "Schedule interviews"
            },
            new
            {
                Name = "interview.manage",
                Description = "Manage and update interviews"
            },
            new
            {
                Name = "interview.read",
                Description = "Read interview details"
            },

            // Notifications
            new
            {
                Name = "notification.send",
                Description = "Send recruitment notifications"
            },
            new
            {
                Name = "notification.read",
                Description = "Read notifications"
            },

            // Admin
            new
            {
                Name = "admin.users.manage",
                Description = "Manage users"
            }
        };

        var permissions = new Dictionary<string, Permission>();

        foreach (var permissionData in permissionsData)
        {
            var permission = await context.Permissions
                .FirstOrDefaultAsync(
                    x => x.Name == permissionData.Name);

            if (permission == null)
            {
                permission = new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = permissionData.Name,
                    Description = permissionData.Description,
                    CreatedDate= DateTime.UtcNow
                };

                context.Permissions.Add(permission);
            }

            permissions[permissionData.Name] = permission;
        }

        await context.SaveChangesAsync();

        // ==========================================
        // 3. ROLE -> PERMISSION MAPPING
        // ==========================================

        var rolePermissions = new Dictionary<string, string[]>
        {
            ["CANDIDATE"] = new[]
            {
                "candidate.profile.read",
                "candidate.profile.update",
                "candidate.resume.upload",
                "candidate.resume.read",
                "candidate.application.create",
                "candidate.application.read",
                "candidate.application.withdraw",
                "job.read",
                "interview.read",
                "notification.read"
            },

            ["RECRUITER"] = new[]
            {
                "job.create",
                "job.read",
                "job.update",
                "job.publish",
                "job.close",
                "candidate.search",
                "candidate.view",
                "candidate.resume.read",
                "application.read",
                "application.manage",
                "applications.view",
                "applications.change",
                "interview.schedule",
                "interview.manage",
                "interview.read",
                "notification.send",
                "notification.read",
                "company.read"
            },

            ["HR"] = new[]
            {
                "job.create",
                "job.read",
                "job.update",
                "job.publish",
                "job.close",
                "candidate.search",
                "candidate.view",
                "candidate.resume.read",
                "application.read",
                "application.manage",
                "applications.view",
                "applications.change",
                "interview.schedule",
                "interview.manage",
                "interview.read",
                "notification.send",
                "notification.read",
                "company.read",
                "company.update",
                "company.members.manage"
            },

            ["COMPANY_ADMIN"] = new[]
            {
                "job.create",
                "job.read",
                "job.update",
                "job.publish",
                "job.close",
                "candidate.search",
                "candidate.view",
                "candidate.resume.read",
                "application.read",
                "application.manage",
                "applications.view",
                "applications.change",
                "interview.schedule",
                "interview.manage",
                "interview.read",
                "notification.send",
                "notification.read",
                "company.read",
                "company.update",
                "company.members.manage"
            },

            ["ADMIN"] = permissionsData
                .Select(x => x.Name)
                .ToArray()
        };

        foreach (var rolePermissionData in rolePermissions)
        {
            var role = roles[rolePermissionData.Key];

            foreach (var permissionName in rolePermissionData.Value)
            {
                var permission = permissions[permissionName];

                var exists = await context.RolePermissions
                    .AnyAsync(x =>
                        x.RoleId == role.Id &&
                        x.PermissionId == permission.Id);

                if (!exists)
                {
                    context.RolePermissions.Add(
                        new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = permission.Id
                        });
                }
            }
        }

        await context.SaveChangesAsync();

        // ==========================================
        // 4. ADMIN USER
        // ==========================================

        var adminEmail = "admin@talenty.com";
        var adminUser = await context.Users.FirstOrDefaultAsync(x => x.Email == adminEmail);

        if (adminUser == null)
        {
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
            var passwordHash = passwordHasher.HashPassword(null!, "Admin@123");

            var adminRole = roles["ADMIN"];

            adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = adminEmail,
                PasswordHash = passwordHash,
                FirstName = "System",
                LastName = "Admin",
                IsActive = true,
                EmailVerified = true,
                IsLocked = false,
                CreatedDate = DateTime.UtcNow
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            context.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
            await context.SaveChangesAsync();
        }
    }
}