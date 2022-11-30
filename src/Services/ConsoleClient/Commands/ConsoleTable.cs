using System.Text;

namespace ConsoleClient.Commands;

public class ConsoleTable
{
    public ConsoleTable(params string[] headers)
    {
        Headers = headers;
        Rows    = new List<string[]>();
    }

    private List<string[]> Rows { get; }

    private string[] Headers { get; }

    public void AddRow(params string[] row)
    {
        Rows.Add(row);
    }

    public override string ToString()
    {
        var sb                                                   = new StringBuilder();
        var columnWidths                                         = new int[Headers.Length];
        for (var i = 0; i < Headers.Length; i++) columnWidths[i] = Headers[i].Length;

        foreach (var row in Rows)
            for (var i = 0; i < row.Length; i++)
                if (row[i].Length > columnWidths[i])
                    columnWidths[i] = row[i].Length;

        var header = string.Join(" | ", Headers.Select((t, i) => t.PadRight(columnWidths[i])));
        sb.AppendLine(header);
        sb.AppendLine(string.Join("", Enumerable.Repeat("-", header.Length)));

        foreach (var row in Rows)
        {
            var line = string.Join(" | ", row.Select((t, i) => t.PadRight(columnWidths[i])));
            sb.AppendLine(line);
        }

        return sb.ToString();
    }
}