using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BG.Model
{
    partial class EconomicTaskType : ICloneable
    {

        public object Clone()
        {
            var clone = new EconomicTaskType();
            clone.Name = this.Name;
            foreach (var item in this.EconomicProject)
            {
                clone.EconomicProject.Add(item);
            }
            clone.EconomicTaskTypeId = this.EconomicTaskTypeId;
            foreach (var task in this.Task.ToList())
            {
                clone.Task.Add(task);
            }
            return clone;
        }

    }
}
