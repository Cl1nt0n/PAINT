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
        int sizeCoefWidth;
        int sizeCoefHeight;

        Graphics graphics;
        Bitmap currentBitmap;
        Bitmap saveBitmap;
        Pen pen;
        SolidBrush brush;
        SolidBrush erase;
        SolidBrush pencil;

        Point[] pointsTriangle = new Point[3];

        enum tools { Empty, Line, Rectangle, Ellipse, Erase, Text, Pencil, Pipette, Triangle, Palette };

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

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            pen.Width = trackBarWight.Value;
        }

        private void paint_Load(object sender, EventArgs e)
        {
            currentTools = tools.Empty;

            isFillShape = false;
            canDraw = false;
            eraseSize = 1;

            currentTools = tools.Empty;
            currentBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            graphics = Graphics.FromImage(currentBitmap);
            saveBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            pen = new Pen(labelLineColor.BackColor, 1);
            brush = new SolidBrush(labelFillColor.BackColor);
            erase = new SolidBrush(Color.White);
            pencil = new SolidBrush(labelLineColor.BackColor);

            labelLineColor.BackColor = colorDialog.Color;
            labelFillColor.BackColor = colorDialog.Color;

            openFileDialog.Filter = "img(*.jpg; .png)|.jpg; *.png";
            saveFileDialog.Filter = "img(*.jpg; .png)|.jpg; *.png";

            sizeCoefWidth = 1;
            sizeCoefHeight = 1;

            pointsTriangle[0].X = 125; pointsTriangle[0].Y = 100;
            pointsTriangle[1].X = 200; pointsTriangle[1].Y = 200;
            pointsTriangle[2].X = 50; pointsTriangle[2].Y = 200;
        }

        private void buttonText_Click(object sender, EventArgs e)
        {
            currentTools = tools.Text;
            textBoxText.Visible = true;
        }
        private void buttonPen_Click(object sender, EventArgs e)
        {
            currentTools = tools.Pencil;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            canDraw = true;
            xStart = e.X;
            yStart = e.Y;
            pen = new Pen(labelLineColor.BackColor, trackBarWight.Value);
            brush = new SolidBrush(labelFillColor.BackColor);
            pencil = new SolidBrush(labelLineColor.BackColor);
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            canDraw = false;
            textBoxText.Visible = false;

            saveBitmap = (Bitmap)currentBitmap.Clone();
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
                        pointsTriangle[0].X = xStart; pointsTriangle[0].Y = yStart;
                        pointsTriangle[1].X = xEnd; pointsTriangle[1].Y = yEnd;
                        pointsTriangle[2].X = xStart - (xEnd - xStart); pointsTriangle[2].Y = yEnd;
                        graphics.DrawPolygon(pen, pointsTriangle);
                        if (isFillShape == true)
                        {
                            graphics.FillPolygon(brush, pointsTriangle);
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
                        graphics.DrawString(textBoxText.Text, textBoxText.Font, pencil, e.X, e.Y);
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
                        labelLineColor.BackColor = saveBitmap.GetPixel(e.X * sizeCoefWidth, e.Y * sizeCoefHeight);
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
                //pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                saveBitmap = (Bitmap)Image.FromFile(openFileDialog.FileName);
                pictureBox.Image = saveBitmap;
                MessageBox.Show($"PB {pictureBox.Width};{pictureBox.Height}\nBM {saveBitmap.Width}; {saveBitmap.Height}");


                sizeCoefWidth = saveBitmap.Width / pictureBox.Width;
                sizeCoefHeight = saveBitmap.Height / pictureBox.Height;
            }
        }

        private void labelBlackColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelBlackColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelBlackColor.BackColor;
            }
        }

        private void labelPurpleColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelPurpleColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelPurpleColor.BackColor;
            }
        }

        private void labelWhiteColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelWhiteColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelWhiteColor.BackColor;
            }
        }

        private void labelYellowColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelYellowColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelYellowColor.BackColor;
            }
        }

        private void labelRedColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelRedColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelRedColor.BackColor;
            }
        }

        private void labelBlueColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelBlueColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelBlueColor.BackColor;
            }
        }

        private void labelGreenColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelGreenColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelGreenColor.BackColor;
            }
        }

        private void labelAquaColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelAquaColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelAquaColor.BackColor;
            }
        }

        private void labelGreyColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelAquaColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelAquaColor.BackColor;
            }
        }

        private void labelMaroonColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelMaroonColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelMaroonColor.BackColor;
            }
        }

        private void labelOrangeColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelOrangeColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelOrangeColor.BackColor;
            }
        }

        private void labelMagentaColor_Click(object sender, EventArgs e)
        {
            if (isFillShape == true)
            {
                labelFillColor.BackColor = labelMagentaColor.BackColor;
            }
            else
            {
                labelLineColor.BackColor = labelMagentaColor.BackColor;
            }
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
        }


        private void checkBoxFill_CheckedChanged(object sender, EventArgs e)
        {
            isFillShape = checkBoxFill.Checked;
        }

        private void trackBarWight_Scroll(object sender, EventArgs e)
        {
            eraseSize = trackBarWight.Value;
        }
    }
}
