﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Contracts
{
    public interface IPlugin
    {
        string Name { get; }
        void Do(RichTextBox richTextBox);
    }
}
