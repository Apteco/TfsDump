using System.Threading.Tasks;

namespace Apteco.TfsDump.Core.Sinks
{
  public interface ISink
  {
    Task InitialiseSink(string[] fieldNames, string keyFieldName);
    Task Write(string[] data);
  }
}
