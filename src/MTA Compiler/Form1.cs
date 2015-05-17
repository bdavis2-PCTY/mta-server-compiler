using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MTA_Compiler;

namespace MTA_Compiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string resourcePath = "";
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description = "Select your Multi Theft Auto resources folder";
            folderBrowserDialog1.ShowNewFolderButton = false;
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                resourcePath = textBox1.Text;
                button2.Enabled = true;
            }
            else
            {
                MessageBox.Show("Folder not selected!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // check for existing directory
            if (!Directory.Exists(resourcePath))
            {
                MessageBox.Show("That folder path doesn't exist. Please select a valid path by clicking \"Browse\"", "Error");
                return;
            }

            /* var cR = MessageBox.Show("Are you sure that the MTA resource directory is\n" + resourcePath.ToString() + "?", "Confirmation", MessageBoxButtons.YesNo );
            if (cR != DialogResult.Yes)
            {
                return;
            } */

            this.Hide();

            // create compile form
            compileWindow f = new compileWindow();
            f.create(resourcePath);
            resourcePath = null;
            f = null;
        }
    }
}
