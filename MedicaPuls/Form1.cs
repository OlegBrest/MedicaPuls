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
        public struct pulsdavl
        {
            public double pulse;
            public double davl_high;
            public double davl_low;
        }
        public MainForm()
        {
            InitializeComponent();
            this.dataGridView1.Columns.Add("first", "Файл");
            this.dataGridView1.Columns.Add("secnd", "Пульс");
            this.dataGridView1.Columns.Add("third", "Систола");
            this.dataGridView1.Columns.Add("fourth", "Диастола");
        }

        private void bttn_open_file_Click(object sender, EventArgs e)
        {
            
            openFileDialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string [] filenames = openFileDialog.FileNames;
            // читаем файл в строку
            foreach (string filename in filenames)
            {
                string fileText = System.IO.File.ReadAllText(filename);

                string[] stroki = fileText.Split('\n');
                int zapisei = stroki.Length;
                int[,] cifrmass = new int[stroki.Length, 5];
                for (int i = 0; i < zapisei; i++)
                {
                    if (stroki[i].Trim() != "")
                    {
                        // adding to int array our mesuares (0-pressure, 1 - pulse , 2 - time , 3 - finder for start and end mes, 4 - normalize)
                        cifrmass[i, 0] = Convert.ToInt32(stroki[i].Split(' ')[0]);
                        cifrmass[i, 1] = Convert.ToInt32(stroki[i].Split(' ')[1]);
                        cifrmass[i, 2] = Convert.ToInt32(stroki[i].Split(' ')[2]);
                        cifrmass[i, 3] = cifrmass[i, 1] - cifrmass[i, 0];
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



                for (int i = (zapisei - 2); i > 0; i--)
                {
                    if ((cifrmass[i, 3] > cifrmass[i + 1, 3]) && (cifrmass[i + 1, 3] < 0))
                    {
                        ending_pos = i - 1;
                        break;
                    }
                }

                int median_start = 0;
                int median_end = 0;
                for (int i = starting_pos; i < (starting_pos + 10); i++)
                {
                    median_start += cifrmass[i, 1];
                }
                median_start /= 10;
                for (int i = (ending_pos - 10); i < ending_pos; i++)
                {
                    median_end += cifrmass[i, 1];
                }
                median_end /= 10;

                double delta_puls = median_start - median_end;
                double delta_id = ending_pos - starting_pos;
                int[] ToPulse = new int[ending_pos - starting_pos];

                for (int i = starting_pos; i < ending_pos; i++)
                {
                    cifrmass[i, 4] = cifrmass[i, 1] + (int)((i - starting_pos) * delta_puls / delta_id);
                    ToPulse[i - starting_pos] = cifrmass[i, 4];
                    //   this.dataGridView1.Rows.Add(cifrmass[i, 0], cifrmass[i, 1], cifrmass[i, 2]
                    //       , cifrmass[i, 3], cifrmass[i, 4]);
                }
                pulsdavl pd = GetPulse(ToPulse.Average() * 1.1, cifrmass, starting_pos, ending_pos);
                this.dataGridView1.Rows.Add(filename, pd.pulse, pd.davl_high, pd.davl_low);
                //MessageBox.Show("High="+pd.davl_high.ToString() + "\nLow=" + pd.davl_low + "\nPulse=" + pd.pulse);
            }
        }

        private pulsdavl GetPulse(double tochn, int [,] mass, int start, int end)
        {
            double pulse = 0;
            double start_time = 0;
            double end_time = 0;
            double count = 0;
            double high_davl = 0;
            double low_davl = 0;
            for (int i = start; i < end-1; i++)
            {
                if((mass[i,4]<tochn) && (mass[i+1,4]>tochn))
                {
                    if (start_time == 0)
                    {
                        start_time = mass[i, 2];
                        high_davl = mass[i, 0];
                    }
                    count++;
                }

                if ((mass[i, 4] > tochn) && (mass[i + 1, 4] < tochn) && (start_time!=0))
                {
                    end_time = mass[i, 2];
                    low_davl = mass[i, 0];
                }
            }
            pulse = (count / ((end_time - start_time) / 1000)) * 60;
            pulsdavl pd_res= new pulsdavl();
            pd_res.pulse = pulse;
            pd_res.davl_low = low_davl/3.5;
            pd_res.davl_high = high_davl/3.5;
            return pd_res;
        }


        private void bttn_clr_Click(object sender, EventArgs e)
        {
            int coun = this.dataGridView1.Rows.Count;
            for (int i = 0; i < coun; i++) this.dataGridView1.Rows.RemoveAt(0);
        }
    }
}
