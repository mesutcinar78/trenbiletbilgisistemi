using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public class OrtakMetodlar
    {
        public static bool BosAlanVarMi(GroupBox grp)
        {
            foreach (Control item in grp.Controls)
            {
                if (item is TextBox)
                {
                    if (item.Text == "")
                        return true;
                }
                else if(item is ComboBox)
                {
                    if(((ComboBox)item).Text == "")
                    {
                        return true;
                    }
                }

                else if (item is NumericUpDown)
                {
                    if (((NumericUpDown)item).Value  == 0)
                    {
                        return true;
                    }
                }

            }
            return false;

        }

        public static void Temizle(Panel pnl)
        {
            foreach (Control item in pnl.Controls)
            {
                if(item is TextBox)
                {
                    item.Text ="";
                }
                else if(item is CheckBox)
                {
                    ((CheckBox)item).Checked = false;
                }
            }
        }

        public static bool BosAlanVarMi(Panel pnl)
        {
            foreach (Control item in pnl.Controls)
            {
                if (item is TextBox)
                {
                    if (item.Text == "")
                        return true;
                }
                else if (item is ComboBox)
                {
                    if (((ComboBox)item).Text == "")
                    {
                        return true;
                    }
                }

                else if (item is NumericUpDown)
                {
                    if (((NumericUpDown)item).Value == 0)
                    {
                        return true;
                    }
                }

            }
            return false;

        }

    }

    //Comboboxta value ve text tutabilmek için kullanıyoruz. Örneğin: value: durakID, text: durakAdı
    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

}
