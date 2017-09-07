using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedicaPuls
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.dataGridView1.Columns.Add("first", "Давление");
            this.dataGridView1.Columns.Add("secnd", "Пульс");
            this.dataGridView1.Columns.Add("third", "Время");
            this.dataGridView1.Columns.Add("fourth", "Поиск");
        }

        private void bttn_open_file_Click(object sender, EventArgs e)
        {
            
            this.openFileDialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog.FileName;
            // читаем файл в строку
            string fileText = System.IO.File.ReadAllText(filename);
            
            string[] stroki = fileText.Split('\n');
            int zapisei = stroki.Length;
            int [,] cifrmass = new int[stroki.Length,4];
            for (int i = 0; i< zapisei;i++)
            {
                if (stroki[i].Trim() != "")
                {
                    // adding to int array our mesuares (0-pressure, 1 - pulse , 2 - time , 3 - finder for start and end mes)
                    cifrmass[i, 0] = Convert.ToInt32(stroki[i].Split(' ')[0]);
                    cifrmass[i, 1] = Convert.ToInt32(stroki[i].Split(' ')[1]);
                    cifrmass[i, 2] = Convert.ToInt32(stroki[i].Split(' ')[2]);
                    cifrmass[i, 3] = cifrmass[i, 1]- cifrmass[i, 0];
                }
            }
            int max_on_pulse = 0;
            int min_on_pulse = 0;
            int starting_pos = 0;
            int ending_pos = 0;
            for (int i = 0; i < zapisei; i++)
            {
                if (cifrmass[i, 1] > max_on_pulse) max_on_pulse = cifrmass[i, 1];
                if (min_on_pulse > cifrmass[i, 3])
                {
                    min_on_pulse = cifrmass[i, 3];
                }
                if ((starting_pos == 0) && (min_on_pulse < cifrmass[i, 3]) && (cifrmass[i, 3] < 0)) starting_pos = i;
            }



            for (int i = (zapisei-2); i > 0; i--)
            {
                if ((cifrmass[i, 3] > cifrmass[i+1, 3]) && (cifrmass[i+1, 3]< 0))
                {
                    ending_pos = i-1;
                    break;
                }
            }

            for (int i = starting_pos; i < ending_pos; i++)
            {
                this.dataGridView1.Rows.Add(cifrmass[i, 0], cifrmass[i, 1], cifrmass[i, 2], cifrmass[i, 3]);
            }
        }

        private void bttn_clr_Click(object sender, EventArgs e)
        {
            int coun = this.dataGridView1.Rows.Count;
            for (int i = 0; i < coun; i++) this.dataGridView1.Rows.RemoveAt(0);
        }
    }
}
