using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename.Core
{
    public interface IConfigRuleWindow
    {
        public string GetData();

        public bool? ShowDialog();
    }
}
