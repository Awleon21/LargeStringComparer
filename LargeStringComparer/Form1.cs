namespace LargeStringComparer
{

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FormSettings();
        }

        private void FormSettings()
        {
            this.Name = "Talend Tool";
            this.MaximizeBox = false;
            this.ResizeRedraw = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            rtbQuery2.WordWrap = true;
            rtbQuery1.WordWrap = true;
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            //Create a place in memory to hold the text from the two textboxes as lowercase values
            string lQuery1 = rtbQuery1.Text.ToLower();
            string lQuery2 = rtbQuery2.Text.ToLower();

            lQuery1.Trim(' ');
            lQuery2.Trim(' ');

            //Check to see if the text in the two text boxes are the same
            if (lQuery1.Equals(lQuery2, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("These things are the same");
            }
            else
            {
                //if the text in the two text boxes are not the same then add each line in the Text box to an array
                string[] Query1Columns = rtbQuery1.Lines;
                string[] Query2Columns = rtbQuery2.Lines;

                //Create lists to hole the lines from their respective textbox
                List<string> ListQuery1 = new List<string>();
                List<string> ListQuery2 = new List<string>();

                //Each line in the array is added to a list
                foreach (string thing in Query1Columns)
                {
                    ListQuery1.Add(thing);
                }

                //Each line in the array is added to a list
                foreach (string Thing in Query2Columns)
                {
                    ListQuery2.Add(Thing);
                }

                //Add the differences between the two text boxes to a list
                List<string> differencesQuery1 = ListQuery1.Except(ListQuery2).ToList();
                List<string> differencesQuery2 = ListQuery2.Except(ListQuery1).ToList();

                //Select each difference found in the list and change the color to red
                foreach (string difference in differencesQuery1)
                {
                    if (rtbQuery1.Text.Contains(difference))
                    {
                        rtbQuery1.Select(rtbQuery1.Text.IndexOf(difference), difference.Length);
                        rtbQuery1.SelectionColor = Color.Red;
                    }
                }

                //Select each difference found in the list and change the color to red
                foreach (string difference in differencesQuery2)
                {
                    if (rtbQuery2.Text.Contains(difference))
                    {
                        rtbQuery2.Select(rtbQuery2.Text.IndexOf(difference), difference.Length);
                        rtbQuery2.SelectionColor = Color.Red;
                    }
                }
            }
        }

        private void btnBeautify_Click(object sender, EventArgs e)
        {
            rtbQuery2.Clear();
            string lStringToParse = rtbQuery1.Text.ToUpper();

            string lPattern = @"(((\/\*)+?[\w\W]+?(\*\/)+))";
            string lReplacement = "";
            Regex lReg = new Regex(lPattern);
            string lResult = lReg.Replace(lStringToParse, lReplacement);

            List<string> lKeywords = new List<string>();

            lKeywords.Add(" FROM ");
            lKeywords.Add(" LEFT JOIN ");
            lKeywords.Add(" ELSE ");
            lKeywords.Add(" WHERE ");
            lKeywords.Add(" ON ");
            lKeywords.Add(" INNER JOIN ");
            lKeywords.Add(" GROUP BY ");
            lKeywords.Add(" SUM ");
            lKeywords.Add(" CASE ");
            lKeywords.Add(" THEN ");
            lKeywords.Add("--");
            lKeywords.Add("SELECT ");
            lKeywords.Add(" (SELECT ");
            lKeywords.Add(" WHEN ");
            lKeywords.Add(" AND ");

            int count = 0;
            foreach (string word in lKeywords)
            {
                if (lResult.ToUpper().Contains(word))
                {
                    if (word == "SELECT " && count == 0)
                    {
                        lResult = lResult.Replace(word, string.Concat(word, Environment.NewLine));
                        count++;
                    }
                    else
                    {
                        lResult = lResult.Replace(word, string.Concat(Environment.NewLine, word));
                    }
                }
            }

            lResult = lResult.Replace(",", string.Concat(",", Environment.NewLine)).Replace("GROUP BY", string.Concat("GROUP BY", Environment.NewLine));
            rtbQuery2.AppendText(lResult);
        }

        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {
            string TextToCopy = rtbQuery2.Text;

            CopyTextToClipboard(TextToCopy);
        }

        private void CopyTextToClipboard(string TextToBeCopied)
        {
            Clipboard.SetText(TextToBeCopied);
        }

        private void btnAddCommas_Click(object sender, EventArgs e)
        {
            rtbQuery2.Clear();

            string[] lColumns = rtbQuery1.Lines;
            int Count = 0;
            foreach (string Column in lColumns)
            {
                if (Column.Length != 0)
                {
                    if (Count < lColumns.Length - 1)
                    {
                        string ColumnComma = Column.Replace(Column, string.Concat(Column, ",", Environment.NewLine));
                        rtbQuery2.Text += ColumnComma;
                        Count++;
                    }
                    else
                    {
                        rtbQuery2.Text += Column;
                    }
                }
            }
        }

        private void btnCountColumns2_Click(object sender, EventArgs e)
        {
            string[] Lines = rtbQuery2.Lines;
            string[] Lines2 = rtbQuery1.Lines;
            MessageBox.Show(string.Format("Box 1 has {0} Columns, and Box 2 has {1} Columns", Lines.Length.ToString(), Lines2.Length.ToString()));
        }

        private void btnResetColor_Click(object sender, EventArgs e)
        {
            int first = (rtbQuery1.Text.Length - rtbQuery1.Text.Length);
            int first2 = (rtbQuery2.Text.Length - rtbQuery2.Text.Length);
            rtbQuery1.Select(first, rtbQuery1.Text.Length);
            rtbQuery1.SelectionColor = Color.Black;

            rtbQuery2.Select(first2, rtbQuery2.Text.Length);
            rtbQuery2.SelectionColor = Color.Black;
        }      

        private void btnBeautifyThisBox_Click(object sender, EventArgs e)
        {
            string lStringToParse = rtbQuery1.Text.ToUpper();

            ParseQuery(lStringToParse, rtbQuery1);
        }

        private void btnBeautifyThisBox2_Click(object sender, EventArgs e)
        {
            string lStringToParse = rtbQuery2.Text.ToUpper();

            ParseQuery(lStringToParse, rtbQuery2);            
        }

        private void ParseQuery(string lStringToParse, RichTextBox rtbQuery)
        {
            string lPattern = @"(((\/\*)+?[\w\W]+?(\*\/)+))";
            string lReplacement = "";
            Regex lReg = new Regex(lPattern);
            string lResult = lReg.Replace(lStringToParse, lReplacement);

            //Finds all white space including newlines and replaces with a single instance of the character, for instance TAB TAB TAB will replace with just TAB
            lResult = Regex.Replace(lResult, @"(\s)\s+", "$1");

            List<string> lKeywords = new List<string>();

            #region List Of Keywords
            lKeywords.Add(" FROM ");
            lKeywords.Add("LEFT JOIN");
            lKeywords.Add("LEFT OUTER JOIN");
            lKeywords.Add(" ELSE ");
            lKeywords.Add(" WHERE ");
            lKeywords.Add(" ON ");
            lKeywords.Add(" INNER JOIN ");
            lKeywords.Add("INNER JOIN");
            lKeywords.Add(" GROUP BY ");
            lKeywords.Add(" SUM ");
            lKeywords.Add(" CASE ");
            lKeywords.Add(" THEN ");
            lKeywords.Add("--");
            lKeywords.Add("SELECT ");
            lKeywords.Add(" (SELECT ");
            lKeywords.Add(" WHEN ");
            lKeywords.Add(" AND "); 
            #endregion

            int count = 0;
            foreach (string word in lKeywords)
            {
                if (lResult.ToUpper().Contains(word))
                {
                    if (word == "SELECT " && count == 0)
                    {
                        lResult = lResult.Replace(word, string.Concat(word, Environment.NewLine));
                        count++;
                    }
                    else
                    {
                        lResult = lResult.Replace(word, string.Concat(Environment.NewLine, word));
                    }
                }
            }

            rtbQuery.Clear();
            lResult = lResult.Replace(",", string.Concat(",", Environment.NewLine)).Replace("GROUP BY", string.Concat("GROUP BY", Environment.NewLine)).Replace(@"\\R\\N", "").Replace(@"\\T", "");

            rtbQuery.AppendText(lResult);

            string[] Lines = rtbQuery.Lines;

            foreach (string line in Lines)
            {
                int first = (line.Length - line.Length);
                if (line.StartsWith("--"))
                {
                    rtbQuery.Select(rtbQuery.Text.IndexOf(line), line.Length);
                    rtbQuery.SelectionColor = Color.Green;
                }
            }
        }

        #region Clear Buttons
        private void button1_Click(object sender, EventArgs e)
        {
            rtbQuery1.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            rtbQuery2.Clear();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            rtbQuery1.Clear();
            rtbQuery2.Clear();
        }

        #endregion
    }
}
