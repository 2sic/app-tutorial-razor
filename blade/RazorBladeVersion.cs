using System;
using System.Diagnostics;
using System.IO;

public class RazorBladeVersion: Custom.Hybrid.Code12
{

  public FileVersionInfo GetRazorBladeVersion() {
    var pathToRazorDll = "~/bin/ToSic.Razor.dll";
    pathToRazorDll = System.Web.HttpContext.Current.Server.MapPath(pathToRazorDll);
    var exists = File.Exists(pathToRazorDll);
    return exists
      ? FileVersionInfo.GetVersionInfo(pathToRazorDll)
      : null;
  }

  public double VersionDouble() {
    var verInfo = GetRazorBladeVersion();
    if(verInfo == null) return 0;

    var major = verInfo.FileMajorPart;
    var minor = verInfo.FileMinorPart;
    return Convert.ToDouble(major + "." + minor.ToString("D2"));
  }

  public string VersionInfo(double version, double expected) {
    var cls = expected <= version ? "secondary" : "danger";
    return "<span class='badge badge-" + cls + "'>v" + expected.ToString("0.00") + "</span>";
  }
}
