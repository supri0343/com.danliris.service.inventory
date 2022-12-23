using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentWasteProductionViewModel.ReceiptWaste;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.PDFTemplates.GarmentWaste
{
    public class GarmentReceiptWastePDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(GarmentReceiptWasteProductionViewModel viewModel)
        {
            const int MARGIN = 20;
            var timeoffset = 7;

            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 14);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font body_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font normal_font_underlined = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8, Font.UNDERLINE);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);

            Document document = new Document(PageSize.A5.Rotate(), MARGIN, MARGIN, MARGIN, 70);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.PageEvent = new GarmentReceipWastetSignPDFTemplatePageEvent();

            document.Open();
            #region TITLE
            Paragraph title = new Paragraph($"BON TERIMA {viewModel.WasteType}", header_font);
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            Paragraph danliris = new Paragraph("PT. DANLIRIS", bold_font);
            document.Add(danliris);
            Paragraph address = new Paragraph("BANARAN, GROGOL, SUKOHARJO", normal_font);
            document.Add(address);

            Paragraph p = new Paragraph(new Chunk(new LineSeparator(0.5F, 100.0F, BaseColor.Black, Element.ALIGN_LEFT, 1)));
            p.SpacingBefore = -10f;
            document.Add(p);
            #endregion

            #region HEADER
            Paragraph avalType = new Paragraph(viewModel.WasteType, bold_font);
            avalType.Alignment = Element.ALIGN_CENTER;
            document.Add(avalType);

            PdfPTable tableHeader = new PdfPTable(6);
            tableHeader.WidthPercentage = 100;
            tableHeader.HorizontalAlignment = Element.ALIGN_LEFT;
            tableHeader.SetWidths(new float[] { 2f, 0.1f, 5f, 2f, 0.1f, 4f });

            PdfPCell cellHeaderContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellHeaderContentLeft.Phrase = new Phrase("NO", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(":", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.GarmentReceiptWasteNo, bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            //cellHeaderContentLeft.Phrase = new Phrase("Terima dari", normal_font);
            //tableHeader.AddCell(cellHeaderContentLeft);
            //cellHeaderContentLeft.Phrase = new Phrase(":", normal_font);
            //tableHeader.AddCell(cellHeaderContentLeft);
            //cellHeaderContentLeft.Phrase = new Phrase(viewModel.UnitFrom.Name, normal_font);
            //tableHeader.AddCell(cellHeaderContentLeft);

            cellHeaderContentLeft.Phrase = new Phrase("Tanggal", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(":", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.ReceiptDate.AddHours(timeoffset).ToString("dd MMM yyyy"), normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            cellHeaderContentLeft.Phrase = new Phrase("", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase("", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase("", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            tableHeader.SpacingAfter = 15f;
            tableHeader.SpacingBefore = 15f;
            document.Add(tableHeader);

            #endregion

            #region BODY

      
                PdfPTable bodyTable = new PdfPTable(9);
                bodyTable.WidthPercentage = 100;
                bodyTable.SetWidths(new float[] { 1f, 3f, 3f, 4f, 3f, 3f,5f, 2f, 2f });

                PdfPCell bodyTableCellRightBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                PdfPCell bodyTableCellLeftBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                PdfPCell bodyTableCellCenterBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
                PdfPCell bodyTableCellNoBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

                bodyTableCellCenterBorder.Phrase = new Phrase("NO", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("KODE BARANG", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("NOMOR BC", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("TANGGAL BC", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("TIPE BC", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("NO RO", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("ARTIKEL", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("QTY", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("SAT", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);

                int index = 0;
                double totalQuantity = 0;
                foreach (var item in viewModel.Items)
                {
                    index++;
                    bodyTableCellLeftBorder.Phrase = new Phrase(index.ToString(), normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.Product.Code, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.BCNo == null ? "-" : item.BCNo, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.BCDate == null ? "-" : item.BCDate?.ToString("dd-MMM-yyyy"), normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.BCType == null ? "-" : item.BCType, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.RONo, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.Article, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellRightBorder.Phrase = new Phrase(item.Quantity.ToString(), normal_font);
                    bodyTable.AddCell(bodyTableCellRightBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase("KG", normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);

                    totalQuantity += item.Quantity;
                }
                bodyTableCellRightBorder.Phrase = new Phrase("TOTAL", bold_font);
                bodyTableCellRightBorder.Colspan = 7;
                bodyTable.AddCell(bodyTableCellRightBorder);
                bodyTableCellRightBorder.Phrase = new Phrase(totalQuantity.ToString(), bold_font);
                bodyTableCellRightBorder.Colspan = 1;
                bodyTable.AddCell(bodyTableCellRightBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase("KG", bold_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);

                //bodyTableCellNoBorder.Phrase = new Phrase("", bold_font);
                //bodyTableCellNoBorder.Colspan = 8;
                //bodyTable.AddCell(bodyTableCellNoBorder);
                //bodyTableCellRightBorder.Phrase = new Phrase(viewModel.TotalAval.ToString(), bold_font);
                //bodyTableCellRightBorder.Colspan = 1;
                //bodyTable.AddCell(bodyTableCellRightBorder);
                //bodyTableCellLeftBorder.Phrase = new Phrase("KG", bold_font);
                //bodyTable.AddCell(bodyTableCellLeftBorder);

                bodyTable.SpacingAfter = 25f;
                document.Add(bodyTable);
            

            #endregion

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        public class GarmentReceipWastetSignPDFTemplatePageEvent : iTextSharp.text.pdf.PdfPageEventHelper
        {
            public override void OnStartPage(PdfWriter writer, Document document)
            {

                PdfContentByte cb = writer.DirectContent;
                cb.BeginText();
                Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
                Font normal_font_underlined = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8, Font.UNDERLINE);

                float height = writer.PageSize.Height, width = writer.PageSize.Width;
                float marginLeft = document.LeftMargin - 10, marginTop = document.TopMargin, marginRight = document.RightMargin - 10;

                cb.SetFontAndSize(bf, 8);

                #region SIGNATURE
                var signY = document.BottomMargin;
                var signX = document.RightMargin + 50;
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Pengirim,", document.LeftMargin + 100, signY, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "(_____________________________)", document.LeftMargin + 100, signY - 40, 0);
                //cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Gudang Unit", document.LeftMargin + 100, signY - 50, 0);

                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Penerima,", document.RightMargin + 450, signY, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "(_____________________________)", document.RightMargin + 450, signY - 40, 0);
                //cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Gudang Sisa", document.RightMargin + 450, signY - 50, 0);

                #endregion

                cb.EndText();
            }
        }
    }
}
