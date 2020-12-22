using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MARC;
using SwiftExcel;

namespace PMarcWin
{
    public partial class Form1 : Form
    {
        private FileMARC marcRecords = new FileMARC();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result=openFileDialog1.ShowDialog();
            if(result== DialogResult.OK)
            {
                //result=MessageBox.Show("Dafk");
                marcRecords.ImportMARC(openFileDialog1.FileName);
                if(marcRecords.Count>0)
                {
                    lblStatus.Text = "Läser in MARC-fil...";
                    ListRecords(txtFilter.Text);
                }
            }
        }
        private void ListRecords(string libraryFilter="")
        {
            lvRecords.Items.Clear();
            /**
                100 a (author)
                245 a (title)
                245 b (title)
                942 c (item type)
                952 a (permanent location)
                952 c (shelving location)
            **/
            /**lvRecords.Columns.Add("Författare");
            lvRecords.Columns.Add("Titel");
            lvRecords.Columns.Add("Typ");
            lvRecords.Columns.Add("Bibliotek");
            lvRecords.Columns.Add("Placering");**/
            foreach (Record record in marcRecords)
            {
                Field authorField = record["100"];
                Field titleField = record["245"];
                Field typeField = record["942"];
                Field locationField = record["952"];

                string Author;
                string CoAuthors ="";
                string Title;
                string Type;
                string Library;
                string Placement;

                #region authorfield
                if (authorField == null)
                {
                    Author = "";
                }
                else
                {
                    if(authorField.IsDataField())
                    {
                        DataField authorDataField = (DataField)authorField;
                        Subfield authorName = authorDataField['a'];
                        Author = authorName.Data;
                    }
                    else
                    {
                        Author = "";
                    }
                }
                #endregion

                #region titleField
                Title = "";
                if (titleField == null)
                {
                    Title = "";
                }
                else
                {
                    if (titleField.IsDataField())
                    {
                        DataField titleDataField = (DataField)titleField;
                        Subfield titleA = titleDataField['a'];
                        Subfield titleB = titleDataField['b'];
                        Subfield coAuthorsField = titleDataField['c'];

                        if (titleA != null)
                        {
                            Title += titleA.Data;
                            if (Title.EndsWith("/"))
                            {
                                Title = Title.Substring(0, Title.Length - 1);
                            }
                            Console.WriteLine($"titleA='{titleA.Data}'");
                        }
                        if(titleB != null)
                        {
                            
                            Title += titleB.Data;
                            if (Title.EndsWith("/"))
                            {
                                Title = Title.Substring(0,Title.Length-1);
                            }
                                Console.WriteLine($"titleB='{titleB.Data}'");
                        }
                        if(coAuthorsField!=null)
                        {
                            CoAuthors = coAuthorsField.Data;
                        }
                    }
                    else
                    {
                        Title = "";
                    }
                }
                #endregion

                #region typeField
                Type = "";
                if (typeField != null)
                {
                    if (typeField.IsDataField())
                    {
                        DataField typeDataField = (DataField)typeField;
                        Subfield typeData = typeDataField['c'];
                        if(typeData != null)
                        {
                            Type = typeData.Data;
                        }
                        
                    }
                    else
                    {
                        Type = "";
                    }
                }
                #endregion

                #region locationField
                Library = "";
                Placement = "";
                if (locationField != null)
                {
                    if (locationField.IsDataField())
                    {
                        DataField locationDataField = (DataField)locationField;
                        Subfield libraryData = locationDataField['a'];
                        Subfield placementData = locationDataField['c'];
                        if(libraryData!=null)
                        {
                            Library = libraryData.Data;
                        }
                        if(placementData!=null)
                        {
                            Placement = placementData.Data;
                        }
                        
                    }
                    else
                    {
                        Library = "";
                        Placement = "";
                    }
                }
                #endregion

                if(Author=="")
                {
                    Author = CoAuthors;
                }
                if(libraryFilter=="" || Library == libraryFilter)
                {
                    lvRecords.Items.Add(new ListViewItem(new string[] { Author, Title, Type, Library, Placement }));
                }
                
            }
            lblStatus.Text = "Läste in MARC-fil. Klicka på Exportera för att spara som en Excelfil.";
            lblNumRecords.Text = lvRecords.Items.Count.ToString();
            btnExport.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if(lvRecords.Items.Count>0)
            {
                var result = saveFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ExportListView(saveFileDialog1.FileName);
                }
                
            }
            
        }

        private void ExportListView(string FileName)
        {

            int xlRow = 1;

            using (var ew = new ExcelWriter(FileName))
            {
                for (int n = 0; n < lvRecords.Columns.Count; n++)
                {
                    ew.Write($"{lvRecords.Columns[n].Text}", n + 1, xlRow);
                }

                for (int itemrow = 0; itemrow < lvRecords.Items.Count; itemrow++)
                {
                    for (int itemcol = 0; itemcol < lvRecords.Columns.Count; itemcol++)
                    {
                        ew.Write($"{lvRecords.Items[itemrow].SubItems[itemcol].Text}", itemcol + 1, itemrow + 2);
                    }
                }
            }
            lblStatus.Text = $"Klar! Exporterade {FileName}";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var count = 0;
            for(int n=lvRecords.Items.Count-1;n>=0;n--)
            {
                if(lvRecords.Items[n].Selected)
                {
                    lvRecords.Items[n].Remove();
                    count++;
                }
            }
            lblStatus.Text = $"Raderade {count} poster från listan";
            lblNumRecords.Text = lvRecords.Items.Count.ToString();
            
        }
    }
}
