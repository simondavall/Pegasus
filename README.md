# Pegasus
Pegasus was initially created to test the processes and features of creating a .Net Core project. Originally created in .Net Core 2.1, was upgraded to .Net Core 3.1.

The project is a lightweight issue/task tracker created without the feature bloat of existing tools like Jira and YouTrack.

The project is separated into:

Pegasus - providing all of the UI interaction.
Pegasus.Api - providing data connectivity and authentication.

There is also a workers service created to run as a 'keep alive' nudge to the UI.

Basic features include:
Adding projects.
Adding Tasks/Issues to Projects.
Tracking task status/type/priority.
Filters allow the user to customize what task are displayed. (E.g. Open tasks, Backlog, High Priority, etc.)
User login. (with Two Factor Authentication)
Comments section for each project task.
Comments attributed to users.
A settings section allows the user to customize the UI.

Database interaction and authentication is restricted to the Api project which use Entity Framework and Identitiy.
Connectivity between Pegasus and the Api is controlled using JwtBearer tokens.






