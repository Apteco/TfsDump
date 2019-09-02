using System.Threading.Tasks;

namespace Apteco.TfsDump.Core.Sinks
{
  public interface ISink
  {
    Task InitialiseSink(params string[] fieldNames);
    Task Write(params string[] data);
  }
}
