using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models.ViewModels
{
	public class ProjectsListViewModel
	{
		public IEnumerable<ProjectMember> ProjectMember { get; set; }
		public PagingInfo PagingInfo { get; set; }
		public string NameSearched { get; set; }
	}
}
