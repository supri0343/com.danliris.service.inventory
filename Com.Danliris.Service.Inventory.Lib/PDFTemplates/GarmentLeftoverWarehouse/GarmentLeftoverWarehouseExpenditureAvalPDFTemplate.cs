using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAval;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.PDFTemplates.GarmentLeftoverWarehouse
{
    public class GarmentLeftoverWarehouseExpenditureAvalPDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(GarmentLeftoverWarehouseExpenditureAvalViewModel viewModel)
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
            string signee = viewModel.ExpenditureTo == "JUAL LOKAL" ? viewModel.Buyer.Name : "";
            writer.PageEvent = new GarmentLeftoverWarehouseExpenditureSignPDFTemplatePageEvent(signee);

            document.Open();
            #region TITLE
            Paragraph title = new Paragraph("BON KELUAR AVAL GUDANG SISA", header_font);
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
            Paragraph avalType = new Paragraph(viewModel.AvalType, bold_font);
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
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.AvalExpenditureNo, bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            cellHeaderContentLeft.Phrase = new Phrase("Tujuan", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(":", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.ExpenditureTo, normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            cellHeaderContentLeft.Phrase = new Phrase("Tanggal", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(":", normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(viewModel.ExpenditureDate.GetValueOrDefault().ToOffset(new TimeSpan(timeoffset, 0, 0)).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("en-EN")), normal_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            if (viewModel.ExpenditureTo == "JUAL LOKAL")
            {
                cellHeaderContentLeft.Phrase = new Phrase("Buyer", normal_font);
                tableHeader.AddCell(cellHeaderContentLeft);
                cellHeaderContentLeft.Phrase = new Phrase(":", normal_font);
                tableHeader.AddCell(cellHeaderContentLeft);
                cellHeaderContentLeft.Phrase = new Phrase(viewModel.Buyer.Name, normal_font);
                tableHeader.AddCell(cellHeaderContentLeft);
            }
            else
            {
                cellHeaderContentLeft.Phrase = new Phrase("Keterangan Lain-lain", normal_font);
                tableHeader.AddCell(cellHeaderContentLeft);
                cellHeaderContentLeft.Phrase = new Phrase(":", normal_font);
                tableHeader.AddCell(cellHeaderContentLeft);
                cellHeaderContentLeft.Phrase = new Phrase(viewModel.OtherDescription, normal_font);
                tableHeader.AddCell(cellHeaderContentLeft);
            }

            tableHeader.SpacingAfter = 15f;
            tableHeader.SpacingBefore = 15f;
            document.Add(tableHeader);

            #endregion

            #region BODY
            if (viewModel.AvalType == "AVAL BAHAN PENOLONG")
            {
                PdfPTable bodyTable = new PdfPTable(6);
                bodyTable.WidthPercentage = 100;
                bodyTable.SetWidths(new float[] { 1f, 3f, 5f, 3f, 3f, 3f });

                PdfPCell bodyTableCellRightBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                PdfPCell bodyTableCellLeftBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                PdfPCell bodyTableCellCenterBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

                bodyTableCellCenterBorder.Phrase = new Phrase("NO", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("KODE BARANG", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("NAMA BARANG", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("UNIT ASAL", bold_font);
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
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.Product.Name, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.Unit.Name, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellRightBorder.Phrase = new Phrase(item.Quantity.ToString(), normal_font);
                    bodyTable.AddCell(bodyTableCellRightBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.Uom.Unit, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);

                    totalQuantity += item.Quantity;
                }
                bodyTableCellRightBorder.Phrase = new Phrase("TOTAL", bold_font);
                bodyTableCellRightBorder.Colspan = 4;
                bodyTable.AddCell(bodyTableCellRightBorder);
                bodyTableCellRightBorder.Phrase = new Phrase(totalQuantity.ToString(), bold_font);
                bodyTableCellRightBorder.Colspan = 1;
                bodyTable.AddCell(bodyTableCellRightBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase("KG", bold_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);

                bodyTable.SpacingAfter = 25f;
                document.Add(bodyTable);
            }
            else if (viewModel.AvalType == "AVAL FABRIC")
            {
                PdfPTable bodyTable = new PdfPTable(5);
                bodyTable.WidthPercentage = 100;
                bodyTable.SetWidths(new float[] { 1f, 3f, 5f, 3f, 3f });

                PdfPCell bodyTableCellRightBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                PdfPCell bodyTableCellLeftBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                PdfPCell bodyTableCellCenterBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
                PdfPCell bodyTableCellNoBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

                bodyTableCellCenterBorder.Phrase = new Phrase("NO", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("NO BON MASUK", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("UNIT ASAL", bold_font);
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
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.AvalReceiptNo, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.Unit.Name, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellRightBorder.Phrase = new Phrase(item.ActualQuantity.ToString(), normal_font);
                    bodyTable.AddCell(bodyTableCellRightBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase("KG", normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);

                    totalQuantity += item.ActualQuantity;
                }
                bodyTableCellRightBorder.Phrase = new Phrase("TOTAL", bold_font);
                bodyTableCellRightBorder.Colspan = 3;
                bodyTable.AddCell(bodyTableCellRightBorder);
                bodyTableCellRightBorder.Phrase = new Phrase(totalQuantity.ToString(), bold_font);
                bodyTableCellRightBorder.Colspan = 1;
                bodyTable.AddCell(bodyTableCellRightBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase("KG", bold_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);

                bodyTable.SpacingAfter = 25f;
                document.Add(bodyTable);
            }
            else if (viewModel.AvalType == "AVAL KOMPONEN")
            {
                PdfPTable bodyTable = new PdfPTable(5);
                bodyTable.WidthPercentage = 100;
                bodyTable.SetWidths(new float[] { 1f, 4f, 3f, 3f, 3f });

                PdfPCell bodyTableCellRightBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                PdfPCell bodyTableCellLeftBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                PdfPCell bodyTableCellCenterBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
                PdfPCell bodyTableCellNoBorder = new PdfPCell() { MinimumHeight = 15, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

                bodyTableCellCenterBorder.Phrase = new Phrase("NO", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("NO BON MASUK", bold_font);
                bodyTable.AddCell(bodyTableCellCenterBorder);
                bodyTableCellCenterBorder.Phrase = new Phrase("UNIT ASAL", bold_font);
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
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.AvalReceiptNo, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase(item.Unit.Name, normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);
                    bodyTableCellRightBorder.Phrase = new Phrase(item.ActualQuantity.ToString(), normal_font);
                    bodyTable.AddCell(bodyTableCellRightBorder);
                    bodyTableCellLeftBorder.Phrase = new Phrase("KG", normal_font);
                    bodyTable.AddCell(bodyTableCellLeftBorder);

                    totalQuantity += item.ActualQuantity;
                }
                bodyTableCellRightBorder.Phrase = new Phrase("TOTAL", bold_font);
                bodyTableCellRightBorder.Colspan = 3;
                bodyTable.AddCell(bodyTableCellRightBorder);
                bodyTableCellRightBorder.Phrase = new Phrase(totalQuantity.ToString(), bold_font);
                bodyTableCellRightBorder.Colspan = 1;
                bodyTable.AddCell(bodyTableCellRightBorder);
                bodyTableCellLeftBorder.Phrase = new Phrase("PCS", bold_font);
                bodyTable.AddCell(bodyTableCellLeftBorder);

                bodyTable.SpacingAfter = 25f;
                document.Add(bodyTable);
            }

            #endregion

            #region remark
            PdfPTable tableRemark = new PdfPTable(3);
            tableRemark.WidthPercentage = 80;
            tableRemark.HorizontalAlignment = Element.ALIGN_LEFT;
            tableRemark.SetWidths(new float[] { 1f, 0.1f, 8f });

            PdfPCell cellRemarkContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellRemarkContentLeft.Phrase = new Phrase("Keterangan", normal_font);
            tableRemark.AddCell(cellRemarkContentLeft);
            cellRemarkContentLeft.Phrase = new Phrase(":", normal_font);
            tableRemark.AddCell(cellRemarkContentLeft);
            cellRemarkContentLeft.Phrase = new Phrase(viewModel.Description, normal_font);
            tableRemark.AddCell(cellRemarkContentLeft);

            document.Add(tableRemark);
            #endregion

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }
    }
}
