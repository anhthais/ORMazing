IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DemoDB')
BEGIN
    CREATE DATABASE DemoDB;
END
GO

USE DemoDB;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Users' AND xtype = 'U')
BEGIN
    CREATE TABLE Users (
        Id INT PRIMARY KEY,
        Name NVARCHAR(100),
        Age INT
    );
    
    INSERT INTO Users (Id, Name, Age)
    VALUES (1, 'Alice', 30),
           (2, 'Bob', 25),
           (3, 'Charlie', 35);
END
GO
