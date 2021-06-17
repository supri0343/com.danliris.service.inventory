using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.PDFTemplates.GarmentLeftoverWarehouse
{
    public class GarmentLeftoverWarehouseReceiptFabricPDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(GarmentLeftoverWarehouseReceiptFabricViewModel viewModel)
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
            writer.PageEvent = new GarmentLeftoverWarehouseReceiptSignPDFTemplatePageEvent();

            document.Open();
            #region TITLE
            Paragraph title = new Paragraph("BON TERIMA FABRIC GUDANG SISA", header_font);
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            Paragraph danliris= new Paragraph("PT. DANLIRIS", bold_font);
            document.Add(danliris);
            Paragraph address = new Paragraph("BANARAN, GROGOL, SUKOHARJO", normal_font);
            document.Add(address);

            Paragraph p = new Paragraph(new Chunk(new LineSeparator(0.5F, 100.0F, BaseColor.Black, Element.ALIGN_LEFT, 1)));
            p.SpacingBefore = -10f;
            document.Add(p);
            #endregion

            #region HEADER
            PdfPTable tableHeader = new PdfPTable(6);
            tableHeader.WidthPercentage = 100;
            tableHeader.HorizontalAlignment = Element.ALIGN_LEFT;
            tableHeader.SetWidths(new float[] { 2f, 0.1f, 5f, 2f, 0.1f, 4f });

            PdfPCell cellHeaderContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellHeaderContentLeft.Phrase = new Phrase("NO", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(":", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.ReceiptNoteNo, bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);


            cellHeaderContentLeft.Phrase = new Phrase("Terima dari", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(":", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.UnitFrom.Name, normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            cellHeaderContentLeft.Phrase = new Phrase("Tanggal", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(":", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.ReceiptDate.GetValueOrDefault().ToOffset(new TimeSpan(timeoffset, 0, 0)).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("en-EN")), normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            cellHeaderContentLeft.Phrase = new Phrase("Dasar terima", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(":", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.UENNo, normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);


            tableHeader.SpacingAfter = 15f;
            document.Add(tableHeader);

            #endregion

            #region BODY
            PdfPTable bodyTable = new PdfPTable(7);
            bodyTable.WidthPercentage = 100;
            bodyTable.SetWidths(new float[] { 1f, 3f, 3f, 5f, 6f, 3f, 3f });

            PdfPCell bodyTableCellRightBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment=Element.ALIGN_RIGHT };
            PdfPCell bodyTableCellLeftBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell bodyTableCellCenterBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            bodyTableCellCenterBorder.Phrase = new Phrase("NO", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);
            bodyTableCellCenterBorder.Phrase = new Phrase("KODE BARANG", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);
            bodyTableCellCenterBorder.Phrase = new Phrase("NO. PO", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);
            bodyTableCellCenterBorder.Phrase = new Phrase("KOMPOSISI", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);
            bodyTableCellCenterBorder.Phrase = new Phrase("CONST,YARN,WIDTH", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);
            bodyTableCellCenterBorder.Phrase = new Phrase("QTY", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);
            bodyTableCellCenterBorder.Phrase = new Phrase("SAT", bold_font);
            bodyTable.AddCell(bodyTableCellCenterBorder);

            int index = 0;
            double totalQuantity = 0;
            foreach(var item in viewModel.Items)
            {
                index++;
                bodyTableCellLeftBorder.Phrase = new Phrase(index.ToString(), normal_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase(item.Product.Code, normal_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase(item.POSerialNumber, normal_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase(item.Composition, normal_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase(item.FabricRemark, normal_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);
                bodyTableCellRightBorder.Phrase = new Phrase(item.Quantity.ToString(), normal_font);
                bodyTable.AddCell(bodyTableCellRightBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase(item.Uom.Unit, normal_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);

                totalQuantity += item.Quantity;
            }

            bodyTableCellRightBorder.Phrase = new Phrase("TOTAL", bold_font);
            bodyTableCellRightBorder.Colspan = 5;
            bodyTable.AddCell(bodyTableCellRightBorder);
            bodyTableCellRightBorder.Phrase = new Phrase(totalQuantity.ToString(), bold_font);
            bodyTableCellRightBorder.Colspan = 1;
            bodyTable.AddCell(bodyTableCellRightBorder);
            bodyTableCellLeftBorder.Phrase = new Phrase("MT", bold_font);
            bodyTable.AddCell(bodyTableCellLeftBorder);

            bodyTable.SpacingAfter = 25f;
            document.Add(bodyTable);
            #endregion

            #region sign
            //PdfPTable tableSign = new PdfPTable(2);
            //tableSign.WidthPercentage = 100;
            //tableSign.SetWidths(new float[] { 1f, 1f});

            //PdfPCell cellBodySignNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            //cellBodySignNoBorder.Phrase = new Phrase("Pengirim,\n\n\n\n\n", normal_font);
            //tableSign.AddCell(cellBodySignNoBorder);
            //cellBodySignNoBorder.Phrase = new Phrase("Penerima,\n\n\n\n\n", normal_font);
            //tableSign.AddCell(cellBodySignNoBorder);
            //cellBodySignNoBorder.Phrase = new Phrase("(_____________________________)", normal_font);
            //tableSign.AddCell(cellBodySignNoBorder);
            //cellBodySignNoBorder.Phrase = new Phrase("(_____________________________)", normal_font);
            //tableSign.AddCell(cellBodySignNoBorder);


            //cellBodySignNoBorder.Phrase = new Phrase("Gudang Unit", normal_font);
            //tableSign.AddCell(cellBodySignNoBorder);
            //cellBodySignNoBorder.Phrase = new Phrase("Gudang Sisa", normal_font);
            //tableSign.AddCell(cellBodySignNoBorder);

            //document.Add(tableSign);
            #endregion

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }
    }

    public class GarmentLeftoverWarehouseReceiptSignPDFTemplatePageEvent : iTextSharp.text.pdf.PdfPageEventHelper
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
            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Gudang Unit", document.LeftMargin + 100, signY - 50, 0);

            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Penerima,", document.RightMargin + 450, signY, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "(_____________________________)", document.RightMargin + 450, signY - 40, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Gudang Sisa", document.RightMargin + 450, signY - 50, 0);

            #endregion

            cb.EndText();
        }
    }
}
