using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using DocumentFormat.OpenXml.Wordprocessing;
using b2xtanslator.Converters;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using VerticalAlignmentValues = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;

namespace oneWin.OfficeCreate
{
    public class createDocument
    {
        public string createWord(string url, Dictionary<string, string> replaceText = null, Dictionary<string, List<string>> tab = null, bool allTableReplace=false)
        {
            if (System.IO.File.Exists(url))
            {
                string s = Path.GetExtension(url);
                if (Path.GetExtension(url) == ".doc")
                {
                    url = new ConvertDoc().ConvertWord(url);
                }
                if (url != "")
                {
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(url, true))
                    {
                        //ищем и перезаписываем текст
                        if (replaceText != null)
                        {
                            string docText = null;
                            using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                            {
                                docText = sr.ReadToEnd();
                            }

                            if (docText != null)
                            {

                                foreach (var findText in replaceText)
                                {
                                    Regex regexText = new Regex(findText.Key);
                                    docText = regexText.Replace(docText, findText.Value == null ? "" : findText.Value);

                                    
                                }
                                docText = docText.Replace("[", "").Replace("]", "");
                                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                                    sw.Write(docText);
                            }
                        }
                        //заполняем таблицу
                        if (tab != null)
                        {
                            List<Table> tables = null;
                            if (wordDoc.MainDocumentPart.Document.Body.Elements<Table>().Any())
                            {
                                if (allTableReplace)
                                    tables = wordDoc.MainDocumentPart.Document.Body.Elements<Table>().ToList();
                                else
                                {
                                    tables = new();
                                    tables.Add(wordDoc.MainDocumentPart.Document.Body.Elements<Table>().First());
                                }
                            }
                            else
                            {
                                tables = new();
                                wordDoc.MainDocumentPart.Document.Body.Append(tables);
                            }

                            

                            foreach (var table in tables)
                            {
                                int celCount = 2;                                
                                if (table.Elements<TableGrid>().Any())
                                {
                                    if(table.Elements<TableGrid>().First().Elements<GridColumn>().Any())
                                    {
                                        celCount = table.Elements<TableGrid>().First().Elements<GridColumn>().Count();
                                    }
                                }
                                foreach (var d in tab)
                                {
                                    int indexCel = 0;
                                    TableRow tr1 = new TableRow();
                                    foreach (var t in d.Value)
                                    {
                                        if (indexCel >= celCount)
                                            continue;
                                        TableCell tc1 = new TableCell(new Paragraph(new Run(new Text(t))));
                                        tr1.Append(tc1);
                                        indexCel++;
                                    }
                                    table.AppendChild(tr1);
                                }

                            }
                        }
                    }
                }
            }
            return url;
        }


        /// <summary>
        /// Генерируем документ ворд с таблицами
        /// </summary>
        /// <param name="url"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public string createWord(string url, Dictionary<string, List<string>> tab)
        {
            if (tab != null && tab.Count > 0)
            {
                if (!System.IO.File.Exists(url))
                {
                    using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(url, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document(new Body());

                        
                    }
                }

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(url, true))
                {



                    bool newTable = true;
                    //заполняем таблицу
                    if (tab != null)
                    {
                        if(tab.ContainsKey("Landscape"))
                        {
                            SectionProperties sectProp = new SectionProperties();
                            PageSize pageSize = new PageSize() { Width = 16838U, Height = 11906U, Orient = PageOrientationValues.Landscape };
                            PageMargin pageMargin = new PageMargin() { Top = 720, Right = 720U, Bottom = 720, Left = 720U };
                            sectProp.Append(pageSize);
                            sectProp.Append(pageMargin);
                            wordDoc.MainDocumentPart.Document.Body.Append(sectProp);
                        }



                        Table table = null;
                        foreach (var row in tab.Where(x => x.Key != "width" && x.Key != "border" && x.Key != "Landscape"))
                        {
                            if (row.Key.Contains("paragraph"))
                            {
                                Paragraph para = paragraphCreate(row.Value.First());
                                wordDoc.MainDocumentPart.Document.Body.Append(para);
                                newTable = true;
                            }

                            if (!row.Key.Contains("paragraph") && newTable)
                            {
                                newTable = false;
                                table = new Table();
                                if (tab.Any(x => x.Key == "border"))
                                {

                                    TableProperties tblProp = new TableProperties(
                                        new TableBorders(
                                            new DocumentFormat.OpenXml.Wordprocessing.TopBorder()
                                            {
                                                Val = new EnumValue<BorderValues>(BorderValues.Single)
                                            },
                                            new DocumentFormat.OpenXml.Wordprocessing.BottomBorder()
                                            {
                                                Val = new EnumValue<BorderValues>(BorderValues.Single)
                                            },
                                            new DocumentFormat.OpenXml.Wordprocessing.LeftBorder()
                                            {
                                                Val = new EnumValue<BorderValues>(BorderValues.Single)
                                            },
                                            new DocumentFormat.OpenXml.Wordprocessing.RightBorder()
                                            {
                                                Val = new EnumValue<BorderValues>(BorderValues.Single)
                                            },
                                            new DocumentFormat.OpenXml.Wordprocessing.InsideHorizontalBorder()
                                            {
                                                Val = new EnumValue<BorderValues>(BorderValues.Single)
                                            },
                                            new DocumentFormat.OpenXml.Wordprocessing.InsideVerticalBorder()
                                            {
                                                Val = new EnumValue<BorderValues>(BorderValues.Single)
                                            }
                                        )
                                    );
                                    table.AppendChild<TableProperties>(tblProp);
                                }
                                wordDoc.MainDocumentPart.Document.Body.Append(table);
                            }

                            if (!row.Key.Contains("paragraph"))
                            {

                                TableRow tr = new TableRow(); // Create a row.
                                int cellIndex = 0;
                                foreach (var cel in row.Value)
                                {
                                    TableCell tc1 = new TableCell();// Create a cell.
                                    if (tab.Any(x => x.Key == "width"))
                                    {
                                        tc1.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = tab.First(x => x.Key == "width").Value[cellIndex] })); // Specify the width property of the table cell.
                                    }
                                    cellIndex++;                                    
                                    Paragraph para = paragraphCreate(cel);

                                    tc1.Append(para);

                                    tr.Append(tc1);// Append the table cell to the table row.
                                    int maxCell = tab.Where(x=>x.Value!=null).Max(x => x.Value.Count);
                                    if (row.Value.Count == 1 && maxCell >= 2)
                                    {
                                        tc1.Append(new TableCellProperties(new HorizontalMerge { Val = MergedCellValues.Restart }));

                                        for (int i = 2; i <= maxCell; i++)
                                        {
                                            tc1 = new TableCell();
                                            if (tab.Any(x => x.Key == "width"))
                                            {
                                                tc1.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = tab.First(x => x.Key == "width").Value[cellIndex] })); // Specify the width property of the table cell.
                                            }
                                            cellIndex++;
                                            tc1.Append(new Paragraph());// Specify the table cell content.
                                            tr.Append(tc1);
                                                                                       
                                            tc1.Append(new TableCellProperties(new HorizontalMerge { Val = MergedCellValues.Continue }));
                                           

                                        }
                                    }

                                }
                                table.Append(tr);// Append the table row to the table.
                            }
                        }
                    }
                }
            }

            return url;
        }

        public Paragraph paragraphCreate(string textParagraph)
        {
            Paragraph para = new Paragraph();
            string textCell = textParagraph;
            RunProperties runProperties1 = new RunProperties();
            runProperties1.Append(new RunFonts() { HighAnsi = "Times New Roman" });
            runProperties1.Append(new RunFonts() { Ascii = "Times New Roman" });
            var run = new Run();
            Dictionary<string, string> paramText = new();
            if (textParagraph.Contains("?"))
            {
                paramText = textParagraph.Split("?")[1].Split("&").ToDictionary(x => x.Split("=")[0], x => x.Split("=")[1]);
                textCell = textParagraph.Split("?")[0];
            }

            if (paramText.ContainsKey("size"))
            {
                runProperties1.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = paramText["size"] });
            }
            else
            {
                runProperties1.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "28" });
            }
            if (paramText.ContainsKey("align"))
                switch (paramText["align"])
                {
                    case "center": para.Append(new ParagraphProperties(new Justification() { Val = JustificationValues.Center })); break;
                    case "right": para.Append(new ParagraphProperties(new Justification() { Val = JustificationValues.Right })); break;
                    case "left": para.Append(new ParagraphProperties(new Justification() { Val = JustificationValues.Left })); break;
                }
            else
            {
                para.Append(new ParagraphProperties(new Justification() { Val = JustificationValues.Both }));
                para.Append(new ParagraphProperties(new Indentation() { FirstLine = new StringValue("720") }));
            }
            run.Append(runProperties1);
            if (textCell.Contains("\n"))
            {
                foreach (var breaks in textCell.Split("\n"))
                {
                    if (!string.IsNullOrEmpty(breaks)) run.Append(new Text(breaks));
                    run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Break());
                }
            }
            else
                if (!string.IsNullOrEmpty(textCell)) run.Append(new Text(textCell));
            para.Append(run);

            return para;
        }


        /// <summary>
        /// Создание или редактирование Excel
        /// </summary>
        /// <param name="url">адрес файла</param>
        /// <param name="tab">таблица значений с параметрами</param>
        /// <param name="enableStyle">применить программный стиль</param>
        /// <param name="sheetName">кол-во страниц Excel</param>
        /// <returns></returns>
        public string createExcel(string url, Dictionary<string, List<string>> tab, bool enableStyle = false, string sheetName = "")
        {
            if (!System.IO.File.Exists(url))
            {
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(url, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                    Sheet sheet = new Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.
                        GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = sheetName != "" ? sheetName : "Report"
                    };
                    sheets.Append(sheet);

                    workbookpart.Workbook.Save();
                }
            }

            using (SpreadsheetDocument excel = SpreadsheetDocument.Open(url, true))
            {
                if (tab != null)
                {
                    WorkbookPart wbp = excel.WorkbookPart;
                    WorksheetPart wsp = null;
                    Sheet s = null;
                    if (!wbp.Workbook.Sheets.Elements<Sheet>().Any())
                    {
                        excel.Close();
                        return "";
                    }
                    else
                    {
                        if (sheetName == "")
                            s = (Sheet)wbp.Workbook.Sheets.Elements<Sheet>().First();
                        else
                        {
                            if (wbp.Workbook.Sheets.Elements<Sheet>().Any(x => x.Name == sheetName))
                            {
                                s = (Sheet)wbp.Workbook.Sheets.Elements<Sheet>().First(x => x.Name == sheetName);
                            }
                            else
                            {
                                wsp = wbp.AddNewPart<WorksheetPart>();
                                wsp.Worksheet = new Worksheet(new SheetData());

                                Sheets sheets = wbp.Workbook.GetFirstChild<Sheets>();
                                string relationshipId = wbp.GetIdOfPart(wsp);

                                uint sheetId = 1;
                                if (sheets.Elements<Sheet>().Count() > 0)
                                {
                                    sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                                }
                                s = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
                                sheets.Append(s);
                            }
                        }
                    }
                    wsp = (WorksheetPart)excel.WorkbookPart.GetPartById(s.Id.Value);
                    SheetData sheetData = (SheetData)wsp.Worksheet.GetFirstChild<SheetData>();
                    //string[] nameCel = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T" };

                    if (enableStyle)
                    {
                        if (!wbp.GetPartsOfType<WorkbookStylesPart>().Any())
                        {
                            WorkbookStylesPart stylePart = wbp.AddNewPart<WorkbookStylesPart>();
                            stylePart.Stylesheet = GenerateStylesheet();
                            stylePart.Stylesheet.Save();
                        }
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Columns columns = null;
                    if (!wsp.Worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.Columns>().Any())
                    {
                        columns = new DocumentFormat.OpenXml.Spreadsheet.Columns();
                        wsp.Worksheet.InsertBefore(columns, sheetData);
                    }
                    else
                    {
                        columns = wsp.Worksheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Columns>();
                    }
                    for (UInt32 iCol = 1; iCol <= tab.Max(x => x.Value.Count); iCol++)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Column col = new DocumentFormat.OpenXml.Spreadsheet.Column();
                        col.Min = iCol;
                        col.Max = iCol;
                        if (tab.ContainsKey("width"))
                        {
                            if (tab.First(x => x.Key == "width").Value.Count >= iCol)
                                col.Width = Convert.ToInt32(tab.First(x => x.Key == "width").Value.ElementAt((int)(iCol - 1)));
                            else
                                col.Width = 10;
                        }
                        else
                            col.Width = 10;
                        col.CustomWidth = true;
                        columns.Append(col);
                    }
                    UInt32 rowIndex = 1;
                    foreach (var d in tab.Where(x => x.Key != "width"))
                    {
                        if (d.Key.Contains("emptyRow"))
                        {
                            rowIndex++;
                            continue;
                        }
                        UInt32 styleIndex = (UInt32)(d.Key.Contains("head") ? 2 : d.Key.Contains("NoBorder") ? 3 : 1);



                        Row row = null;
                        if (sheetData.Elements<Row>().Any(ri => ri.RowIndex == rowIndex))
                            row = (Row)sheetData.Elements<Row>().First(ri => ri.RowIndex == rowIndex);
                        else
                        {
                            row = new Row() { RowIndex = rowIndex };
                            sheetData.Append(row);
                        }

                        int i = 1;
                        foreach (var t in d.Value)
                        {
                            Cell newCell = null;
                            if (row.Elements<Cell>().Any(c => c.CellReference == (ColumnIndex(i) + rowIndex)))
                                newCell = (Cell)row.Elements<Cell>().First(c => c.CellReference == (ColumnIndex(i) + rowIndex));
                            else
                            {
                                newCell = new Cell() { CellReference = (ColumnIndex(i) + rowIndex) };
                                row.Append(newCell);
                            }
                            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
                            newCell.CellValue = new CellValue(Regex.Replace(t, r, "", RegexOptions.Compiled));
                            int ints;
                            if (Int32.TryParse(t, out ints))
                            {
                                newCell.DataType = new EnumValue<CellValues>(CellValues.Number);
                            }
                            else
                                newCell.DataType = new EnumValue<CellValues>(CellValues.String);

                            if (enableStyle)
                                newCell.StyleIndex = styleIndex;
                            if (d.Key.Contains("startTwo") && i == 1)
                            {
                                mergeCellsAdd((ColumnIndex(1) + rowIndex) + ":" + ColumnIndex(2) + rowIndex, wsp.Worksheet);
                                var cols = wsp.Worksheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Columns>().Elements<DocumentFormat.OpenXml.Spreadsheet.Column>();
                                setRowHeight(row, (double)cols.ElementAt(0).Width + (double)cols.ElementAt(1).Width, d.Value[0]);
                            }
                            if (d.Value.Count == 1 && tab.Max(x => x.Value.Count) >= 2)
                            {
                                setRowHeight(row, wsp.Worksheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Columns>().Elements<DocumentFormat.OpenXml.Spreadsheet.Column>().Sum(x => x.Width), t);

                                for (int m = 1; m < tab.Max(x => x.Value.Count); m++)
                                {
                                    if (row.Elements<Cell>().Any(c => c.CellReference == (ColumnIndex(m) + rowIndex)))
                                        newCell = (Cell)row.Elements<Cell>().First(c => c.CellReference == (ColumnIndex(m) + rowIndex));
                                    else
                                    {
                                        newCell = new Cell() { CellReference = (ColumnIndex(m) + rowIndex) };
                                        row.Append(newCell);
                                    }
                                    if (enableStyle)
                                        newCell.StyleIndex = styleIndex;
                                }

                                mergeCellsAdd((ColumnIndex(1) + rowIndex) + ":" + ColumnIndex(tab.Max(x => x.Value.Count)) + rowIndex, wsp.Worksheet);

                            }
                            i++;

                        }



                        rowIndex++;
                    }
                    excel.WorkbookPart.Workbook.Save();
                }
            }
            return url;
        }


        static string ColumnIndex(int colIndex)
        {
            int div = colIndex;
            string colLetter = String.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }

        private void setRowHeight(Row r, double w, string val)
        {
            double heightRow = Math.Ceiling(val.Length / w);
            if (heightRow > 1)
            {
                r.Height = 15.75 * heightRow;
                r.CustomHeight = true;
                r.CustomFormat = true;
            }
        }

        private void mergeCellsAdd(string refer, Worksheet w)
        {
            MergeCells mergeCells;

            if (w.Elements<MergeCells>().Count() > 0)
                mergeCells = w.Elements<MergeCells>().First();
            else
            {
                mergeCells = new MergeCells();
                if (w.Elements<CustomSheetView>().Count() > 0)
                    w.InsertAfter(mergeCells, w.Elements<CustomSheetView>().First());
                else
                    w.InsertAfter(mergeCells, w.Elements<SheetData>().First());
            }

            MergeCell mergeCell = new MergeCell() { Reference = new StringValue(refer) };
            mergeCells.Append(mergeCell);
        }

        private Stylesheet GenerateStylesheet()
        {
            Stylesheet styleSheet = null;

            DocumentFormat.OpenXml.Spreadsheet.Fonts fonts = new DocumentFormat.OpenXml.Spreadsheet.Fonts(
                new DocumentFormat.OpenXml.Spreadsheet.Font( // Index 0 - header
                    new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 12 },
                    new DocumentFormat.OpenXml.Spreadsheet.Bold() { Val = false },
                    new DocumentFormat.OpenXml.Spreadsheet.FontName() { Val = "Times New Roman" }),
                 new DocumentFormat.OpenXml.Spreadsheet.Font( // Index 1 - header
                    new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 12 },
                    new DocumentFormat.OpenXml.Spreadsheet.Bold() { Val = true },
                    new DocumentFormat.OpenXml.Spreadsheet.FontName() { Val = "Times New Roman" })
                );

            Fills fills = new Fills(
                    new DocumentFormat.OpenXml.Spreadsheet.Fill(new DocumentFormat.OpenXml.Spreadsheet.PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                    new DocumentFormat.OpenXml.Spreadsheet.Fill(new DocumentFormat.OpenXml.Spreadsheet.PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - default
                    new DocumentFormat.OpenXml.Spreadsheet.Fill(new DocumentFormat.OpenXml.Spreadsheet.PatternFill(new DocumentFormat.OpenXml.Spreadsheet.ForegroundColor { Rgb = new HexBinaryValue() { Value = "66666666" } }) { PatternType = PatternValues.Solid }) // Index 2 - header
                );

            Borders borders = new Borders(
                    new DocumentFormat.OpenXml.Spreadsheet.Border(),
                    new DocumentFormat.OpenXml.Spreadsheet.Border( // index 2 black border
                        new DocumentFormat.OpenXml.Spreadsheet.LeftBorder(new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DocumentFormat.OpenXml.Spreadsheet.RightBorder(new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DocumentFormat.OpenXml.Spreadsheet.TopBorder(new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DocumentFormat.OpenXml.Spreadsheet.BottomBorder(new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                );

            CellFormats cellFormats = new CellFormats(
                    new CellFormat(new Alignment() { WrapText = true, ShrinkToFit = true, Vertical = VerticalAlignmentValues.Center }) { FontId = 0, BorderId = 1, ApplyBorder = true }, // 0
                    new CellFormat(new Alignment() { WrapText = true, ShrinkToFit = true, Vertical = VerticalAlignmentValues.Center }) { FontId = 0, BorderId = 1, ApplyBorder = true },
                    new CellFormat(new Alignment() { WrapText = true, ShrinkToFit = true, Vertical = VerticalAlignmentValues.Center }) { FontId = 1, BorderId = 1, ApplyBorder = true },
                    new CellFormat(new Alignment() { WrapText = true, ShrinkToFit = true, Vertical = VerticalAlignmentValues.Center, Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center }) { FontId = 1, ApplyBorder = true } // split cells
                );
            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);
            return styleSheet;
        }
    }
}
