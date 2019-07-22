using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    public class Vagon
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]       //Id'nin otomatik artmasını engeller.
        public int VagonID { get; set; }
        public int VagonTipiID { get; set; }

        public virtual List<TrenVagon> VagonunTrenleri { get; set; }
        public virtual VagonTipi VagonTipi { get; set; }
    }
}
