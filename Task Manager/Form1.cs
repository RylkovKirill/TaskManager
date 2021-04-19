using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;
using Microsoft.VisualBasic;

namespace Task_Manager
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        private List<Process> processes = null;
        private ListViewItemComparer comparer = null;


        public Form1()
        {
            InitializeComponent();
        }

        private void GetProcesses()
        {
            processes.Clear();
            processes = Process.GetProcesses().ToList<Process>();
        }

        private void RefreshProcessesList()
        {
            listView1.Items.Clear();

            double memorySize = 0;

            foreach (Process process in processes)
            {
                memorySize = 0;
                PerformanceCounter performanceCounter = new PerformanceCounter();
                performanceCounter.CategoryName = "Process";
                performanceCounter.CounterName = "Working Set - Private";
                performanceCounter.InstanceName = process.ProcessName;

                memorySize = (double)performanceCounter.NextValue() / (1000 * 1000);
                string[] row = new string[] { process.ProcessName.ToString(), Math.Round(memorySize, 1).ToString() };

                listView1.Items.Add(new ListViewItem(row));

                performanceCounter.Close();
                performanceCounter.Dispose();
            }

            Text = "Запущено процессов: " + processes.Count.ToString();
        }

        private void RefreshProcessesList(List<Process> processes, string keyword)
        {
            try
            {
                listView1.Items.Clear();

                double memorySize = 0;

                foreach (Process process in processes)
                {
                    if (process != null)
                    {
                        memorySize = 0;
                        PerformanceCounter performanceCounter = new PerformanceCounter();
                        performanceCounter.CategoryName = "Process";
                        performanceCounter.CounterName = "Working Set - Private";
                        performanceCounter.InstanceName = process.ProcessName;

                        memorySize = (double)performanceCounter.NextValue() / (1000 * 1000);
                        string[] row = new string[] { process.ProcessName.ToString(), Math.Round(memorySize, 1).ToString() };

                        listView1.Items.Add(new ListViewItem(row));

                        performanceCounter.Close();
                        performanceCounter.Dispose();
                    }
                }

                Text = $"Запущено процессов '{keyword}': " + processes.Count.ToString();

            }
            catch (Exception)
            {

            }
        }

        private void KillProcess(Process process)
        {
            process.Kill();

            process.WaitForExit();
        }

        private void KillProcessAndChildren(int processId)
        {
            if (processId == 0)
            {
                return;
            }

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + processId);
            ManagementObjectCollection objectCollection = managementObjectSearcher.Get();

            foreach (ManagementObject managementObject in objectCollection)
            {
                KillProcessAndChildren(Convert.ToInt32(managementObject["ProcessID"]));
            }

            try
            {
                Process process = Process.GetProcessById(processId);

                process.Kill();
                process.WaitForExit();
            }
            catch (ArgumentException)
            {
            }
        }

        private int GetParentProcessId(Process process)
        {
            int parentId = 0;

            try
            {
                ManagementObject managementObject = new ManagementObject("win32_process.handle='" + process.Id + "'");

                managementObject.Get();

                parentId = Convert.ToInt32(managementObject["ParentProcessId"]);
            }
            catch (Exception)
            {

            }
            return parentId;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            processes = new List<Process>();
            comparer = new ListViewItemComparer();
            comparer.ColumnIndex = 0;
            GetProcesses();
            RefreshProcessesList();

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GetProcesses();
            RefreshProcessesList();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    KillProcess(processToKill);
                    GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch (Exception)
            {

            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    KillProcessAndChildren(GetParentProcessId(processToKill));
                    GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch (Exception)
            {

            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            GetProcesses();
            List<Process> filteProcesses = processes.Where((x) => x.ProcessName.ToLower().Contains(toolStripTextBox1.Text.ToLower())).ToList<Process>();

            RefreshProcessesList(filteProcesses, toolStripTextBox1.Text);
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            comparer.ColumnIndex = e.Column;

            comparer.SortDirection = comparer.SortDirection == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            listView1.ListViewItemSorter = comparer;
            listView1.Sort();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void performanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new PerformanceForm();
            form.Show();
        }

        private void runTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Введите имя программы", "Запуск новой задачи");
            try
            {
                Process.Start(path);
            }
            catch (Exception)
            {

            }
        }
    }
}
