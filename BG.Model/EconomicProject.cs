using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BG.Model
{
    partial class EconomicProject : ICloneable
    {
        public object Clone()
        {
            var project = new EconomicProject();
            project.EconomicCache = this.EconomicCache;
            project.EconomicProjectId = this.EconomicProjectId;
            project.Name = this.Name;
            foreach(var taskType in this.EconomicTaskTypes.ToList())
            {
                project.EconomicTaskTypes.Add(taskType);
            }
            return project;
        }
    }
}
