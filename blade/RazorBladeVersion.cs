using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;

public class RazorBladeVersion: Custom.Hybrid.Code14
{

  public Version GetRazorBladeVersion() {
    var div = ToSic.Razor.Blade.Tag.Div();
    var version = Assembly.GetAssembly(div.GetType()).GetName().Version;
    return version;
  }

  public float VersionDouble() {
    var verInfo = GetRazorBladeVersion();
    if(verInfo == null) return 0;

    var major = verInfo.Major;
    var minor = verInfo.Minor;
    return Kit.Convert.ToFloat(major + "." + minor.ToString("D2"));
  }

  public string VersionInfo(double version, double expected) {
    var cls = expected <= version ? "secondary" : "danger";
    return "<span class='badge badge-" + cls + "'>v" + expected.ToString("0.00") + "</span>";
  }
}
