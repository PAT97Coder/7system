using DataAccessLayer;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    internal static class Exam312QuestionExcel
    {
        internal static List<dt312_QuestionImportItem> Load(string excelPath)
        {
            using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
            {
                var data = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
                });
                if (data.Tables.Count == 0) throw new ArgumentException("試算表沒有可匯入的工作表。");
                return Parse(data.Tables[0], Path.Combine(Path.GetDirectoryName(excelPath), "Images"));
            }
        }

        internal static List<dt312_QuestionImportItem> Parse(DataTable table, string imagesFolder)
        {
            if (table.Columns.Count < 7) throw new ArgumentException("試算表至少需要原有的 A 至 G 欄。");
            var items = new List<dt312_QuestionImportItem>();
            var byNumber = new Dictionary<int, dt312_QuestionImportItem>();
            dt312_QuestionImportItem current = null;
            for (int index = 0; index < table.Rows.Count; index++)
            {
                var row = table.Rows[index];
                if (row.ItemArray.All(v => string.IsNullOrWhiteSpace(Convert.ToString(v)))) continue;
                try
                {
                    string numberText = Cell(row, 0);
                    int number;
                    if (numberText.Length == 0 && current != null) number = current.ExcelNumber;
                    else if (!int.TryParse(numberText, out number) || number <= 0)
                        throw new ArgumentException("A 欄必須填寫正整數題號。");
                    byNumber.TryGetValue(number, out var item);
                    string type = Cell(row, 7);
                    if (type.Length != 0 && type != "選擇題" && type != "申論題")
                        throw new ArgumentException("H 欄題型只能填寫「選擇題」或「申論題」。");
                    bool essay = type == "申論題" || (type.Length == 0 && item != null && item.Question.IsEssay);
                    string question = Cell(row, 1), image = Cell(row, 2), reference = Cell(row, 8);
                    if (item == null)
                    {
                        if (question.Length == 0 || question.Length > 2048 || reference.Length > 16000)
                            throw new ArgumentException("題目為必填且最多 2048 字；參考答案最多 16000 字。");
                        item = new dt312_QuestionImportItem
                        {
                            ExcelNumber = number,
                            Question = new dt312_Questions
                            {
                                DisplayName = question, ImageName = ImagePath(image, imagesFolder),
                                IsEssay = essay, ReferenceAnswer = essay ? reference : null
                            }
                        };
                        items.Add(item); byNumber.Add(number, item);
                    }
                    else
                    {
                        if (item.Question.IsEssay || item.Question.IsEssay != essay)
                            throw new ArgumentException("申論題每題只能填寫一列，且同一題號不可混用題型。");
                        if ((question.Length > 0 && question != item.Question.DisplayName)
                            || (image.Length > 0 && ImagePath(image, imagesFolder) != item.Question.ImageName))
                            throw new ArgumentException("同一題號的題目內容與圖片必須一致。");
                    }
                    current = item;
                    if (essay)
                    {
                        if (Enumerable.Range(3, 4).Any(column => Cell(row, column).Length > 0))
                            throw new ArgumentException("申論題的 D 至 G 欄請留空。");
                        continue;
                    }
                    if (reference.Length > 0) throw new ArgumentException("I 欄參考答案僅供申論題使用；請確認 H 欄題型。");
                    string answerText = Cell(row, 4), answerImage = ImagePath(Cell(row, 5), imagesFolder), correct = Cell(row, 6);
                    if ((answerText.Length == 0 && answerImage.Length == 0) || answerText.Length > 2048)
                        throw new ArgumentException("選項必須有文字或圖片，文字最多 2048 字。");
                    if (correct != "" && correct != "0" && correct != "1") throw new ArgumentException("G 欄正確答案只能填寫 1 或 0。");
                    item.Answers.Add(new dt312_Answers
                    {
                        Id = item.Answers.Count + 1, QuesId = number, DisplayName = answerText,
                        ImageName = answerImage, TrueAns = correct == "1"
                    });
                }
                catch (ArgumentException ex) { throw new ArgumentException("第 " + (index + 2) + " 列：" + ex.Message); }
            }
            if (items.Count == 0) throw new ArgumentException("工作表沒有可匯入的題目。");
            foreach (var item in items.Where(i => !i.Question.IsEssay))
            {
                int count = item.Answers.Count(a => a.TrueAns);
                if (count == 0) throw new ArgumentException("第 " + item.ExcelNumber + " 題未設定正確答案。");
                item.Question.IsmultiAns = count > 1;
            }
            return items;
        }

        private static string Cell(DataRow row, int column)
        {
            if (column >= row.Table.Columns.Count) return "";
            string value = Convert.ToString(row[column], CultureInfo.InvariantCulture) ?? "";
            return Regex.Replace(value, @"[\t\n\r\s]+", m => m.Value.Contains("\n") ? "\r\n" : " ").Trim();
        }

        private static string ImagePath(string name, string folder)
        {
            if (name.Length == 0) return "";
            if (Path.GetFileName(name) != name || name.Contains("/") || name.Contains("\\") || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException("圖片欄請只填寫 Images 資料夾內的檔名。");
            string path = Path.GetFullPath(Path.Combine(folder, name));
            if (!File.Exists(path)) throw new ArgumentException("找不到圖片：" + name);
            try { using (var image = Image.FromFile(path)) { } }
            catch { throw new ArgumentException("無法讀取圖片：" + name); }
            return path;
        }
    }
}
