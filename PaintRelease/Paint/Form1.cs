using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;



namespace Paint
{
    public partial class paint : Form
    {
        public paint()
        {
            InitializeComponent();
        }

        bool isFillShape;
        bool canDraw;

        int xStart, yStart;
        int xEnd, yEnd;
        int eraseSize;

        int openingTextBoxTextHeight;

        int sizeCoefWidth;
        int sizeCoefHeight;

        Point openingGroupBoxColorLocation;
        Point openingTrackBarWidthLocation;
        Point openingLabelWidthLocation;

        Point usingTextGroupBoxColorLocation;
        Point usingTextTrackBarWidthLocation;
        Point usingTextLabelWidthLocation;

        Graphics graphics;
        Bitmap currentBitmap;
        Bitmap saveBitmap;
        Pen pen;
        SolidBrush brush;
        SolidBrush erase;
        SolidBrush pencil;

        Point[] pointsTriangle = new Point[3];
        Point[] pointsPentagon = new Point[5];

        Image brightImage;
        Image contrastImage;

        enum tools { Empty, Line, Rectangle, Pentagon, Ellipse, Triangle, Erase, Text, Pencil, Pipette, Palette };

        tools currentTools;

        private void labelLineColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pen.Color = colorDialog.Color;
                labelLineColor.BackColor = colorDialog.Color;

                pencil.Color = colorDialog.Color;
                labelLineColor.BackColor = colorDialog.Color;
            }
        }

        private void labelFillColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                brush.Color = colorDialog.Color;
                labelFillColor.BackColor = colorDialog.Color;
            }
        }

        private void buttonLine_Click(object sender, EventArgs e)
        {
            currentTools = tools.Line;
        }

        private void buttonRectangle_Click(object sender, EventArgs e)
        {
            currentTools = tools.Rectangle;
        }

        private void buttonEllipse_Click(object sender, EventArgs e)
        {
            currentTools = tools.Ellipse;
        }

        private void buttonErase_Click(object sender, EventArgs e)
        {
            currentTools = tools.Erase;
        }

        private void paint_Load(object sender, EventArgs e)
        {
            currentTools = tools.Empty;

            isFillShape = false;
            canDraw = false;

            eraseSize = 1;
            sizeCoefWidth = 1;
            sizeCoefHeight = 1;

            currentBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            graphics = Graphics.FromImage(currentBitmap);
            saveBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            pen = new Pen(labelLineColor.BackColor, 1);
            brush = new SolidBrush(labelFillColor.BackColor);
            erase = new SolidBrush(Color.White);
            pencil = new SolidBrush(labelLineColor.BackColor);

            labelLineColor.BackColor = colorDialog.Color;
            labelFillColor.BackColor = colorDialog.Color;
            EnableTrackBars(false);

            SetPictureFileDialogFilter();

            openingTextBoxTextHeight = textBoxText.Height;

            openingGroupBoxColorLocation = new Point(groupBoxColor.Location.X, groupBoxColor.Location.Y - textBoxText.Height);
            openingTrackBarWidthLocation = new Point(trackBarWight.Location.X, trackBarWight.Location.Y - textBoxText.Height);
            openingLabelWidthLocation = new Point(labelWidth.Location.X, labelWidth.Location.Y - textBoxText.Height);
            SetGroupBoxLocation(openingGroupBoxColorLocation, openingTrackBarWidthLocation, openingLabelWidthLocation);
            SetUsingTextLocation();

            EmptyHueLabels();

            SetOriginalTrackBarsValues();
        }

        private void SetUsingTextLocation()
        {
            usingTextGroupBoxColorLocation = new Point(openingGroupBoxColorLocation.X, openingGroupBoxColorLocation.Y + textBoxText.Height);
            usingTextTrackBarWidthLocation = new Point(openingTrackBarWidthLocation.X, openingTrackBarWidthLocation.Y + textBoxText.Height);
            usingTextLabelWidthLocation = new Point(openingLabelWidthLocation.X, openingLabelWidthLocation.Y + textBoxText.Height);
        }

        private void SetGroupBoxLocation(Point usingTextGroupBoxColorLocation, Point usingTextTrackBarWidthLocation, Point usingTextLabelWidthLocation)
        {
            groupBoxColor.Location = usingTextGroupBoxColorLocation;
            trackBarWight.Location = usingTextTrackBarWidthLocation;
            labelWidth.Location = usingTextLabelWidthLocation;
        }

        private void EnableTrackBars(bool isTrackBarEnabled)
        {
            trackBarBrightness.Enabled = isTrackBarEnabled;
            trackBarSaturation.Enabled = isTrackBarEnabled;
        }

        private void SetPictureFileDialogFilter()
        {
            openFileDialog.Filter = "img(*.jpg; .png)|.jpg; *.png";
            saveFileDialog.Filter = "img(*.jpg; .png)|.jpg; *.png";
        }

        private void buttonText_Click(object sender, EventArgs e)
        {
            currentTools = tools.Text;
            textBoxText.Visible = true;
            buttonTextStyle.Visible = true;

            SetGroupBoxLocation(usingTextGroupBoxColorLocation, usingTextTrackBarWidthLocation, usingTextLabelWidthLocation);
        }
        private void buttonPen_Click(object sender, EventArgs e)
        {
            currentTools = tools.Pencil;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            canDraw = true;
            xStart = e.X * sizeCoefWidth;
            yStart = e.Y * sizeCoefHeight;

            pen.Color = labelLineColor.BackColor;
            brush.Color = labelFillColor.BackColor;
            pencil.Color = labelLineColor.BackColor;
            if (currentTools != tools.Text)
            {
                textBoxText.Visible = false;
                buttonTextStyle.Visible = false;
                SetGroupBoxLocation(openingGroupBoxColorLocation, openingTrackBarWidthLocation, openingLabelWidthLocation);
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            canDraw = false;
            saveBitmap = (Bitmap)currentBitmap.Clone();
            EmptyHueLabels();
            brightImage = saveBitmap;
            contrastImage = saveBitmap;
        }

        private void EmptyHueLabels()
        {
            labelSaturation.Text = string.Empty;
            labelBrightness.Text = string.Empty;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (canDraw == true)
            {
                xEnd = e.X * sizeCoefWidth;
                yEnd = e.Y * sizeCoefHeight;

                currentBitmap.Dispose();
                currentBitmap = (Bitmap)saveBitmap.Clone();
                graphics = Graphics.FromImage(currentBitmap);

                switch (currentTools)
                {
                    case tools.Line:
                        graphics = Graphics.FromImage(currentBitmap);

                        graphics.DrawLine(pen, xStart, yStart, xEnd, yEnd);

                        break;
                    case tools.Triangle:
                        pointsTriangle[0].X = (xStart + xEnd) / 2; pointsTriangle[0].Y = yStart;
                        pointsTriangle[1].X = xEnd; pointsTriangle[1].Y = yEnd;
                        pointsTriangle[2].X = xStart; pointsTriangle[2].Y = yEnd;

                        graphics.DrawPolygon(pen, pointsTriangle);
                        if (isFillShape == true)
                        {
                            graphics.FillPolygon(brush, pointsTriangle);
                        }
                        break;
                    case tools.Pentagon:
                        pointsPentagon[0].X = (xStart + xEnd) / 2; pointsPentagon[0].Y = yStart;
                        pointsPentagon[1].X = xEnd; pointsPentagon[1].Y = (yStart + yEnd) / 2;
                        pointsPentagon[2].X = ((xStart + xEnd) / 2 + xEnd) / 2; pointsPentagon[2].Y = yEnd;
                        pointsPentagon[3].X = ((xStart + xEnd) / 2 + xStart) / 2; pointsPentagon[3].Y = yEnd;
                        pointsPentagon[4].X = xStart; pointsPentagon[4].Y = (yStart + yEnd) / 2;

                        graphics.DrawPolygon(pen, pointsPentagon);
                        if (isFillShape == true)
                        {
                            graphics.FillPolygon(brush, pointsPentagon);
                        }
                        break;
                    case tools.Rectangle:
                        graphics = Graphics.FromImage(currentBitmap);
                        if (isFillShape == true)
                        {
                            if (xStart - xEnd > 0 && yStart - yEnd > 0)
                            {
                                graphics.FillRectangle(brush, xEnd, yEnd, xStart - xEnd, yStart - yEnd);
                                graphics.DrawRectangle(pen, xEnd, yEnd, xStart - xEnd, yStart - yEnd);
                            }
                            if (xStart - xEnd > 0 && yEnd - yStart > 0)
                            {
                                graphics.FillRectangle(brush, xEnd, yStart, xStart - xEnd, yEnd - yStart);
                                graphics.DrawRectangle(pen, xEnd, yStart, xStart - xEnd, yEnd - yStart);
                            }
                            if (xEnd - xStart > 0 && yStart - yEnd > 0)
                            {
                                graphics.FillRectangle(brush, xStart, yEnd, xEnd - xStart, yStart - yEnd);
                                graphics.DrawRectangle(pen, xStart, yEnd, xEnd - xStart, yStart - yEnd);
                            }
                            if (xEnd - xStart > 0 && yEnd - yStart > 0)
                            {
                                graphics.FillRectangle(brush, xStart, yStart, xEnd - xStart, yEnd - yStart);
                                graphics.DrawRectangle(pen, xStart, yStart, xEnd - xStart, yEnd - yStart);
                            }
                        }
                        else
                        {
                            if (xStart - xEnd > 0 && yStart - yEnd > 0)
                            {
                                graphics.DrawRectangle(pen, xEnd, yEnd, xStart - xEnd, yStart - yEnd);
                            }
                            if (xStart - xEnd > 0 && yEnd - yStart > 0)
                            {
                                graphics.DrawRectangle(pen, xEnd, yStart, xStart - xEnd, yEnd - yStart);
                            }
                            if (xEnd - xStart > 0 && yStart - yEnd > 0)
                            {
                                graphics.DrawRectangle(pen, xStart, yEnd, xEnd - xStart, yStart - yEnd);
                            }
                            if (xEnd - xStart > 0 && yEnd - yStart > 0)
                            {
                                graphics.DrawRectangle(pen, xStart, yStart, xEnd - xStart, yEnd - yStart);
                            }
                        }
                        break;
                    case tools.Ellipse:
                        graphics = Graphics.FromImage(currentBitmap);
                        if (isFillShape == true)
                        {
                            graphics.FillEllipse(brush, xStart, yStart, xEnd - xStart, yEnd - yStart);
                            graphics.DrawEllipse(pen, xStart, yStart, xEnd - xStart, yEnd - yStart);
                        }
                        else
                        {
                            graphics.DrawEllipse(pen, xStart, yStart, xEnd - xStart, yEnd - yStart);
                        }
                        break;
                    case tools.Text:
                        graphics.DrawString(textBoxText.Text, textBoxText.Font, pencil, e.X * sizeCoefWidth, e.Y * sizeCoefHeight);
                        break;
                    case tools.Erase:
                        graphics.FillRectangle(erase, (e.X - eraseSize / 2) * sizeCoefWidth, (e.Y - eraseSize / 2) * sizeCoefHeight, eraseSize, eraseSize);
                        saveBitmap = (Bitmap)currentBitmap.Clone();
                        break;
                    case tools.Pencil:
                        graphics.FillRectangle(pencil, (e.X - eraseSize / 2) * sizeCoefWidth, (e.Y - eraseSize / 2) * sizeCoefHeight, eraseSize, eraseSize);
                        saveBitmap = (Bitmap)currentBitmap.Clone();
                        break;
                    case tools.Pipette:
                        if (isFillShape == true)
                        {
                            labelFillColor.BackColor = saveBitmap.GetPixel(e.X * sizeCoefWidth, e.Y * sizeCoefHeight);
                            labelSaturation.Text = (saveBitmap.GetPixel(e.X * sizeCoefWidth, e.Y * sizeCoefHeight).GetSaturation()).ToString();
                            labelBrightness.Text = (saveBitmap.GetPixel(e.X * sizeCoefWidth, e.Y * sizeCoefHeight).GetBrightness()).ToString();
                        }
                        else
                        {
                            labelLineColor.BackColor = saveBitmap.GetPixel(e.X * sizeCoefWidth, e.Y * sizeCoefHeight);
                            labelSaturation.Text = (saveBitmap.GetPixel(e.X * sizeCoefWidth, e.Y * sizeCoefHeight).GetSaturation()).ToString();
                            labelBrightness.Text = (saveBitmap.GetPixel(e.X * sizeCoefWidth, e.Y * sizeCoefHeight).GetBrightness()).ToString();
                        }
                        break;
                }

                pictureBox.Image = currentBitmap;
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                saveBitmap.Save(saveFileDialog.FileName);
            }
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image = null;
                currentTools = tools.Empty;
                saveBitmap = (Bitmap)Image.FromFile(openFileDialog.FileName);
                pictureBox.Image = saveBitmap;
                brightImage = saveBitmap;
                contrastImage = saveBitmap;
                currentBitmap = saveBitmap;

                textBoxText.Visible = false;
                buttonTextStyle.Visible = false;
                SetGroupBoxLocation(openingGroupBoxColorLocation, openingTrackBarWidthLocation, openingLabelWidthLocation);

                sizeCoefWidth = saveBitmap.Width / pictureBox.Width;
                sizeCoefHeight = saveBitmap.Height / pictureBox.Height;

                EnableTrackBars(true);
                SetOriginalTrackBarsValues();
            }
        }

        private void SetCurrentColor(Color labelColor)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelColor;
            }
            else
            {
                labelLineColor.BackColor = labelColor;
            }
        }

        private void labelBlackColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelBlackColor.BackColor);
        }

        private void labelPurpleColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelPurpleColor.BackColor);
        }

        private void labelWhiteColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelWhiteColor.BackColor);
        }

        private void labelYellowColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelYellowColor.BackColor);
        }

        private void labelRedColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelRedColor.BackColor);
        }

        private void labelBlueColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelBlueColor.BackColor);
        }

        private void labelGreenColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelGreenColor.BackColor);
        }

        private void labelAquaColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelAquaColor.BackColor);
        }

        private void labelGreyColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelGreyColor.BackColor);
        }

        private void labelMaroonColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelMaroonColor.BackColor);
        }

        private void labelOrangeColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelOrangeColor.BackColor);
        }

        private void labelMagentaColor_Click(object sender, EventArgs e)
        {
            SetCurrentColor(labelMagentaColor.BackColor);
        }

        private void buttonPentagon_Click(object sender, EventArgs e)
        {
            currentTools = tools.Pentagon;
        }

        private void buttonPipette_Click(object sender, EventArgs e)
        {
            currentTools = tools.Pipette;
        }

        private void buttonTriangle_Click(object sender, EventArgs e)
        {
            currentTools = tools.Triangle;
        }

        private void buttonPalette_Click(object sender, EventArgs e)
        {
            currentTools = tools.Palette;

            labelLineColor.BackColor = Color.FromArgb(((labelLineColor.BackColor.R + labelFillColor.BackColor.R) / 2) % 256,
                                                      ((labelLineColor.BackColor.G + labelFillColor.BackColor.G) / 2) % 256,
                                                      ((labelLineColor.BackColor.B + labelFillColor.BackColor.B) / 2) % 256);
        }

        private void buttonTextStyle_Click(object sender, EventArgs e)
        {
            if (fontDialogText.ShowDialog() == DialogResult.OK)
            {
                textBoxText.Font = fontDialogText.Font;
            }
        }

        private void trackBarBrightness_Scroll(object sender, EventArgs e)
        {
            AdjustImage();
            contrastImage = saveBitmap;
        }

        private void AdjustImage()
        {
            saveBitmap = (Bitmap)AdjustBrightness(brightImage, (float)(trackBarBrightness.Value / 100.0)).Clone();
            pictureBox.Image = saveBitmap;
        }

        private void trackBarSaturation_ValueChanged(object sender, EventArgs e)
        {
            Bitmap tempBitmap = (Bitmap)pictureBox.Image;

            if (pictureBox.Image != null)
            {
                tempBitmap = Contrast((Bitmap)contrastImage, trackBarSaturation.Value);
                pictureBox.Image = tempBitmap;
                saveBitmap = (Bitmap)tempBitmap.Clone();
                brightImage = tempBitmap;
            }
        }

        private Bitmap Contrast(Bitmap sourceBitmap, int threshold)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                        sourceBitmap.Width, sourceBitmap.Height),
                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);

            double blue = 0;
            double green = 0;
            double red = 0;

            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = ((((pixelBuffer[k] / 255.0) - 0.5) *
                           contrastLevel) + 0.5) * 255.0;

                green = ((((pixelBuffer[k + 1] / 255.0) - 0.5) *
                            contrastLevel) + 0.5) * 255.0;

                red = ((((pixelBuffer[k + 2] / 255.0) - 0.5) *
                           contrastLevel) + 0.5) * 255.0;

                if (blue > 255)
                { blue = 255; }
                else if (blue < 0)
                { blue = 0; }

                if (green > 255)
                { green = 255; }
                else if (green < 0)
                { green = 0; }

                if (red > 255)
                { red = 255; }
                else if (red < 0)
                { red = 0; }

                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                        resultBitmap.Width, resultBitmap.Height),
                                        ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        private Bitmap AdjustBrightness(Image image, float brightness)
        {
            // Make the ColorMatrix.
            float b = brightness;
            ColorMatrix cm = new ColorMatrix(new float[][]
                {
                    new float[] {b, 0, 0, 0, 0},
                    new float[] {0, b, 0, 0, 0},
                    new float[] {0, 0, b, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1},
                });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(cm);

            // Draw the image onto the new bitmap while applying the new ColorMatrix.
            Point[] points =
            {
                new Point(0, 0),
                new Point(image.Width, 0),
                new Point(0, image.Height),
            };
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            // Make the result bitmap.
            Bitmap bm = new Bitmap(image.Width, image.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.DrawImage(image, points, rect, GraphicsUnit.Pixel, attributes);
            }

            // Return the result.
            return bm;
        }

        private void создатьНовыйФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox.Image = null;
            currentTools = tools.Empty;
            currentBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            graphics = Graphics.FromImage(currentBitmap);
            saveBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);

            sizeCoefWidth = 1;
            sizeCoefHeight = 1;
            SetOriginalTrackBarsValues();

            textBoxText.Visible = false;
            buttonTextStyle.Visible = false;
            SetGroupBoxLocation(openingGroupBoxColorLocation, openingTrackBarWidthLocation, openingLabelWidthLocation);

            EnableTrackBars(false);
        }

        private void SetOriginalTrackBarsValues()
        {
            trackBarBrightness.Value = 100;
            trackBarSaturation.Value = (trackBarSaturation.Minimum + trackBarSaturation.Maximum) / 2;
        }

        private void textBoxText_SizeChanged(object sender, EventArgs e)
        {
            SetUsingTextLocation();
            SetGroupBoxLocation(usingTextGroupBoxColorLocation, usingTextTrackBarWidthLocation, usingTextLabelWidthLocation);
        }

        private void checkBoxFill_CheckedChanged(object sender, EventArgs e)
        {
            isFillShape = checkBoxFill.Checked;
        }

        private void trackBarWight_Scroll(object sender, EventArgs e)
        {
            pen.Width = trackBarWight.Value;
            eraseSize = trackBarWight.Value;
        }
    }
}
