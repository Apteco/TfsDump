namespace Apteco.TfsDump.Core
{
  public static class StringExtensions
  {
    public static string SanitiseForTabDelimitedString(this string s)
    {
      return s?.Replace("\t", " ").Replace("\r\n", " ").Replace("\r", "").Replace("\n", " ");
    }
  }
}
