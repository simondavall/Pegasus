update TaskComments
Set Modified = Created
where Modified IS NULL

Update ProjectTasks
Set UserId = 'f7d69a12-ab97-4cf4-916a-da96f403a0e7'
where UserId IS NULL

Update TaskComments
Set UserId = 'f7d69a12-ab97-4cf4-916a-da96f403a0e7'
where UserId IS NULL

Update StatusHistory
Set UserId = 'f7d69a12-ab97-4cf4-916a-da96f403a0e7'
where UserId IS NULL

Insert Users (Id, UserName, Email)
Values ('f7d69a12-ab97-4cf4-916a-da96f403a0e7', 'Simon Da Vall', 'simon.davall@gmail.com')
