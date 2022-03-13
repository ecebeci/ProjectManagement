# Project Management
ASP.NET CORE 5.0 MVC Project for the Internet Programming Lesson

This project aims to provide project management with projects and boards (like sub-projects) within the project, inspired by the working logic of project management applications such as Trello/Jira. There can be lists in each board and also each list can have works. Status and deadline of each work can be updated. Also status of work can be selected and be changed later.

![Main Page](https://user-images.githubusercontent.com/31591904/158066429-4767cf41-01a1-400f-a5d8-c70ff95de775.png)

## **Table of Actors**
<table>
<colgroup>
<col style="width: 49%" />
<col style="width: 50%" />
</colgroup>
<thead>
<tr class="header">
<th>Actor Name</th>
<th>Use Cases</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td rowspan="6"><p>Project Manager</p>
<p><strong>Project Manager</strong> is creator of a Project. It can
create a board inside of project. Also, can add or delete people to
project. A Project Manager for a project has Project Members use
cases.</p></td>
<td>Create a Project</td>
</tr>
<tr class="even">
<td>Add/Delete People to Project and Edit a Project</td>
</tr>
<tr class="odd">
<td>Delete/Revive a Project</td>
</tr>
<tr class="even">
<td>Create a Board</td>
</tr>
<tr class="odd">
<td>Edit a Board</td>
</tr>
<tr class="even">
<td>Add Template to Board</td>
</tr>
<tr class="odd">
<td rowspan="7"><p>Project Member</p>
<p><strong>Project Member</strong> is a member of a Project. It can
create and edit works.</p></td>
<td>Register System</td>
</tr>
<tr class="even">
<td>Login System</td>
</tr>
<tr class="odd">
<td>Add a List&amp;Label</td>
</tr>
<tr class="even">
<td>Change List</td>
</tr>
<tr class="odd">
<td>Create a Work to a List</td>
</tr>
<tr class="even">
<td>Set Priority of Work</td>
</tr>
<tr class="odd">
<td>Change Work Status</td>
</tr>
<tr class="even">
<td rowspan="3"><p>System Manager</p>
<p>System Manager manages all system. It can see all users and delete
users. Also, can add new board templates.</p></td>
<td>Check Users Information</td>
</tr>
<tr class="odd">
<td>Delete Users</td>
</tr>
<tr class="even">
<td>Adding New Template</td>
</tr>
</tbody>
</table>

## **Application Prototypes**

### Projects Controller

The controller where the projects belonging to the user are listed. If
the actor is the project manager on their project, they can edit their
project.

![Projects Controller](https://user-images.githubusercontent.com/31591904/158066441-74aa1dbe-18da-4040-8ddc-b5b8f02545ca.png)

### Boards Controller

The controller where the board belonging to the project are listed. If
the actor is project manager on selected project, they can add or edit
boards.

![Boards Controller](https://user-images.githubusercontent.com/31591904/158066452-2ee354c0-496b-4771-b3da-d88db495b9ed.png)

### Dashboard Controller

The list controller where the lists belonging to the board are listed.
Every project member on project can add or edit works.

![Dashboard Controller](https://user-images.githubusercontent.com/31591904/158066459-465cff60-bf0c-4c6e-95f4-61a33ef03d8e.png)
