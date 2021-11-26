using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotsCollector
{
    public partial class Form1 : Form
    {
        List<string> sites = new List<string>();
        List<string> words = new List<string>();
        public Form1()
        {
            InitializeComponent();

            var domains = File.ReadLines(@"Domains.txt");
            foreach (string domain in domains)
            {
                listBox1.Items.Add(domain);
            }
        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            sites.AddRange(listBox1.Items.Cast<String>().ToList());
            PerformRobotInspector();
        }

        private async void PerformRobotInspector()
        {

            foreach (string site in sites)
            {
                listBox1.Items.Remove(site);

                string res = await GetRobot(site);
                listBox2.Items.Add(site);

                foreach (string resline in res.Split(Environment.NewLine))
                {
                    string line = resline.ToLower();
                    if (!line.StartsWith("#") || !line.StartsWith(" "))
                    {
                        var wordz = new string(line.Select(c => char.IsLetterOrDigit(c) || c == '_' ? c : ' ').ToArray())
                            .Split()
                            .Where(s => !String.IsNullOrWhiteSpace(s));

                        foreach (string s in wordz)
                        {
                            if (s.Length > 4 && s.Length < 15)
                            {
                                if (!words.Contains(s))
                                {
                                    words.Add(s);
                                    listBox3.Items.Add(s + Environment.NewLine);
                                    label1.Text = $"Words: {words.Count}";
                                }
                            }
                        }
                    }
                }
            }
        }
        private static HttpClient Client = new HttpClient();
        string response = string.Empty;
        private async Task<string> GetRobot(string site)
        {
            string result = string.Empty;
            string url = $"http://{site}/robots.txt";
            try
            {
                result = await Client.GetStringAsync(url);
            }
            catch (Exception ex)
            {

            }


            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            File.WriteAllLines("export.txt", words, Encoding.UTF8);
        }
    }
}
