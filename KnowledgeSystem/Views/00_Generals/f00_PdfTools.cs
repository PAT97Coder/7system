using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.Pdf;
using DevExpress.XtraEditors;
using DevExpress.XtraPdfViewer;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSpreadsheet.Model;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_PdfTools : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public string OutFileName { get; private set; }
        public string Describe { get; private set; }

        public f00_PdfTools(string FilePath, string OutDic, bool FullSign = false)
        {
            InitializeComponent();
            InitializeIcon();
            filePath = FilePath;
            outDic = OutDic;
            fullSign = FullSign;

            pdfViewer.MouseDown += PdfViewer_MouseDown;
            pdfViewer.MouseUp += PdfViewer_MouseUp;
            pdfViewer.MouseMove += PdfViewer_MouseMove;
            pdfViewer.MouseDoubleClick += PdfViewer_MouseDoubleClick;
            pdfViewer.Paint += PdfViewer_Paint;

            pdfViewer.PopupMenuShowing += PdfViewer_PopupMenuShowing;
            pdfViewer.KeyDown += PdfViewer_KeyDown;
            ribbonControl1.KeyDown += PdfViewer_KeyDown;

            pdfViewer.NavigationPanePageVisibility = PdfNavigationPanePageVisibility.None;
            KeyPreview = true;
        }

        #region parameters

        private void InitializeIcon()
        {
            btnStamp.ImageOptions.SvgImage = TPSvgimages.Stamp;
            btnSignDefault.ImageOptions.SvgImage = TPSvgimages.Approval;
        }

        class GraphicsCoordinates
        {
            public GraphicsCoordinates(int pageIndex, PdfPoint point1, PdfPoint point2, Image imageSign, string descrip, SignInfo signType)
            {
                PageIndex = pageIndex;
                Point1 = point1;
                Point2 = point2;
                ImageSign = imageSign;
                Descrip = descrip;
                SignType = signType;
            }

            public int PageIndex { get; }
            public PdfPoint Point1 { get; set; }
            public PdfPoint Point2 { get; set; }
            public bool IsEmpty => Point1 == Point2;
            public Image ImageSign { get; }
            public string Descrip { get; set; }
            public SignInfo SignType { get; }
        }

        enum SignEditMode
        {
            None,
            Create,
            Move,
            Resize
        }

        SignInfo signInfo = SignInfo.Sign;
        List<dm_Sign> dmSigns;
        dm_Sign signSelect = new dm_Sign();

        List<GraphicsCoordinates> signs = new List<GraphicsCoordinates>();
        GraphicsCoordinates currentSign;
        GraphicsCoordinates selectedSign;
        SignEditMode signEditMode = SignEditMode.None;
        PdfPoint dragStartPoint;
        PdfPoint dragStartPoint1;
        PdfPoint dragStartPoint2;
        Rectangle dragStartClientRectangle;
        GraphicsCoordinates contextMenuSign;
        DXPopupMenu popupMenuSign;
        DXMenuItem itemEditSignDate;

        bool fullSign = false;
        string filePath = "";
        string outDic = "";

        Image imageSign = null;
        string descrip = "";
        Font font = new Font("Times New Roman", 12, FontStyle.Regular);
        SizeF sizeFont = new SizeF();

        // This variable indicates whether the Drawing button is activated
        bool ActivateDrawing = false;

        #endregion

        #region methods

        private void InitializeMenuItems()
        {
            popupMenuSign = new DXPopupMenu();
            itemEditSignDate = CreateMenuItem("修改日期", ItemEditSignDate_Click, TPSvgimages.Edit);
            popupMenuSign.Items.Add(itemEditSignDate);
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = System.Drawing.Color.Blue;
        }

        private void ItemEditSignDate_Click(object sender, EventArgs e)
        {
            if (contextMenuSign == null)
                return;

            selectedSign = contextMenuSign;
            EditSelectedSignDescription();
        }

        private void DefaultSign()
        {
            if (!dmSigns.Any(r => r.ImgType == 0)) return;

            signSelect = dmSigns.FirstOrDefault(r => r.ImgType == 0);
            string signPath = Path.Combine(TPConfigs.FolderSign, signSelect.ImgName);
            imageSign = File.Exists(signPath) ? new Bitmap(signPath) : TPSvgimages.NoImage;

            descrip = DateTime.Now.ToString("yyyy.MM.dd");
        }

        Rectangle GetClientRectangle(GraphicsCoordinates rect)
        {
            PointF start = pdfViewer.GetClientPoint(new PdfDocumentPosition(rect.PageIndex + 1, rect.Point1));
            PointF end = pdfViewer.GetClientPoint(new PdfDocumentPosition(rect.PageIndex + 1, rect.Point2));

            return Rectangle.FromLTRB(
                (int)Math.Min(start.X, end.X),
                (int)Math.Min(start.Y, end.Y),
                (int)Math.Max(start.X, end.X),
                (int)Math.Max(start.Y, end.Y));
        }

        Rectangle GetResizeHandleRectangle(Rectangle rect)
        {
            const int handleSize = 8;
            return new Rectangle(rect.Right - handleSize / 2, rect.Bottom - handleSize / 2, handleSize, handleSize);
        }

        bool TryGetPdfPointFromClientPoint(int pageIndex, PointF clientPoint, out PdfPoint pdfPoint)
        {
            var documentPosition = pdfViewer.GetDocumentPosition(Point.Round(clientPoint), true);
            pdfPoint = documentPosition.Point;
            return documentPosition.PageNumber - 1 == pageIndex;
        }

        double GetSignAspectRatio(Image image, string text)
        {
            if (image == null || image.Height <= 0)
                return 1;

            double textHeight = string.IsNullOrWhiteSpace(text) ? 0 : font.Height * 1.2;
            return image.Width / (image.Height + textHeight);
        }

        bool TryGetPdfUnitsPerClientPixel(int pageIndex, Point center, int deltaX, int deltaY, out double unitsPerPixel)
        {
            unitsPerPixel = 1;
            var centerPosition = pdfViewer.GetDocumentPosition(center, true);
            if (centerPosition.PageNumber - 1 != pageIndex)
                return false;

            Point probe = new Point(center.X + deltaX, center.Y + deltaY);
            var probePosition = pdfViewer.GetDocumentPosition(probe, true);
            if (probePosition.PageNumber - 1 != pageIndex)
            {
                probe = new Point(center.X - deltaX, center.Y - deltaY);
                probePosition = pdfViewer.GetDocumentPosition(probe, true);
            }

            if (probePosition.PageNumber - 1 != pageIndex)
                return false;

            double pdfDistance = Math.Sqrt(
                Math.Pow(probePosition.Point.X - centerPosition.Point.X, 2) +
                Math.Pow(probePosition.Point.Y - centerPosition.Point.Y, 2));
            double clientDistance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            if (pdfDistance <= 0 || clientDistance <= 0)
                return false;

            unitsPerPixel = pdfDistance / clientDistance;
            return true;
        }

        bool TryCreateCenteredPdfRectangleFromClientSize(int pageIndex, Point center, double pdfWidth, double pdfHeight, out PdfPoint point1, out PdfPoint point2)
        {
            point1 = new PdfPoint();
            point2 = new PdfPoint();

            double pdfUnitsPerClientPixelX;
            double pdfUnitsPerClientPixelY;
            if (!TryGetPdfUnitsPerClientPixel(pageIndex, center, 20, 0, out pdfUnitsPerClientPixelX))
                return false;
            if (!TryGetPdfUnitsPerClientPixel(pageIndex, center, 0, 20, out pdfUnitsPerClientPixelY))
                return false;

            double clientWidth = Math.Max(pdfWidth / pdfUnitsPerClientPixelX, 12);
            double clientHeight = Math.Max(pdfHeight / pdfUnitsPerClientPixelY, 12);
            PointF topLeft = new PointF((float)(center.X - clientWidth / 2), (float)(center.Y - clientHeight / 2));
            PointF bottomRight = new PointF((float)(center.X + clientWidth / 2), (float)(center.Y + clientHeight / 2));

            if (!TryGetPdfPointFromClientPoint(pageIndex, topLeft, out point1))
                return false;
            if (!TryGetPdfPointFromClientPoint(pageIndex, bottomRight, out point2))
                return false;

            return true;
        }

        void DrawImageRectangle(Graphics graphics, GraphicsCoordinates rect)
        {
            var image = rect.ImageSign;
            string descripSign = rect.Descrip;
            sizeFont = graphics.MeasureString(descripSign, font);

            var desHeight = (int)(sizeFont.Height);
            var desWidth = (int)(sizeFont.Width);

            // Create a rectangle where graphics should be drawn
            var recRectangle = GetClientRectangle(rect);
            var recSignImage = Rectangle.FromLTRB(recRectangle.Left, recRectangle.Top, recRectangle.Right, recRectangle.Bottom - desHeight);

            // Draw a rectangle in the created area
            using (Pen pen = new Pen(rect == selectedSign ? Color.Red : Color.Blue))
                graphics.DrawRectangle(pen, recRectangle);

            // Vẽ chữ ký
            recSignImage = string.IsNullOrWhiteSpace(rect.Descrip) ? recRectangle : recSignImage;
            graphics.DrawImage(image, recSignImage);

            if (rect == selectedSign && rect.SignType == SignInfo.Sign)
            {
                Rectangle handle = GetResizeHandleRectangle(recRectangle);
                using (SolidBrush brush = new SolidBrush(Color.Red))
                    graphics.FillRectangle(brush, handle);
            }

            // Vẽ mô tả (Ngày tháng)
            if (string.IsNullOrWhiteSpace(rect.Descrip)) return;
            PointF point = new PointF(recRectangle.Right - desWidth, recRectangle.Bottom - desHeight);
            using (SolidBrush mybrush = new SolidBrush(Color.Black))
                graphics.DrawString(descripSign, font, mybrush, point);
        }

        void UpdateCurrentRect(Point location)
        {
            if (signs != null && currentSign != null && imageSign != null)
            {
                var documentPosition = pdfViewer.GetDocumentPosition(location, true);

                var desHeight = string.IsNullOrWhiteSpace(descrip) ? 0 : font.Height * 1.2f;
                var widthImage = imageSign.Width;
                var heightImage = imageSign.Height + desHeight;

                PdfPoint point1 = new PdfPoint();
                PdfPoint point2 = new PdfPoint();

                // Tạo toạ độ theo Chữ ký hoặc con dấu
                switch (signInfo)
                {
                    case SignInfo.Sign:
                        PointF startClient = pdfViewer.GetClientPoint(new PdfDocumentPosition(currentSign.PageIndex + 1, currentSign.Point1));
                        double aspectRatio = GetSignAspectRatio(imageSign, descrip);
                        double deltaClientX = location.X - startClient.X;
                        double deltaClientY = location.Y - startClient.Y;
                        double clientHeight = Math.Max(Math.Abs(deltaClientY), Math.Abs(deltaClientX) / aspectRatio);
                        double clientWidth = clientHeight * aspectRatio;
                        double directionX = deltaClientX < 0 ? -1 : 1;
                        double directionY = deltaClientY < 0 ? -1 : 1;

                        PointF endClient = new PointF(
                            (float)(startClient.X + clientWidth * directionX),
                            (float)(startClient.Y + clientHeight * directionY));

                        if (!TryGetPdfPointFromClientPoint(currentSign.PageIndex, endClient, out point2))
                            return;

                        point1 = currentSign.Point1;
                        break;
                    case SignInfo.Stamp:
                        widthImage = signSelect.WidImg ?? 10;
                        heightImage = signSelect.HgtImg ?? 2;

                        if (!TryCreateCenteredPdfRectangleFromClientSize(currentSign.PageIndex, location, widthImage, heightImage, out point1, out point2))
                            return;
                        break;
                    default:
                        break;
                }

                if (currentSign.PageIndex == documentPosition.PageNumber - 1)
                {
                    currentSign.Point1 = point1;
                    currentSign.Point2 = point2;
                }
            }
        }

        GraphicsCoordinates HitTestSign(Point location, out SignEditMode hitMode)
        {
            hitMode = SignEditMode.None;

            for (int i = signs.Count - 1; i >= 0; i--)
            {
                GraphicsCoordinates sign = signs[i];
                Rectangle rect = GetClientRectangle(sign);
                if (rect.Width <= 0 || rect.Height <= 0)
                    continue;

                if (sign.SignType == SignInfo.Sign && GetResizeHandleRectangle(rect).Contains(location))
                {
                    hitMode = SignEditMode.Resize;
                    return sign;
                }

                Rectangle hitArea = rect;
                hitArea.Inflate(4, 4);
                if (hitArea.Contains(location))
                {
                    hitMode = SignEditMode.Move;
                    return sign;
                }
            }

            return null;
        }

        void UpdateSelectedSign(Point location)
        {
            if (selectedSign == null || signEditMode == SignEditMode.None)
                return;

            var documentPosition = pdfViewer.GetDocumentPosition(location, true);
            if (documentPosition.PageNumber - 1 != selectedSign.PageIndex)
                return;

            switch (signEditMode)
            {
                case SignEditMode.Move:
                    double deltaX = documentPosition.Point.X - dragStartPoint.X;
                    double deltaY = documentPosition.Point.Y - dragStartPoint.Y;
                    selectedSign.Point1 = new PdfPoint(dragStartPoint1.X + deltaX, dragStartPoint1.Y + deltaY);
                    selectedSign.Point2 = new PdfPoint(dragStartPoint2.X + deltaX, dragStartPoint2.Y + deltaY);
                    break;
                case SignEditMode.Resize:
                    ResizeSelectedSign(location);
                    break;
            }
        }

        void ResizeSelectedSign(Point location)
        {
            double left = dragStartClientRectangle.Left;
            double top = dragStartClientRectangle.Top;
            double oldWidth = Math.Max(dragStartClientRectangle.Width, 1);
            double oldHeight = Math.Max(dragStartClientRectangle.Height, 1);
            double aspectRatio = oldWidth / oldHeight;

            double newWidth = Math.Max(Math.Abs(location.X - left), 12);
            double newHeight = Math.Max(newWidth / aspectRatio, 12);

            PdfPoint point1;
            PdfPoint point2;
            if (!TryGetPdfPointFromClientPoint(selectedSign.PageIndex, new PointF((float)left, (float)top), out point1))
                return;
            if (!TryGetPdfPointFromClientPoint(selectedSign.PageIndex, new PointF((float)(left + newWidth), (float)(top + newHeight)), out point2))
                return;

            selectedSign.Point1 = point1;
            selectedSign.Point2 = point2;
        }

        void UpdateCursor(Point location)
        {
            if (signEditMode != SignEditMode.None)
                return;

            SignEditMode hitMode;
            HitTestSign(location, out hitMode);

            switch (hitMode)
            {
                case SignEditMode.Resize:
                    pdfViewer.Cursor = Cursors.SizeNWSE;
                    break;
                case SignEditMode.Move:
                    pdfViewer.Cursor = Cursors.SizeAll;
                    break;
                default:
                    pdfViewer.Cursor = ActivateDrawing ? Cursors.Cross : Cursors.Default;
                    break;
            }
        }

        void EditSelectedSignDescription()
        {
            if (selectedSign == null || string.IsNullOrWhiteSpace(selectedSign.Descrip))
                return;

            DateTime currentDate;
            if (!DateTime.TryParseExact(selectedSign.Descrip, "yyyy.MM.dd", null, System.Globalization.DateTimeStyles.None, out currentDate))
                currentDate = DateTime.Now;

            DateEdit dateEdit = new DateEdit
            {
                EditValue = currentDate
            };
            dateEdit.Properties.DisplayFormat.FormatString = "yyyy.MM.dd";
            dateEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dateEdit.Properties.EditFormat.FormatString = "yyyy.MM.dd";
            dateEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dateEdit.Properties.Mask.EditMask = "yyyy.MM.dd";
            dateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            dateEdit.Properties.CalendarTimeProperties.DisplayFormat.FormatString = "yyyy.MM.dd";
            dateEdit.Properties.CalendarTimeProperties.EditFormat.FormatString = "yyyy.MM.dd";

            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "修改日期",
                DefaultButtonIndex = 0,
                Editor = dateEdit,
                DefaultResponse = currentDate
            };

            var result = XtraInputBox.Show(args);
            if (result == null)
                return;

            selectedSign.Descrip = Convert.ToDateTime(result).ToString("yyyy.MM.dd");
            pdfViewer.Invalidate();
        }

        void StopCreatingSign()
        {
            ActivateDrawing = false;
            signEditMode = SignEditMode.None;
            currentSign = null;
        }

        private void SaveDrawingAndReload()
        {
            string fileName = pdfViewer.DocumentFilePath;
            ValidatePdfFile(fileName, "PDF input file is empty.");

            string outFileName = EncryptionHelper.EncryptionFileName(fileName);

            string fileNameSave = Path.Combine(outDic, outFileName);
            string tempPath = null;
            string backupPath = null;

            pdfViewer.CloseDocument();
            try
            {
                using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
                {
                    // Load a document to the PdfDocumentProcessor instance
                    processor.LoadDocument(fileName);
                    foreach (var rect in signs)
                    {
                        // Create a PdfGraphics object
                        using (PdfGraphics graph = processor.CreateGraphics())
                        {
                            PdfPage page = processor.Document.Pages[rect.PageIndex];
                            PdfRectangle pageCropBox = page.CropBox;
                            PdfPoint p1 = new PdfPoint(rect.Point1.X, pageCropBox.Height - rect.Point1.Y);
                            PdfPoint p2 = new PdfPoint(rect.Point2.X, pageCropBox.Height - rect.Point2.Y);

                            var image = rect.ImageSign;
                            string desSign = rect.Descrip;
                            SizeF sizeFont = graph.MeasureString(desSign, font);

                            var desHeight = (float)(sizeFont.Height * 0.7);
                            var desWidth = (float)(sizeFont.Width * 0.75);

                            // Tạo khung vẽ vòng đỏ
                            RectangleF recRectangle = RectangleF.FromLTRB(
                                (float)Math.Min(p1.X, p2.X), (float)Math.Min(p1.Y, p2.Y),
                                (float)Math.Max(p1.X, p2.X), (float)Math.Max(p1.Y, p2.Y));

                            // Tạo khung vẽ chữ ký
                            RectangleF recSignImage = RectangleF.FromLTRB(
                               (float)Math.Min(p1.X, p2.X), (float)Math.Min(p1.Y, p2.Y),
                               (float)Math.Max(p1.X, p2.X), (float)Math.Max(p1.Y, p2.Y) - desHeight);

                            // Draw a rectangle in the created area
                            // graph.DrawRectangle(new Pen(Color.Red), recRectangle);

                            // Vẽ chữ ký
                            recSignImage = string.IsNullOrWhiteSpace(rect.Descrip) ? recRectangle : recSignImage;
                            graph.DrawImage(image, recSignImage);

                            // Vẽ phần mô tả chữ ký (Ngày tháng)
                            if (!string.IsNullOrWhiteSpace(rect.Descrip))
                            {
                                PointF point = new PointF((float)recRectangle.Right - desWidth, (float)recRectangle.Bottom - desHeight);
                                using (SolidBrush mybrush = new SolidBrush(Color.Black))
                                {
                                    graph.DrawString(desSign, font, mybrush, point);
                                }
                            }
                            graph.AddToPageForeground(page, 72, 72);
                        }
                    }
                    // Save the document
                    if (!Directory.Exists(outDic))
                        Directory.CreateDirectory(outDic);

                    tempPath = GetTempPdfPath(fileNameSave);
                    backupPath = GetBackupPdfPath(fileNameSave);
                    processor.SaveDocument(tempPath);
                }

                CommitSavedDocument(tempPath, fileNameSave, backupPath);
                tempPath = null;
                backupPath = null;
            }
            catch
            {
                DeleteIfExists(tempPath);
                DeleteIfExists(backupPath);
                throw;
            }
            OutFileName = outFileName;
            signs.Clear();
            currentSign = null;
            selectedSign = null;
            signEditMode = SignEditMode.None;
            ActivateDrawing = false;

            // Open the document in the PDF Viewer
            pdfViewer.LoadDocument(fileNameSave);
        }

        private string GetTempPdfPath(string finalPath)
        {
            string directory = Path.GetDirectoryName(finalPath);
            return Path.Combine(directory, $"{Path.GetFileName(finalPath)}.{Guid.NewGuid():N}.tmp");
        }

        private string GetBackupPdfPath(string finalPath)
        {
            string directory = Path.GetDirectoryName(finalPath);
            return Path.Combine(directory, $"{Path.GetFileName(finalPath)}.{Guid.NewGuid():N}.bak");
        }

        private void CommitSavedDocument(string tempPath, string finalPath, string backupPath)
        {
            ValidatePdfFile(tempPath, "PDF output file is empty.");

            if (File.Exists(finalPath))
            {
                File.Replace(tempPath, finalPath, backupPath, true);
                DeleteIfExists(backupPath);
            }
            else
            {
                File.Move(tempPath, finalPath);
            }

            ValidatePdfFile(finalPath, "PDF output file is empty.");
        }

        private void DeleteIfExists(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                File.Delete(path);
        }

        private void ValidatePdfFile(string path, string message)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists || fileInfo.Length == 0)
                throw new InvalidOperationException(message);

            using (PdfDocumentProcessor validator = new PdfDocumentProcessor())
            {
                validator.LoadDocument(path);
            }
        }

        #endregion

        private void f00_PdfTools_Load(object sender, EventArgs e)
        {
            Text = "PDF工具";
            try
            {
                ValidatePdfFile(filePath, "PDF input file is empty.");
                pdfViewer.LoadDocument(filePath);
            }
            catch (Exception ex)
            {
                OutFileName = null;
                MsgTP.MsgError($"PDF文件異常，無法簽名！\r\n{ex.Message}");
                BeginInvoke(new Action(Close));
                return;
            }

            //pdfViewer.CursorMode = PdfCursorMode.HandTool;
            InitializeMenuItems();

            // Load các chữ ký, con dấu
            var signUsrs = dm_SignUsersBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id).ToList();
            var idSigns = signUsrs.Select(r => r.IdSign).ToList();

            dmSigns = dm_SignBUS.Instance.GetListByIdSigns(idSigns).OrderBy(r => r.Prioritize).ToList();

            DefaultSign();
        }

        private void PdfViewer_Paint(object sender, PaintEventArgs e)
        {
            foreach (var r in signs)
                DrawImageRectangle(e.Graphics, r);

            if (currentSign != null)
                DrawImageRectangle(e.Graphics, currentSign);
        }

        private void PdfViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentSign != null && signEditMode == SignEditMode.Create)
            {
                UpdateCurrentRect(e.Location);
                pdfViewer.Invalidate();
                return;
            }

            if (selectedSign != null && (signEditMode == SignEditMode.Move || signEditMode == SignEditMode.Resize))
            {
                UpdateSelectedSign(e.Location);
                pdfViewer.Invalidate();
                return;
            }

            UpdateCursor(e.Location);
        }

        private void PdfViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ShowSignPopupMenu(e.Location);
                return;
            }

            // Convert the retrieved coordinates to the page coordinates
            if (currentSign != null && signEditMode == SignEditMode.Create)
            {
                UpdateCurrentRect(e.Location);
                if (!currentSign.IsEmpty && ActivateDrawing)
                {
                    // Add coordinates to the list
                    signs.Add(currentSign);
                    selectedSign = currentSign;
                    StopCreatingSign();
                }
                currentSign = null;
            }

            signEditMode = SignEditMode.None;
            pdfViewer.Capture = false;
            UpdateCursor(e.Location);
            pdfViewer.Invalidate();
        }

        private void PdfViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (!pdfViewer.IsDocumentOpened)
            {
                Console.WriteLine("---------- No document loaded ----------");
                return;
            }

            SignEditMode hitMode;
            GraphicsCoordinates hitSign = HitTestSign(e.Location, out hitMode);
            var position = pdfViewer.GetDocumentPosition(e.Location, true);

            if (hitSign != null)
            {
                selectedSign = hitSign;
                currentSign = null;
                signEditMode = hitMode;
                dragStartPoint = position.Point;
                dragStartPoint1 = hitSign.Point1;
                dragStartPoint2 = hitSign.Point2;
                dragStartClientRectangle = GetClientRectangle(hitSign);
                pdfViewer.Capture = true;
                pdfViewer.Invalidate();
                return;
            }

            selectedSign = null;

            if (!ActivateDrawing || imageSign == null)
            {
                signEditMode = SignEditMode.None;
                pdfViewer.Invalidate();
                return;
            }

            signEditMode = SignEditMode.Create;
            currentSign = new GraphicsCoordinates(position.PageNumber - 1, position.Point, position.Point, imageSign, descrip, signInfo);
            pdfViewer.Capture = true;
        }

        private void PdfViewer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Sửa ngày tháng được chuyển sang menu chuột phải để tránh thao tác nhầm.
        }

        private void ShowSignPopupMenu(Point location)
        {
            SignEditMode hitMode;
            GraphicsCoordinates hitSign = HitTestSign(location, out hitMode);
            if (hitSign == null || hitSign.SignType != SignInfo.Sign || string.IsNullOrWhiteSpace(hitSign.Descrip))
                return;

            contextMenuSign = hitSign;
            selectedSign = hitSign;
            pdfViewer.Invalidate();
            popupMenuSign.ShowPopup(pdfViewer, pdfViewer.PointToClient(System.Windows.Forms.Control.MousePosition));
        }

        private void PdfViewer_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine($"{e.Control} {e.KeyCode}");

            if (e.Control && (e.KeyCode == Keys.P || e.KeyCode == Keys.S || e.KeyCode == Keys.O))
            {
                e.SuppressKeyPress = true;
                return;
            }
        }

        private void PdfViewer_PopupMenuShowing(object sender, PdfPopupMenuShowingEventArgs e)
        {
            e.ItemLinks.Clear();
        }

        private void btnSignDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!dmSigns.Any(r => r.ImgType == 0))
            {
                MsgTP.MsgError("你沒有簽名！");
                return;
            }

            DefaultSign();

            // Change the activation indicator
            ActivateDrawing = true;
            pdfViewer.Invalidate();
            signInfo = SignInfo.Sign;
        }

        private void btnClearSign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            signs.Clear();
            currentSign = null;
            selectedSign = null;
            signEditMode = SignEditMode.None;
            ActivateDrawing = false;
            pdfViewer.Invalidate();
        }

        private void btnAdvanced_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!dmSigns.Any(r => r.ImgType == 0))
            {
                MsgTP.MsgError("你沒有簽名！");
                return;
            }

            ActivateDrawing = true;
            pdfViewer.Invalidate();

            uc00_AdvancedSign ucAdvanced = new uc00_AdvancedSign(FullSign: fullSign);
            ucAdvanced.signInfo = SignInfo.Sign;
            if (XtraDialog.Show(ucAdvanced, "修改簽名", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                DefaultSign();
                return;
            }

            signInfo = SignInfo.Sign;
            imageSign = ucAdvanced.ImageSign;
            signSelect = ucAdvanced.SignSelect;
            descrip = ucAdvanced.DescripSign;
        }

        private void btnStamp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!dmSigns.Any(r => r.ImgType == 1))
            {
                MsgTP.MsgError("你沒有蓋章！");
                return;
            }

            ActivateDrawing = true;
            pdfViewer.Invalidate();

            uc00_AdvancedSign ucAdvanced = new uc00_AdvancedSign(FullSign: fullSign);
            ucAdvanced.signInfo = SignInfo.Stamp;
            if (XtraDialog.Show(ucAdvanced, "修改簽名", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                DefaultSign();
                return;
            }

            signInfo = SignInfo.Stamp;
            imageSign = ucAdvanced.ImageSign;
            signSelect = ucAdvanced.SignSelect;
            descrip = "";
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (signs.Count() == 0)
            {
                MsgTP.MsgError("你還沒簽名！");
                return;
            }

            try
            {
                SaveDrawingAndReload();
            }
            catch (Exception ex)
            {
                OutFileName = null;
                MsgTP.MsgError($"PDF保存失敗，請重新簽名！\r\n{ex.Message}");
                return;
            }

            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "退回文件原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit(),
                DefaultResponse = ""
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;
            Describe = result?.ToString() ?? "";

            Close();
        }
    }
}
