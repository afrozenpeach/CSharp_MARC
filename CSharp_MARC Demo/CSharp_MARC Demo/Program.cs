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
 * @author    Matt Schraeder <frozen@frozen-solid.net> <mschraeder@btsb.com>
 * @copyright 2009-2012 Matt Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
 * @license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC;
using System.IO;

namespace CSharp_MARC_Demo
{
	class Program
	{
		static void Main(string[] args)
		{
			//Read raw MARC record from a file. 
			//The example .mrc files are in the project's root.
			string rawMarc = File.ReadAllText("..\\..\\..\\record.mrc");

			//The FileMARC class does the actual decoding of a record and splits a string of multiple records into a list object. 
			//Decoding is not done until you actually access a single record from the FileMARC object.

			//You can import records straight from a string in memory. The string can have one or many MARC records.
			FileMARC marcRecords = new FileMARC(rawMarc);

			//Or you can import it straight from a file
			marcRecords.ImportMARC("..\\..\\..\\record2.mrc");

			//You can get how many records were found by using the Count property
			Console.WriteLine("Found " + marcRecords.Count + " records.");

			//You can access each individual record in it's native MARC format using the RawSource object
			Console.WriteLine("Here is the first record:");
			Console.WriteLine(marcRecords.RawSource[0]);

			//You can access each record manually using array notation.
			Record firstRecord = marcRecords[0];

			//Or you can loop through them as an Enumerable object
			//Note: I recommend only retrieving each record from the FileMARC object once as each time you do it will be decoded. 
			int i = 0;
			foreach (Record record in marcRecords)
			{
				//The Warnings property contains a list of issues that the decoder found with the record. 
				//The decoder attempts to return a valid MARC record to the best of it's ability.
				Console.WriteLine("Book #" + ++i + " has been decoded with " + record.Warnings.Count +" errors!");

				//Once decoded you can easily access specific data within the record, as well as make changes.

				//Array notation we will get the first requested tag in the record, or null if one does not exist.
				//First we'll get the Author.  Since there should only be one author tag array notation is the easiest to use.
				Field authorField = record["100"];
				
				//Each tag in the record is a field object. To get the data we have to know if it is a DataField or a ControlField and act accordingly.
				if (authorField.IsDataField())
				{
					DataField authorDataField = (DataField)authorField;
					//The author's name is in subfield a.  Once again since there should only be one we can use array notation.
					Subfield authorName = authorDataField['a'];
					Console.WriteLine("The author of this book is " + authorName.Data);
				}
				else if (authorField.IsControlField())
				{
					//Unreachable code!
					Console.WriteLine("Something went horribly wrong. The author field should never be a Control Field.");
				}

				//Now we will get the subjects for this record. Since a book can have multiple subjects we will use GetFields() which returns a List<Field> object.
				//Note: Not passing in a tag number to GetFields will return all the tags in the record.
				List<Field> subjects = record.GetFields("650");

				Console.WriteLine("Here are the subjects for Book #" + i);
				//Here we will assume each Field is actually a DataField since ISBNs should always be a DataField.
				foreach (DataField subject in subjects)
				{
					string subjectText = string.Empty;

					//We also want to loop through each subfield.
					//Just like with GetFields() you can either pass in a subfield value, or nothing to get all the subfields
					foreach (Subfield subfield in subject.GetSubfields())
						subjectText += subfield.Data + " ";

					Console.WriteLine(subjectText);
				}
			}

			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();
		}
	}
}
