using Microsoft.AspNetCore.Identity;
using ProjectManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Data
{
	public class SeedData
	{
		private const string ADMIN_EMAIL = "admin@ipg.pt";
		private const string ADMIN_PASS = "Secret123$";

		private const string ROLE_ADMINISTRATOR = "admin";
		//private const string ROLE_PROJECT_MANAGER = "project_manager";
		private const string ROLE_MEMBER = "member";

		internal static void Populate(ProjectsContext ProjectsContext)
		{
			Project project = ProjectsContext.Project.FirstOrDefault();
			//Project project2 = ProjectsContext.Project.FirstOrDefault();

			if (project == null)
			{
				// Populate Member
				for (int i = 1; i <= 10; i++)
				{
					ProjectsContext.Member.Add(
						new Member
						{
							Username = "User" + i,
							Name = "Name of " + i,
						}
					); ;
				}

				project = new Project { Name = "A Mobile App Development Project", ManagerId=0 };
				ProjectsContext.Add(project);

				for (int i = 1; i <= 5; i++)
				{
					ProjectsContext.Board.Add(
						new Board
						{
							Title = "Board " + i,
							BoardDescription = "Board description for" + i,
							Project = project,
							CreatedDate = DateTime.Now
						}
					);
				}

				project = new Project { Name = "Analysis Project", ManagerId = 1 };
				ProjectsContext.Add(project);
				for (int i = 1; i <= 10; i++)
				{
					ProjectsContext.Board.Add(
						new Board
						{
							Title = "Board " + i,
							BoardDescription = "Board description for " + i,
							Project = project,
							CreatedDate = DateTime.Now
						}
					);
				}

				ProjectsContext.SaveChanges();
			}
	
		}

		internal static void CreateDefaultAdmin(UserManager<IdentityUser> userManager)
		{
			EnsureUserIsCreatedAsync(userManager, ADMIN_EMAIL, ADMIN_PASS, ROLE_ADMINISTRATOR).Wait();
		}

		private static async Task EnsureUserIsCreatedAsync(UserManager<IdentityUser> userManager, string email, string password, string role)
		{
			var user = await userManager.FindByNameAsync(email);

			if (user == null)
			{
				user = new IdentityUser
				{
					UserName = email,
					Email = email
				};

				await userManager.CreateAsync(user, password);
			}

			if (await userManager.IsInRoleAsync(user, role)) return;
			await userManager.AddToRoleAsync(user, role);
		}

		internal static void PopulateUsers(UserManager<IdentityUser> userManager)
		{
			EnsureUserIsCreatedAsync(userManager, "john@ipg.pt", "Secret123$", ROLE_MEMBER).Wait();
			//EnsureUserIsCreatedAsync(userManager, "mary@ipg.pt", "Secret123$", ROLE_PROJECT_MANAGER).Wait();
		}

		internal static void CreateRoles(RoleManager<IdentityRole> roleManager)
		{
			EnsureRoleIsCreatedAsync(roleManager, ROLE_ADMINISTRATOR).Wait();
			// EnsureRoleIsCreatedAsync(roleManager, ROLE_PROJECT_MANAGER).Wait();
			EnsureRoleIsCreatedAsync(roleManager, ROLE_MEMBER).Wait();
		}

		private static async Task EnsureRoleIsCreatedAsync(RoleManager<IdentityRole> roleManager, string role)
		{
			if (await roleManager.RoleExistsAsync(role)) return;

			await roleManager.CreateAsync(new IdentityRole(role));
		}
	}
}
	

