

use SocialMedia;
-- Create a new login for the user 'SMUser' with a password and default database
-- The password must meet the SQL Server password policy requirements
GO
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'SMUser' )
BEGIN
    CREATE LOGIN SMUser WITH PASSWORD = 'SMUserPassword123!', DEFAULT_DATABASE=SocialMedia;
END

-- Make the user a member of the db_owner role in the SocialMedia database
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'SMUser' )
BEGIN
EXEC sp_adduser 'SMUser', 'SMUser', 'db_owner';
END
