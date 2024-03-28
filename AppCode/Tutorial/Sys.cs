using AppCode.Output;
using AppCode.TutorialSystem.Source;

namespace AppCode.Tutorial
{

  public class Sys: Custom.Hybrid.CodeTyped
  {
    public SourceCode SourceCode => _sourceCode ??= GetService<SourceCode>();
    private SourceCode _sourceCode;

    public ToolbarHelpers ToolbarHelpers => _tlbHelpers ??= GetService<ToolbarHelpers>();
    private ToolbarHelpers _tlbHelpers;
  }
}