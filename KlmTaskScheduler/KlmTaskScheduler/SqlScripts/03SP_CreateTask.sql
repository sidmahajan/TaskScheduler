CREATE PROCEDURE sp_CreateTask 
       @TaskName                     NVARCHAR(2000), 
       @TaskDate                     DATETIME      , 
       @StartTime                    NVARCHAR(10)  , 
       @EndTime                    NVARCHAR(10),
	   @AssignedUser int  
AS 
BEGIN 
     SET NOCOUNT ON 

     INSERT INTO TASKS
          (                    
            Name,
            Date,
            StartTime,
            EndTime,
			USERID
          ) 
     VALUES 
          ( 
            @TaskName,
            @TaskDate,
            @StartTime,
            @EndTime,
			@AssignedUser
          ) 
END 
GO