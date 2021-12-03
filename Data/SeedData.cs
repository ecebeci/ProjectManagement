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
		
		private const string ROLE_MEMBER = "member";

		internal static void Populate(ProjectsContext ProjectsContext)
		{
			Member member = ProjectsContext.Member.FirstOrDefault();
			if (member == null)
			{
				member = new Member { Username = "admin@ipg.pt", Name = "admin" };
				ProjectsContext.Add(member);
				member = new Member { Username = "john@ipg.pt", Name = "John Smith" };
				ProjectsContext.Add(member);
				member = new Member { Username = "emre@ipg.pt", Name = "Emre Cebeci" };
				ProjectsContext.Add(member);
				member = new Member { Username = "jane@ipg.pt", Name = "Jane Doe" };
				ProjectsContext.Add(member);

				ProjectsContext.SaveChanges();
			}

			Project project = ProjectsContext.Project.FirstOrDefault();

			if (project == null)
			{
				project = new Project { 
					Name = "A Mobile App Development Project", 
					ManagerId=1 ,
					CreatedDate = DateTime.Now 
				};
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

				project = new Project {
					Name = "Analysis Project", 
					ManagerId = 2, 
					CreatedDate = DateTime.Now 
				};
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

				project = new Project {
					Name = "Construction Project",
					ManagerId = 2 ,
					CreatedDate = DateTime.Now
				};
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

				// Add Members for each Project
				ProjectsContext.Add(new ProjectMember { MemberId = 2, ProjectId = 2 });
				ProjectsContext.Add(new ProjectMember { MemberId = 3, ProjectId = 3 });
				ProjectsContext.Add(new ProjectMember { MemberId = 4, ProjectId = 3 });  


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
			EnsureUserIsCreatedAsync(userManager, "emre@ipg.pt", "Secret123$", ROLE_MEMBER).Wait();
			EnsureUserIsCreatedAsync(userManager, "jane@ipg.pt", "Secret123$", ROLE_MEMBER).Wait();
			
		}

		internal static void CreateRoles(RoleManager<IdentityRole> roleManager)
		{
			EnsureRoleIsCreatedAsync(roleManager, ROLE_ADMINISTRATOR).Wait();
			EnsureRoleIsCreatedAsync(roleManager, ROLE_MEMBER).Wait();
		}

		private static async Task EnsureRoleIsCreatedAsync(RoleManager<IdentityRole> roleManager, string role)
		{
			if (await roleManager.RoleExistsAsync(role)) return;

			await roleManager.CreateAsync(new IdentityRole(role));
		}
	}
}
	

