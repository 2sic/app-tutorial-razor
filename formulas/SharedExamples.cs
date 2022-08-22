/// <summary>
/// Serves as a shared definition of samples
/// Which we use in multiple pages
/// So we don't duplicate code.
///
/// The samples must return a dynamic/anonymous object,
/// otherwise the params won't have the desired effect.
/// </summary>
public class SharedExamples : Custom.Hybrid.Code14 {
  public string SayHello() {
    return "Hello from shared functions!";
  }

  // Used in 110 and 200
  public dynamic ValueFromWebApi() {
    return new {
      Title = "Get value for a field from a WebAPI",
      Instructions = "This formula is very advanced, and will initialize a fields value (if empty) from a WebAPI call. ",
      ContentType = "FormulasWebApiText",
      FieldName = "Title"
    };
  }

  // Used in 114 and 200
  public dynamic DropdownFromWebApi() {
    return new {
      Title = "Get dropdown options from WebAPI",
      Instructions = "This is an advanced formula which will call a WebApi to get the possible values in a drop down. ",
      ContentType = "FormulasDropdownWebApi",
      FieldName = "Dropdown"
    };
  }  
}
