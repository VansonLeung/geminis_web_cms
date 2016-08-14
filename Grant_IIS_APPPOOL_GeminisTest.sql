IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'IIS APPPOOL\GeminisTest')
BEGIN
    CREATE LOGIN [IIS APPPOOL\GeminisTest] 
      FROM WINDOWS WITH DEFAULT_DATABASE=[master], 
      DEFAULT_LANGUAGE=[us_english]
END
GO
CREATE USER "IIS APPPOOL\GeminisTest"
  FOR LOGIN [IIS APPPOOL\GeminisTest]
GO
EXEC sp_addrolemember 'db_owner', 'IIS APPPOOL\GeminisTest'
GO