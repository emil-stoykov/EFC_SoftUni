using System;
using System.Collections.Generic;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class DepartmentImportModel
    {
        public string Name { get; set; }
        public ICollection<CellImportModel> Cells { get; set; }
    }
}
