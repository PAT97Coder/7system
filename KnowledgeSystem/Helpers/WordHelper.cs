using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    public class WordHelper
    {
        private static WordHelper instance;

        public static WordHelper Instance
        {
            get { if (instance == null) instance = new WordHelper(); return instance; }
            private set { instance = value; }
        }

        private WordHelper()
        {
        }

        public void MergeFuelLogCells(string fileName, int entryCount)
        {
            if (entryCount < 2) return;
            using (var document = WordprocessingDocument.Open(fileName, true))
            {
                var table = document.MainDocumentPart.Document.Body.Descendants<Table>()
                    .FirstOrDefault(item => item.Elements<TableRow>().Any(row =>
                        row.Elements<TableCell>().Count() >= 10 && row.InnerText.Contains("發票")));
                if (table == null) throw new InvalidOperationException("找不到加油記錄表的資料表格。");
                var rows = table.Elements<TableRow>().ToList();
                int headerIndex = rows.FindIndex(row => row.Elements<TableCell>().Count() >= 10 && row.InnerText.Contains("發票"));
                var dataRows = rows.Skip(headerIndex + 1).Take(entryCount).ToList();
                if (dataRows.Count != entryCount) throw new InvalidOperationException("加油記錄表的資料列數不符。");
                int columnCount = rows[headerIndex].Elements<TableCell>().Count();
                foreach (int column in new[] { columnCount - 2, columnCount - 1 })
                {
                    int start = 0;
                    while (start < dataRows.Count)
                    {
                        string value = dataRows[start].Elements<TableCell>().ElementAt(column).InnerText.Trim();
                        int end = start + 1;
                        while (end < dataRows.Count && dataRows[end].Elements<TableCell>().ElementAt(column).InnerText.Trim() == value)
                            end++;
                        if (end - start > 1)
                            for (int index = start; index < end; index++)
                            {
                                var cell = dataRows[index].Elements<TableCell>().ElementAt(column);
                                if (cell.TableCellProperties == null) cell.TableCellProperties = new TableCellProperties();
                                cell.TableCellProperties.VerticalMerge = new VerticalMerge
                                {
                                    Val = index == start ? MergedCellValues.Restart : MergedCellValues.Continue
                                };
                                cell.TableCellProperties.TableCellVerticalAlignment = new TableCellVerticalAlignment
                                {
                                    Val = TableVerticalAlignmentValues.Center
                                };
                                if (index != start)
                                {
                                    var paragraphProperties = cell.Elements<Paragraph>().FirstOrDefault()?.ParagraphProperties?.CloneNode(true);
                                    foreach (var child in cell.ChildElements.Where(child => !(child is TableCellProperties)).ToList()) child.Remove();
                                    var paragraph = new Paragraph();
                                    if (paragraphProperties != null) paragraph.Append(paragraphProperties);
                                    cell.Append(paragraph);
                                }
                            }
                        start = end;
                    }
                }
                document.MainDocumentPart.Document.Save();
            }
        }

        public DataTable ReadTableFromMSWord(string fileName)
        {
            using (var doc = WordprocessingDocument.Open(fileName, false))
            {

                DataTable dt = new DataTable();
                int rowCount = 0;

                Table table = doc.MainDocumentPart.Document.Body.Elements<Table>().First();
                IEnumerable<TableRow> rows = table.Elements<TableRow>();
                foreach (TableRow row in rows)
                {
                    if (rowCount == 0)
                    {
                        foreach (TableCell cell in row.Descendants<TableCell>())
                        {
                            dt.Columns.Add(cell.InnerText);
                        }
                        rowCount += 1;
                    }
                    else
                    {
                        dt.Rows.Add();
                        int i = 0;
                        foreach (TableCell cell in row.Descendants<TableCell>())
                        {
                            if (dt.Columns.Count <= i)
                            {
                                dt.Columns.Add($"Col{i}");
                            }

                            dt.Rows[dt.Rows.Count - 1][i] = cell.InnerText;
                            i++;
                        }
                    }
                }

                return dt;
            }
        }
    }
}
