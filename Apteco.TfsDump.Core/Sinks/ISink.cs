using System.Threading.Tasks;

namespace Apteco.TfsDump.Core.Sinks
{
  public interface ISink
  {
    Task InitialiseSink(string[] fieldNames, string[] keyFieldNames);
    Task Write(string[] data);
  }
}
