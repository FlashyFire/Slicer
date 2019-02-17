using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Slicer.ObjectModel;
using Slicer.Import;

namespace Slicer
{
    public partial class Form1 : Form
    {
        private readonly Model model = new Model();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void FileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FileOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                STLImporter importer = new STLImporter(model);
                importer.Import(openFileDialog1.FileName);

            }
        }
    }
}
