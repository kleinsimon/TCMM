using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TC_Macro_Manager
{
    public enum MacroPosition
    {
        DB,
        Poly,
        Post,
    }

    interface MacroModule
    {
        MacroPosition Position { get; set; }

        string getString();
    }
}
