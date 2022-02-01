# C# MARC Editor

Full featured editor and C# class structures for Library of Congress MARC21 and MARCXML bibliography records.

This project is built upon the CSharp_MARC project of the same name available at http://csharpmarc.net, which itself is based on the File_MARC package (http://pear.php.net/package/File_MARC) by Dan Scott, which was based on PHP MARC package, originally called "php-marc", that is part of the Emilda Project (http://www.emilda.org). Both projects were released under the LGPL which allowed me to port the project to C# for use with the .NET Framework.

## Features

* Easily read and view MARC records in a simple text based format
* FAST and lightweight! Capable of importing 28,000 records a minute!
* Can handle MARC Records of any size
* Find and Replace with Regular Expression support with tag, indicator, and subfield level filtering
* Advanced Batch Edit allows easily adding, editing, replacing, or deleting multiple fields in a record set
* Automatically resort fields and subfields
* Export to MARC21 UTF8 or MARC8 encoding, MARCXML, or CSV file formats
* Split large record sets into multiple files
* Z39.50 support using the SRU API with customizable address string
* Converts AACR2 to RDA (not intended to be a full conversion, but takes what it can from AACR2 and converts to RDA)
* Validate tag numbers, indicators, and subfield codes
* Report generation by copyright year, classification, and a record summary
* SQLite database backend - advanced users can connect with any SQLite client to do further SQL queries on the dataset

## Changelog:

### 2022-02-01 Changes:
 
* Added a new public member to the FileMARC class to force UTF8 on records that don't have the correct encoding flag in the leader
* Updated copyrights
* Updated .NET Framework and Nuget packages

### 2018-07-06 Changes:

* Fixed issue where a user could attempt to add tags and subfields before a record has been added
* Fixed issue with some MARC21 records not loading properly

### 2018-02-20 Changes:

* Export CSV now exports validation results column

### 2018-02-19 Changes:

* Advanced Batch Editing features now properly insert new fields in sorted order
* Updated copyright information

### 2017-08-18 Changes:

* If a record is completely unreadable, allow importing records to continue. A "blank" record is inserted instead with error information in the warnings field.

### 2017-06-27 Changes:

* Fixed loading and exporting MARCXML format records

### 2017-05-22 Changes:

* Speed up handling of large databases
* Updated copyright information
* If barcode information is in the b subfield, don't use it for classification data
* Fixed some sorting issues

### 2016-08-03 Changes:

* Fix loading preview information for records with holdings information in the 949a

### 2016-07-22 Changes:

* Fix issues with moving a field or subfield up past the top or down past the new row
* Fix moving fields to work as expected
* Add tooltips to field and subfield sorting buttons
* Fix issue importing multiple records from SRU
* Add LoC and OCLC options to the import SRU feature
* Fix ReSharper Inspection fixes
* Fix import and cancel buttons when form gets resized
* Selected Codes and Indicators should actually return selected codes and indicators, respectively, rather than... you know, everything.

### 2016-06-12.2 Changes:

* Fix database reset

### 2016-06-12.1 Changes:

* Fix field sorting
* Speed sorting up
* Batch Edit is now in a background worker thread
* Add sort all fields and subfields functionality

### 2016-06-12 Changes:

* Add Advanced Batch Edit feature
* Fix database reset
 
### 2016-06-09 Changes:

* Fix some more warnings
* Fix issue importing records with certain 949 holdings tags

### 2016-06-05 Changes:

* Fix issue importing UTF8 records with some strange encoding issues
* Add option to force UTF8 encoding

### 2016-06-01 Changes:

* Fix issue with RDA Conversion relating to 300c being changed from cm. and mm. to cm and mm
* Remove some build warnings

### 2016-05-16.1 Changes:

* Fix issue exporting records

### 2016-05-16 Changes:

* Improve speed of adding fields and subfields
* Speed up loading large datasets
* Fix parsing of records with an less than 5 characters in the Record Length portion of the LDR.

### 2016-05-15 Changes:

* Add tag number, indicator, and subfield code validation
* Fix issue where field and subfield sorting buttons were not disabled while a batch process is running

### 2016-05-14.1 Changes:

* Fix issue loading subfields

### 2016-05-14 Changes:

* Allow manually sorting of fields

### 2016-05-13 Changes:

* Allow manually sorting of subfields
* More Improvements to RDA Conversion

### 2016-05-13 Changes:

* More Improvements to RDA Conversion
* RDA Conversion shouldn't duplicate changes if a record has already been converted

### 2016-05-12.2 Changes:

* Improvements to RDA Conversion

### 2016-05-12.1 Changes:

* Fix reading and writing UTF8 files

### 2016-05-12 Changes:

* Added RDA Conversion
* Fixed bug resulting in the export progress bar not working
* Leader now working as an editable field - Note: Some parts of the leader will be automatically calculated
 
### 2016-05-07.1 Changes:

* Setup click-once application installer
* Add copyright date, copyright date by decade, and records reporting functionality
* Placeholders for future RDA Conversion and Validation features

#### 2016-05-07 Changes:

* Added placeholder menu options for future features
* 005 field now loads in the DateChanged column of the records preview information
* 005 field now updates automatically when other fields/subfields are edited
* 005 field is no longer editable by hand
* Added support for custom SRU servers
* Added support for importing multiple records at once from SRU
* Added support for cancelling imports
* Several fixes involving rebuilding the records preview information 
* Several speed improvements for loading and editing large datasets
 
### 2016-05-05 Changes:

* More rebuilding improvements
* Fix issue with not showing changes made to fields in the records preview information

### 2016-05-04 Changes:

* Speed up importing
* Speed up rebuilding records preview information after changing custom fields
 
### 2016-05-03 Changes:

* Sped up Rebuilding the records preview information
* Added print functionality
* Fixed x86/x64 support
 
### 2016-05-01.1 Changes:

* Added support for exporting records into CSV
* Added Z39.50/SRU import support
* Added error reporting for issues importing records
 
### 2016-05-01 Changes:

* Added support for MARC8, UTF8, and MARCXML exports
* Added support for 5 custom fields

### 2016-04-30 Changes:

* Added export split records functionality
* Added regex find and replace
* Fixed issues with case-insensitive find and replace

### 2016-04-28 Changes:

* First release of C# MARC Editor - a full featured MARC Editing suite

### 2016-04-21 Changes:

* Bump .NET Version to 4
* An issue was discovered that resulted in incorrectly reading multi-byte characters when loading records.

### 2014-05-21 Changes:

* FileMARCWriter and FileMARCXMLWriter now have an append option

### 2014-05-06 Changes:

* Fixed issue with special characters not being properly calculated in the directory length

### 2014-02-01 Changes:

* Added support for exporting MARCXML.
* Added FileMARCXMLWriter to make saving single records, as well as lists for records, to XML a simple process
* Updated copyright information for the year 2014!

### 2013-04-12 Changes:

* More support for special MARC8 characters thanks to my new MARC8 Encoding class. Special thanks to Mark V. Sullivan of Sobek CM MARC Library (https://sourceforge.net/projects/marclibrary/) for code used to read the MARC8 format, which much of the Encoding was based on.

### 2013-04-11 Changes:

* Fix issue relating to the FileMARCWriter needing to write multiple bytes per character in some cases, as well as handling encoding of non-special characters better.

### 2013-04-10 Changes:

* Some special characters in a normal MARC21 file are saved using non-standard character encoding. The proper encoding should be MARC8, which .Net does not support. This version adds support for a select few special characters, most importantly the copyright symbol which is used in RDA records. The FileMARCWriter class makes it easier to write records and should follow MARC8/UTF-8 specifications from the 9th character of the Leader. The FileMARCReader class had to be updated to read these MARC8 characters back in correctly.
* Initial commit of FileMARCWriter
* Updated FileMARCReader to support features of FileMARCWriter
* UTF-8 support is only loosely tested. If you are using UTF-8 please let me know how well it works for you.
 
### 2012-06-13 Changes:

* Added FileMARCReader class for handling large MARC21 files without loading the entire file into memory. Special thanks to Stas Paladiy for reporting this issue and helping resolve it.
* Updated Copyright information for the year 2012!

### 2011-09-12.2 Changes:

* Apparently not even Library of Congress MARCXML records follow the XSD specification.  I've removed the requirement that records validate before being added.
* No longer ordering the tags when importing a MARCXML record.

### 2011-09-12.1 Changes:

* CSharp_MARC can now read MARCXML files.  The FileMARCXML class can accept XML strings or native .NET XDocument objects and convert them to Record objects.
* The FileMARC(string source) constructor and ImportMARC function were duplicating code from the FileMARC.Add(string source) function. The coding horror is me :negative:

### 2011-06-30 Change:

* Added Clone() to Fields, Subfields, and Records. This is a DEEP clone and all members should be properly cloned as new instances.

### 2011-04-26 Change:

* Changed how validating a tag works when decoding due to "not thinking syndrome"

### 2011-03-04 Changes:

* Updated copyright information
* Added x86 Platform for easier testing. Apparently it didn't stick when I added it earlier.
* Testing suite to test the class structure and included methods. This will get more advanced as I port specific tests from File_MARC.
* Added a bit of extra error checking to a Field's tag so that it's not possible to assign an invalid tag.
* If the FileMARC parser comes across an invalid tag, it forces the tag to "ZZZ" and should no longer throw an exception. 
* Made Field.IsEmpty() abstract. It's not possible to assign a Field with an empty string tag. Doing so should cause an exception. If this is in fact the case, then IsEmpty on Field should never return true. Because of this IsEmpty is now an override on both DataField and ControlField. Field.IsEmpty now returns the result of it's inherited version.
* ToString functions for ControlField and DataField changed to overrides. Field.ToString seemed odd to only output the tag and not the inner data if available.
* Fixed bug in ControlField.IsEmpty() returning opposite of expected results. Amazing how much unit testing helps!
* DataField.FormatField() with no exclude codes as parameters actually works now.
* Added a bit of extra error checking and cleanup to the indicators in the decode function.
* Indicators are now closer to the MARC21 standard, allowing both numbers and letters. Uppercase letters are forced to lowercase. # is no longer a valid character, as it is supposed to indicate an ASCII SPACE. Invalid characters are automatically changed to ASCII SPACE and warnings are added to indicate this
* Moved warnings into the Record object rather than FileMARC. This makes it easier to track which warnings were for which Record. This change may be API breaking and I apologize for that.
* Fixed a bug where it is possible to make an invalid record by setting the Leader to a string longer than 24 characters. Now it will still allow you to set a long Leader but will only output the first 24 characters.
* Fixed a bug where if the Leader was less than 24 characters it would make an invalid record.
* You can now make tags that are not exactly 3 characters. It will automatically pad the leading 0s.
* Field.ToString() is now an abstract override rather than an abstract new. No idea what I was thinking with it as new.
* Record.ToString() doesn't have to check what type it is, thanks to override!
* Totally had the IEnumerable designed wrong. Reset should reset to -1, as MoveNext will take you to the first record.  After a reset it was ignoring the first record. My bad!

### 2011-02-16 Changes:

* Added personal email to copyright notices.
* Updated copyright years
* Added an overloaded GetSubfields() so you no longer have to pass in null to get all subfields.
* Changed Warnings from protected to public. No idea what I was thinking there.
* Tons more error checking in the FileMARC decode function due to coming across an egregiously bad record.
* Now with fancy new Demo application! Hopefully it has enough example code to get people started better.
* Updated solution files to VS2010
* Probably some other stuff.

## Copyright

### Parser for MARC records

This project is based on the File_MARC package (http://pear.php.net/package/File_MARC) by Dan Scott , which was based on PHP MARC package, originally called "php-marc", that is part of the Emilda Project (http://www.emilda.org). Both projects were released under the LGPL which allowed me to port the project to C# for use with the .NET Framework.
 
This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 
This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.
 
@author    Mattie Schraeder <mattie@csharpmarc.net>
@copyright 2009-2017 Mattie Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
@license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3

### Editor for MARC records

This project is built upon the CSharp_MARC project of the same name available at http://csharpmarc.net, which itself is based on the File_MARC package (http://pear.php.net/package/File_MARC) by Dan Scott, which was based on PHP MARC package, originally called "php-marc", that is part of the Emilda Project (http://www.emilda.org). Both projects were released under the LGPL which allowed me to port the project to C# for use with the .NET Framework.

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. 

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

@author    Mattie Schraeder <mattie@csharpmarc.net>
@copyright 2016-2017 Mattie Schraeder
@license   http://www.gnu.org/licenses/gpl-3.0.html  GPL License 3