using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Endurance_Testing.UI
{
    public static class ComboBoxExtensions
    {
        private static readonly Dictionary<ComboBox, Panel> ComboBoxPanelMap = new Dictionary<ComboBox, Panel>();

        public static void MakeRounded(this ComboBox comboBox,
                                     Color? normalBorderColor = null,
                                     Color? focusedBorderColor = null,
                                     int borderWidth = 2,
                                     int shadowDepth = 3)
        {
            var normalColor = normalBorderColor ?? Color.FromArgb(180, 180, 180);
            var focusedColor = focusedBorderColor ?? Color.FromArgb(64, 158, 255);

            var parent = comboBox.Parent;
            var location = comboBox.Location;

            int margin = shadowDepth * 2;
            int fullHeight = comboBox.Height;

            int horizontalPadding = 20;
            int verticalPadding = 5;

            Panel containerPanel = new Panel
            {
                Location = new Point(location.X - margin - horizontalPadding, location.Y - margin - verticalPadding),
                Size = new Size(comboBox.Width + (margin * 2) + (horizontalPadding * 2),
                               fullHeight + (margin * 2) + (verticalPadding * 2)),
                Anchor = comboBox.Anchor,
                BackColor = Color.Transparent
            };

            comboBox.Parent = containerPanel;

            comboBox.Location = new Point(margin + horizontalPadding, margin + verticalPadding);
            comboBox.Width = containerPanel.Width - (margin * 2) - (horizontalPadding * 2);
            comboBox.FlatStyle = FlatStyle.Flat;

            if (ComboBoxPanelMap.ContainsKey(comboBox))
                ComboBoxPanelMap[comboBox] = containerPanel;
            else
                ComboBoxPanelMap.Add(comboBox, containerPanel);
            
            comboBox.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FlatStyle = FlatStyle.Flat;

            comboBox.DrawItem += (sender, e) =>
            {
                if (e.Index < 0)
                    return;

                e.DrawBackground();

                string text = comboBox.GetItemText(comboBox.Items[e.Index]);

                using (var brush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(text, e.Font, brush, e.Bounds.X + 15, e.Bounds.Y + 5);
                }

                e.DrawFocusRectangle();
            };

            containerPanel.Paint += (sender, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle comboBoxRect = new Rectangle(
                    margin,
                    margin,
                    containerPanel.Width - (margin * 2),
                    containerPanel.Height - (margin * 2));

                for (int i = shadowDepth + 2; i > 0; i--)
                {
                    float offset = i * 0.8f;
                    using (var path = GetPillShapePath(
                        comboBoxRect.X - offset,
                        comboBoxRect.Y - offset,
                        comboBoxRect.Width + (offset * 2),
                        comboBoxRect.Height + (offset * 2)))
                    {
                        int alpha = 14 - (int)(i * 1.2);
                        if (alpha < 2) alpha = 2;

                        using (var shadowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                        {
                            g.FillPath(shadowBrush, path);
                        }
                    }
                }

                Color backgroundColor = comboBox.Enabled ? Color.White : SystemColors.Control;
                using (var path = GetPillShapePath(comboBoxRect.X, comboBoxRect.Y, comboBoxRect.Width, comboBoxRect.Height))
                using (var fillBrush = new SolidBrush(backgroundColor))
                {
                    g.FillPath(fillBrush, path);
                }

                Color borderColor;
                if (!comboBox.Enabled)
                    borderColor = Color.FromArgb(180, 180, 180);
                else if (comboBox.DroppedDown)
                    borderColor = focusedColor;
                else
                    borderColor = normalColor;

                using (var path = GetPillShapePath(comboBoxRect.X, comboBoxRect.Y, comboBoxRect.Width, comboBoxRect.Height))
                {
                    using (var pen = new Pen(borderColor, borderWidth))
                    {
                        pen.Alignment = PenAlignment.Center;
                        pen.LineJoin = LineJoin.Round;
                        g.DrawPath(pen, path);
                    }
                }

                int arrowSize = 8;
                int arrowX = comboBoxRect.Right - arrowSize - 25;
                int arrowY = comboBoxRect.Top + (comboBoxRect.Height - arrowSize) / 2;

                using (var arrowBrush = new SolidBrush(comboBox.Enabled ? Color.FromArgb(100, 100, 100) : Color.FromArgb(160, 160, 160)))
                {
                    Point[] arrowPoints = new Point[]
                    {
                        new Point(arrowX, arrowY),
                        new Point(arrowX + arrowSize, arrowY),
                        new Point(arrowX + arrowSize / 2, arrowY + arrowSize)
                    };
                    g.FillPolygon(arrowBrush, arrowPoints);
                }

                if (comboBox.Enabled && comboBox.DroppedDown)
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        float glowOffset = i * 0.9f;
                        using (var path = GetPillShapePath(
                            comboBoxRect.X - glowOffset,
                            comboBoxRect.Y - glowOffset,
                            comboBoxRect.Width + (glowOffset * 2),
                            comboBoxRect.Height + (glowOffset * 2)))
                        {
                            int alpha = 14 - (int)(i * 2.5);
                            if (alpha < 2) alpha = 2;

                            using (var glowPen = new Pen(Color.FromArgb(alpha, focusedColor), 1))
                            {
                                glowPen.LineJoin = LineJoin.Round;
                                g.DrawPath(glowPen, path);
                            }
                        }
                    }
                }
            };

            EventHandler refreshHandler = (s, e) => containerPanel.Invalidate();
            comboBox.DropDownClosed += refreshHandler;
            comboBox.DropDown += refreshHandler;
            comboBox.EnabledChanged += refreshHandler;
            comboBox.SelectedIndexChanged += refreshHandler;

            parent.Controls.Add(containerPanel);
        }

        private static GraphicsPath GetPillShapePath(float x, float y, float width, float height)
        {
            GraphicsPath path = new GraphicsPath();

            if (width <= 0 || height <= 0)
                return path;

            float diameter = height;
            float radius = diameter / 2;

            if (width < height)
            {
                path.AddEllipse(x, y, width, height);
            }
            else
            {
                path.AddArc(x, y, diameter, diameter, 180, 90);
                path.AddArc(x + width - diameter, y, diameter, diameter, 270, 90);
                path.AddArc(x + width - diameter, y + height - diameter, diameter, diameter, 0, 90);
                path.AddArc(x, y + height - diameter, diameter, diameter, 90, 90);
            }

            path.CloseAllFigures();
            return path;
        }
    }
}