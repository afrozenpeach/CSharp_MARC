/**
 * Parser for MARC records
 *
 * This project is based on the File_MARC package 
 * (http://pear.php.net/package/File_MARC) by Dan Scott , which was based on PHP
 * MARC package, originally called "php-marc", that is part of the Emilda 
 * Project (http://www.emilda.org). Both projects were released under the LGPL
 * which allowed me to port the project to C# for use with the .NET Framework.
 * 
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * @author    Matt Schraeder <mschraeder@csharpmarc.net> <mschraeder@btsb.com>
 * @copyright 2009-2012 Matt Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
 * @license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3
 */

2013-04-12 Changes:
More support for special MARC8 characters thanks to my new MARC8 Encoding class. Special thanks to Mark V. Sullivan of Sobek CM MARC Library (https://sourceforge.net/projects/marclibrary/) for code used to read the MARC8 format, which much of the Encoding was based on.

2013-04-11 Changes:
Fix issue relating to the FileMARCWriter needing to write multiple bytes per character in some cases, as well as handling encoding of non-special characters better.

2013-04-10 Changes:
Some special characters in a normal MARC21 file are saved using non-standard character encoding. The proper encoding should be MARC8, which .Net does not support. This version adds support for a select few special characters, most importantly the copyright symbol which is used in RDA records. The FileMARCWriter class makes it easier to write records and should follow MARC8/UTF-8 specifications from the 9th character of the Leader. The FileMARCReader class had to be updated to read these MARC8 characters back in correctly.

Initial commit of FileMARCWriter
Updated FileMARCReader to support features of FileMARCWriter
UTF-8 support is only loosely tested. If you are using UTF-8 please let me know how well it works for you.
 
2012-06-13 Changes:
Added FileMARCReader class for handling large MARC21 files without loading the entire file into memory. Special thanks to Stas Paladiy for reporting this issue and helping resolve it.
Updated Copyright information for the year 2012!

2011-09-12.2 Changes:
Apparently not even Library of Congress MARCXML records follow the XSD specification.  I've removed the requirement that records validate before being added.
No longer ordering the tags when importing a MARCXML record.

2011-09-12.1 Changes:

CSharp_MARC can now read MARCXML files.  The FileMARCXML class can accept XML strings or native .NET XDocument objects and convert them to Record objects.
The FileMARC(string source) constructor and ImportMARC function were duplicating code from the FileMARC.Add(string source) function. The coding horror is me :negative:

2011-06-30 Change:

Added Clone() to Fields, Subfields, and Records. This is a DEEP clone and all members should be properly cloned as new instances.

2011-04-26 Change:

Changed how validating a tag works when decoding due to "not thinking syndrome"

2011-03-04 Changes:

Updated copyright information
Added x86 Platform for easier testing. Apparently it didn't stick when I added it earlier.
Testing suite to test the class structure and included methods. This will get more advanced as I port specific tests from File_MARC.
Added a bit of extra error checking to a Field's tag so that it's not possible to assign an invalid tag.
If the FileMARC parser comes across an invalid tag, it forces the tag to "ZZZ" and should no longer throw an exception. 
Made Field.IsEmpty() abstract. It's not possible to assign a Field with an empty string tag. Doing so should cause an exception. If this is in fact the case, then IsEmpty on Field should never return true. Because of this IsEmpty is now an override on both DataField and ControlField. Field.IsEmpty now returns the result of it's inherited version.
ToString functions for ControlField and DataField changed to overrides. Field.ToString seemed odd to only output the tag and not the inner data if available.
Fixed bug in ControlField.IsEmpty() returning opposite of expected results. Amazing how much unit testing helps!
DataField.FormatField() with no exclude codes as parameters actually works now.
Added a bit of extra error checking and cleanup to the indicators in the decode function.
Indicators are now closer to the MARC21 standard, allowing both numbers and letters. Uppercase letters are forced to lowercase. # is no longer a valid character, as it is supposed to indicate an ASCII SPACE. Invalid characters are automatically changed to ASCII SPACE and warnings are added to indicate this
Moved warnings into the Record object rather than FileMARC. This makes it easier to track which warnings were for which Record. This change may be API breaking and I apologize for that.
Fixed a bug where it is possible to make an invalid record by setting the Leader to a string longer than 24 characters. Now it will still allow you to set a long Leader but will only output the first 24 characters.
Fixed a bug where if the Leader was less than 24 characters it would make an invalid record.
You can now make tags that are not exactly 3 characters. It will automatically pad the leading 0s.
Field.ToString() is now an abstract override rather than an abstract new. No idea what I was thinking with it as new.
Record.ToString() doesn't have to check what type it is, thanks to override!
Totally had the IEnumerable designed wrong. Reset should reset to -1, as MoveNext will take you to the first record.  After a reset it was ignoring the first record. My bad!

2011-02-16 Changes:

Added personal email to copyright notices.
Updated copyright years
Added an overloaded GetSubfields() so you no longer have to pass in null to get all subfields.
Changed Warnings from protected to public. No idea what I was thinking there.
Tons more error checking in the FileMARC decode function due to coming across an egregiously bad record.
Now with fancy new Demo application! Hopefully it has enough example code to get people started better.
Updated solution files to VS2010
Probably some other stuff.