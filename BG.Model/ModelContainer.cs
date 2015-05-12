using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BG.Model
{
    partial class ModelContainer
    {

        public ModelContainer(bool isLocal)
            : base(isLocal ? 
            "metadata=res://*/Model.csdl|res://*/Model.ssdl|res://*/Model.msl;provider=System.Data.SqlClient;provider connection string=\"data source=(localdb)\\v11.0;initial catalog=BG;integrated security=True;multipleactiveresultsets=True;App=EntityFramework\"" 
            :
            "metadata=res://*/Model.csdl|res://*/Model.ssdl|res://*/Model.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source=mssql3.unoeuro.com; Initial Catalog= bganlaegsteknikapp_dk_db; Integrated Security=false;user id=bganlaegste_dk;password=BGanlaegsteknik12;MultipleActiveResultSets=True;App=EntityFramework\"", "ModelContainer")
        {

            this.ContextOptions.LazyLoadingEnabled = true;

        }
    }
}
