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
				project = new Project { Name = "Anonymous" };
				ProjectsContext.Add(project);
			}

			for (int i = 1; i <= 1000; i++)
			{
				ProjectsContext.Board.Add(
					new Board
					{
						Name = "Board " + i,
						BoardDescription = "Book description " + i,
						Project = project
					}
				);
			}

			ProjectsContext.SaveChanges();
		}
	}
}
