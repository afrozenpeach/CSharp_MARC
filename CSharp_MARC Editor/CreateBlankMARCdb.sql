-- Script Date: 4/18/2016 9:05 PM  - ErikEJ.SqlCeScripting version 3.5.2.58
-- Database information:
-- Database: C:\Users\froze_000\Documents\SVN\CSharp MARC\CSharp_MARC Editor\MARC.db
-- ServerVersion: 3.9.2
-- DatabaseSize: 8 KB
-- Created: 4/12/2016 8:46 PM

-- User Table information:
-- Number of tables: 3
-- Fields: -1 row(s)
-- Records: -1 row(s)
-- Subfields: -1 row(s)

-- Warning - constraint: Fields Parent Columns and Child Columns don't have type-matching columns.

--DROP TABLE [Fields];
--DROP TABLE [Records];
--DROP TABLE [Subfields];

SELECT 1;
PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE [Subfields] (
  [SubfieldID] bigint NOT NULL
, [FieldID] bigint NOT NULL
, [Code] char NOT NULL
, [Data] nvarchar(2147483647) NOT NULL
, CONSTRAINT [sqlite_master_PK_Subfields] PRIMARY KEY ([SubfieldID])
, FOREIGN KEY ([FieldID]) REFERENCES [Fields] ([FieldID]) ON DELETE CASCADE ON UPDATE RESTRICT
);
CREATE TABLE [Records] (
  [RecordID] bigint NOT NULL
, [DateAdded] datetime NOT NULL
, [DateChanged] datetime NULL
, [Author] nvarchar(2147483647) NULL
, [Title] nvarchar(2147483647) NULL
, [Barcode] nvarchar(2147483647) NULL
, [Classification] nvarchar(2147483647) NULL
, [MainEntry] nvarchar(2147483647) NULL
, CONSTRAINT [sqlite_master_PK_Records] PRIMARY KEY ([RecordID])
);
CREATE TABLE [Fields] (
  [FieldID] bigint NOT NULL
, [RecordID] nvarchar(2147483647) NOT NULL
, [TagNumber] nvarchar(2147483647) NOT NULL
, [Ind1] char NOT NULL
, [Ind2] char NOT NULL
, [ControlData] nvarchar(2147483647) NULL
, CONSTRAINT [sqlite_master_PK_Fields] PRIMARY KEY ([FieldID])
, FOREIGN KEY ([RecordID]) REFERENCES [Records] ([RecordID]) ON DELETE CASCADE ON UPDATE RESTRICT
);
COMMIT;

