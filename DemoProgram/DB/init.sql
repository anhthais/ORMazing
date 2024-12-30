CREATE DATABASE DemoDB;
GO

USE DemoDB;
GO

CREATE TABLE Users (
    Id INT PRIMARY KEY,
    Name NVARCHAR(100),
    Age INT
);
GO

INSERT INTO Users (Id, Name, Age)
VALUES (1, 'Alice', 30),
       (2, 'Bob', 25),
       (3, 'Charlie', 35);
GO
