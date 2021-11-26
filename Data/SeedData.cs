using ProjectManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Data
{
	public class SeedData
	{
		internal static void Populate(ProjectsContext ProjectsContext)
		{
			Project project = ProjectsContext.Project.FirstOrDefault();

			if (project == null)
			{
				project = new Project { Name = "Project Test" };
				ProjectsContext.Add(project);
			}

			for (int i = 1; i <= 5; i++)
			{
				ProjectsContext.Board.Add(
					new Board
					{
						Title = "Board " + i,
						BoardDescription = "Board description " + i,
						CreatedDate = DateTime.Now,
						Project = project
					}
				); ;
			}

			ProjectsContext.SaveChanges();
		}
	}
}
