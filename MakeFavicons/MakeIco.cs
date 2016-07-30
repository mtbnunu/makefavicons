using System;
using System.Collections.Generic;
using System.Drawing.IconLib;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeFavicons
{
    internal static class MakeIco
    {
        // Thanks to 
        // https://github.com/Jamedjo/IconLib/releases
        // Apprently, MUST be a 256x256 @32bpp

        public static void Make(string pngPath, string icoPath)
        {
            MultiIcon mIcon = new MultiIcon();
            mIcon.Add("Untitled").CreateFrom(pngPath, IconOutputFormat.FromWin95);
            mIcon.SelectedIndex = 0;
            mIcon.Save(icoPath, MultiIconFormat.ICO);
        }
    }
}
