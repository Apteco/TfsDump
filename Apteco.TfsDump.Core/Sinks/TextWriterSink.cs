using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Apteco.TfsDump.Core.Sinks
{
  public class TextWriterSink : ISink
  {
    private TextWriter writer;
    private bool initialised;
    private int numberOfFields;

    public TextWriterSink(TextWriter writer)
    {
      this.writer = writer;
    }

    public async Task InitialiseSink(string[] fieldNames, string keyFieldName)
    {
      if (fieldNames == null)
        throw new ArgumentNullException(nameof(fieldNames));

      if (initialised)
        throw new Exception($"This sink has already been initialised");

      numberOfFields = fieldNames.Length;
      await writer.WriteLineAsync(string.Join("\t", fieldNames.Select(s => s.SanitiseForTabDelimitedString())));
      initialised = true;
    }

    public async Task Write(string[] data)
    {
      if (data == null)
        throw new ArgumentNullException(nameof(data));

      if (data.Length != numberOfFields)
        throw new Exception($"Can't write data with {data.Length} fields when the sink was initialised with {numberOfFields} fields");

      await writer.WriteLineAsync(string.Join("\t", data.Select(s => s?.SanitiseForTabDelimitedString())));
    }
  }
}
