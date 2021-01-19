using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        Graphics graphics;
        Bitmap currentBitmap;
        Bitmap saveBitmap;
        Pen pen;
        SolidBrush brush;
        SolidBrush erase;

        enum tools { Empty, Line, Rectangle, Ellipse, Erase, Text };

        tools currentTools;
        private void labelLineColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pen.Color = colorDialog.Color;
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
            pen = new Pen(colorDialog.Color, 1);
            brush = new SolidBrush(colorDialog.Color);
            erase = new SolidBrush(Color.White);

            labelLineColor.BackColor = colorDialog.Color;
            labelFillColor.BackColor = colorDialog.Color;

            openFileDialog.Filter = "img(*.jpg; .png)|.jpg; *.png";
            saveFileDialog.Filter = "img(*.jpg; .png)|.jpg; *.png";
        }

        private void buttonText_Click(object sender, EventArgs e)
        {
            currentTools = tools.Text;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            canDraw = true;
            xStart = e.X;
            yStart = e.Y;
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            canDraw = false;
            saveBitmap = (Bitmap)currentBitmap.Clone();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (canDraw == true)
            {
                xEnd = e.X;
                yEnd = e.Y;

                currentBitmap = (Bitmap)saveBitmap.Clone();
                graphics = Graphics.FromImage(currentBitmap);

                switch (currentTools)
                {
                    case tools.Line:
                        graphics = Graphics.FromImage(currentBitmap);

                        graphics.DrawLine(pen, xStart, yStart, xEnd, yEnd);

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
                    case tools.Erase:
                        graphics.FillRectangle(erase, e.X - eraseSize / 2, e.Y - eraseSize / 2, eraseSize, eraseSize);
                        saveBitmap = (Bitmap)currentBitmap.Clone();
                        break;
                    case tools.Text:
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
                saveBitmap = (Bitmap)Image.FromFile(openFileDialog.FileName);
                pictureBox.Image = saveBitmap;
            }
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
