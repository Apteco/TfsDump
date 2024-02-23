using System.Threading.Tasks;

namespace Apteco.TfsDump.Core.Sinks
{
  public static class SinkExtensions
  {
    public static async Task InitialiseSink(this ISink sink, string[] fieldNames, string keyFieldNames)
    {
      await sink.InitialiseSink(fieldNames, new string[] { keyFieldNames });
    }
  }
}
