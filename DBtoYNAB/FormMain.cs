using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBtoYNAB
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                ConvertCSV();
            }
        }

        private void ConvertCSV()
        {
            string outputFileName = openFileDialog1.FileName.Substring(0,
                    openFileDialog1.FileName.LastIndexOf('.')) + "_ynab.csv";

            using (StreamWriter outputFile = 
                new StreamWriter(
                    File.Open(outputFileName, 
                    FileMode.OpenOrCreate, 
                    FileAccess.Write)))
            {
                using (StreamReader inputFile = new StreamReader(openFileDialog1.OpenFile(), true))
                {
                    string line = string.Empty;

                    while ((line = inputFile.ReadLine()) != null && !line.Contains("Buchungstag")) ;

                    outputFile.WriteLine("Date,Payee,Category,Memo,Outflow,Inflow");

                    while ((line = inputFile.ReadLine()) != null && !line.Contains("Kontostand"))
                    {
                        string[] splittedLine = line.Split(';');

                        if (splittedLine.Length < 17)
                            continue;

                        List<string> resultLineValues = new List<string>();

                        DateTime bookinDate = DateTime.Now;

                        DateTime.TryParse(splittedLine[0], out bookinDate);

                        resultLineValues.Add(bookinDate.Month + "/" + bookinDate.Day + "/" + bookinDate.Year);
                        resultLineValues.Add(splittedLine[3].Replace(",", " "));
                        resultLineValues.Add("");
                        resultLineValues.Add(splittedLine[4].Replace(",", " "));

                        decimal outflow = 0;

                        if (decimal.TryParse(splittedLine[15], NumberStyles.Any, new CultureInfo("de-DE"), out outflow))
                        {
                            outflow = outflow * -1;

                            resultLineValues.Add(outflow.ToString(new CultureInfo("en-US")));
                        }
                        else
                        {
                            resultLineValues.Add("");
                        }

                        decimal inflow = 0;

                        if (decimal.TryParse(splittedLine[16], NumberStyles.Any, new CultureInfo("de-DE"), out inflow))
                        {
                            resultLineValues.Add(inflow.ToString(new CultureInfo("en-US")));
                        }
                        else
                        {
                            resultLineValues.Add("");
                        }

                        outputFile.WriteLine(string.Join(",", resultLineValues));
                    }

                }
            }

            MessageBox.Show("Output file was successfully written to disk (" + outputFileName + ")");
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
