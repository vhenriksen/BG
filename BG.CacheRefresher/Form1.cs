using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BG.Controller;
using BG.Model;

namespace BG.CacheRefresher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            MessageBox.Show(
                "Cachen opdateres nu. Dette kan godt tage op til 30 minutter. Du vil få besked når processen er fuldført.",
                "Cache update", MessageBoxButtons.OK, MessageBoxIcon.Information);

            progressBar1.Visible = true;

            //True for at opdatere cache lokalt, false for at opdatere på serveren (manuelt).
            var service = new Service(false);
        
            var cache = service.Container.EconomicCacheSet.FirstOrDefault();
            var oldUpdatedValue = new DateTime(2000, 1, 1);
            if (cache == null)
            {
                cache = EconomicCache.CreateEconomicCache(0, oldUpdatedValue);
                service.Container.EconomicCacheSet.AddObject(cache);
                service.Container.SaveChanges();
            }
            else
            {
                foreach (var project in cache.EconomicProject.ToList())
                {
                    foreach (var taskType in project.EconomicTaskTypes.ToList())
                    {
                        foreach (var task in taskType.Task.ToList())
                        {
                            service.Container.DeleteObject(task);
                        }
                        service.Container.DeleteObject(taskType);
                    }
                    service.Container.DeleteObject(project);
                }
                cache.LastUpdated = oldUpdatedValue;
                service.Container.SaveChanges();
            }

            service.RefreshCache();
            //service.UpdateCache();
            progressBar1.Visible = false;

            MessageBox.Show(
                "Cachen er nu opdateret!",
                "Cache update", MessageBoxButtons.OK, MessageBoxIcon.Information);

            button1.Enabled = true;
        }
    }
}
