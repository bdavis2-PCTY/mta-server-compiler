using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace MTA_Compiler
{
    public partial class compileWindow : Form
    {
        public compileWindow()
        {
            InitializeComponent();
        }

        string path = "";
        public void create( string rPath)
        {
            outputLog("Opening compile GUI");
            path = rPath;
            label2.Text = "Compile Path: " + rPath.ToString();
            this.ShowDialog();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

        }



        private void outputLog (string log, bool sTime=true)
        {
            String now = "[" + string.Format("{0:HH:mm:ss}", DateTime.Now) + "] ";
            if (!sTime)
            {
                string temp = "";
                for (int i = 0; i < Math.Floor ( now.Length*1.8 ); i++){ temp += " ";}
                now = temp; temp = null;
            }
            richTextBox1.Text += "\n" + now + log;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            outputLog("Begining compilation... Preparing software");
            outputLog("-> Disabling GUI elements", false);
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            checkBox3.Enabled = false;
            checkBox4.Enabled = false;
            checkBox5.Enabled = false;
            checkBox6.Enabled = false;
            button1.Enabled = false;

            bool doServer = checkBox1.Checked;
            bool doClient = checkBox2.Checked;

            outputLog("-> Collecting & checking sub directories", false);
            string[] dirs_ = Directory.GetDirectories( path );

            // Resource counts
            int defaultResources = 0;
            int validResources = 0;

            // Script counts
            int success = 0;
            int skipped = 0;
            int failed = 0;

            // Compile options
            bool encrypt = checkBox4.Checked;
            bool antidecompile = checkBox5.Checked;
            bool meta = checkBox6.Checked;

            /*  
                -o		compile
                -b		AntiDec	
                -e		Encrypt 
             */

            string compileLine = "-o";
            if (encrypt && antidecompile){
                compileLine = "-b -e -o";
            } else if (encrypt && !antidecompile){
                compileLine = "-e -o";
            } else if (!encrypt && antidecompile){
                compileLine = "-b -o";
            }

            for (int i = 0; i < dirs_.Length; i++)
            {
                if (File.Exists(dirs_[i] + "\\meta.xml"))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(dirs_[i] + "\\meta.xml");
                    XmlNode c = doc.SelectSingleNode("meta");
                    XmlNodeList f = c.SelectNodes("script");
                    defaultResources++;
                    foreach (XmlNode it in f)
                    {
                        string type;
                        if (it.Attributes["type"] != null)
                        {
                            type = it.Attributes["type"].Value.ToLower();
                        }
                        else
                        {
                            type = "server";
                        }
                        string src = it.Attributes["src"].Value;
                        string src_ = dirs_[i] + "\\" + src;
                        if (Path.GetExtension(src_).ToString().ToLower() + "" == ".lua")
                        {
                            richTextBox1.Text += "\n";
                            outputLog("Loading file " + src_.ToString());
                            if ((type == "server" && doServer) || (type == "client" && doClient) || (type=="shared" && doClient && doServer))
                            {
                                outputLog(" -> Attempting to compile the script, creating process ", false);
                                
                                Process p = new Process();
                                p.StartInfo.FileName = "luac_mta.exe";
                                p.StartInfo.UseShellExecute = false;
                                p.StartInfo.Arguments = compileLine + " " + src_ + "c " + src_;
                                p.StartInfo.RedirectStandardOutput = true;
                                p.StartInfo.CreateNoWindow = true;
                                p.Start();

                                string ot = p.StandardOutput.ReadToEnd().ToString();

                                p.WaitForExit();    
                                if (ot != "")
                                {
                                    failed++;
                                    outputLog(" -> Script may have failed, results: " + ot, false);
                                } else {
                                    success++;
                                    outputLog ( " -> Script successfully compiled!", false );
                                    if (meta)
                                    {
                                        outputLog(" -> Updating meta.xml", false);
                                        it.Attributes["src"].Value = src + "c";

                                    }
                                    else
                                    {
                                        outputLog(" -> Skipping meta.xml update", false);
                                    }
                                }            
                            }
                            else
                            {
                                outputLog(" -> Skipping script - " + type + "-side scripts are not being compiled", false);
                                skipped++;
                            }
                        }
                    }

                    if (meta)
                    {
                        doc.Save(dirs_[i] + "\\meta.xml");
                    }
                }
            }

            richTextBox1.Text += "\n\n";
            outputLog("Detected resources: " + defaultResources.ToString() + " | Valid Resources: " + validResources.ToString());
            outputLog("Compiled Files: " + success.ToString() + " | Skipped Files: " + skipped.ToString() + " | Potential failed files: " + failed.ToString() );
            outputLog("Server Compilation has been complete!");
        }


        private void compileWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
