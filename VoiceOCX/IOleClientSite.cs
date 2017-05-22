using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoiceOCX
{
    [ComImport,
    Guid("00000118-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleClientSite
    {
        void SaveObject();
        void GetMoniker(uint dwAssign, uint dwWhichMoniker, object ppmk);
        void GetContainer(out IOleContainer ppContainer);
        void ShowObject();
        void OnShowWindow(bool fShow);
        void RequestNewObjectLayout();
    }

    [ComImport,
    Guid("0000011B-0000-0000-C000-000000000046"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleContainer
    {
        void EnumObjects([In, MarshalAs(UnmanagedType.U4)] int grfFlags,
         [Out, MarshalAs(UnmanagedType.LPArray)] object[] ppenum);
        void ParseDisplayName([In, MarshalAs(UnmanagedType.Interface)] object pbc,
         [In, MarshalAs(UnmanagedType.BStr)] string pszDisplayName,
         [Out, MarshalAs(UnmanagedType.LPArray)] int[] pchEaten,
         [Out, MarshalAs(UnmanagedType.LPArray)] object[] ppmkOut);
        void LockContainer([In, MarshalAs(UnmanagedType.I4)] int fLock);
    }
}
