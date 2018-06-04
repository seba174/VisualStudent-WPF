using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public class ErrorListItem
    {
        public string Error { get; private set; }
        public string Description { get; private set; }
        public string WhereOccured { get; private set; }

        public ErrorListItem(string e)
        {
            try
            {
                int iW = e.IndexOf(':');
                WhereOccured = e.Substring(0, iW).Trim(' ');
                int iE = e.IndexOf(':', iW + 1);
                Error = e.Substring(iW + 2, iE - iW - 2);
                Description = e.Substring(iE + 2);
            }
            catch (Exception)
            {
                SetDefaultValues();
            }
        }

        private void SetDefaultValues()
        {
            Error = string.Empty;
            Description = string.Empty;
            WhereOccured = string.Empty;
        }
    }
}
