using CompiPascal.Analysers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CompiPascal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string inputString = txtInputEditor.Text.ToString();

            Syntax syntax = new Syntax();
            syntax.Analyze(inputString);

            if (syntax.resultsList.Count > 0)
            {
                foreach (string result in syntax.resultsList)
                {
                    txtOutputEditor.Text += result.ToString();
                    txtOutputEditor.Text += Environment.NewLine;
                }
            }
            if (syntax.errorsList.Count > 0) 
            {
                foreach(string error in syntax.errorsList) 
                {
                    txtOutputEditor.Text += error;
                    txtOutputEditor.Text += Environment.NewLine;
                }
            }
        }
    }
}
