using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Endurance_Testing.UI
{
    public static class TextBoxExtensions
    {
        private static readonly Dictionary<TextBox, Panel> TextBoxPanelMap = new Dictionary<TextBox, Panel>();

        public static void MakeRounded(this TextBox textBox,
                                      Color? normalBorderColor = null,
                                      Color? focusedBorderColor = null,
                                      int borderWidth = 2,
                                      int shadowDepth = 3)
        {
            var normalColor = normalBorderColor ?? Color.FromArgb(180, 180, 180);
            var focusedColor = focusedBorderColor ?? Color.FromArgb(64, 158, 255);

            var parent = textBox.Parent;
            var location = textBox.Location;

            int margin = shadowDepth * 2;
            int fullHeight = textBox.Height;

            Panel containerPanel = new Panel
            {
                Location = new Point(location.X - margin - 1, location.Y - margin - 4),
                Size = new Size(textBox.Width + (margin * 2) + 2, fullHeight + (margin * 2)),
                Anchor = textBox.Anchor,
                BackColor = Color.Transparent,
                Tag = "RoundedTextBoxPanel"
            };

            int horizontalPadding = fullHeight / 2 + 2;

            textBox.Parent = containerPanel;
            textBox.Location = new Point(margin + 1 + horizontalPadding, margin + 4);
            textBox.Width = containerPanel.Width - (margin * 2) - 2 - (horizontalPadding * 2);
            textBox.Height = fullHeight - 8;
            textBox.BorderStyle = BorderStyle.None;

            if (TextBoxPanelMap.ContainsKey(textBox))
                TextBoxPanelMap[textBox] = containerPanel;
            else
                TextBoxPanelMap.Add(textBox, containerPanel);

            containerPanel.Paint += (sender, e) => {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle textBoxRect = new Rectangle(margin, margin,
                                                   containerPanel.Width - (margin * 2),
                                                   fullHeight);

                for (int i = shadowDepth + 2; i > 0; i--)
                {
                    float offset = i * 0.8f;
                    using (var path = GetPillShapePath(
                        textBoxRect.X - offset,
                        textBoxRect.Y - offset,
                        textBoxRect.Width + (offset * 2),
                        textBoxRect.Height + (offset * 2)))
                    {
                        int alpha = 14 - (int)(i * 1.2);
                        if (alpha < 2) alpha = 2;

                        using (var shadowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                        {
                            g.FillPath(shadowBrush, path);
                        }
                    }
                }

                Color backgroundColor = textBox.Enabled ? Color.White : SystemColors.Control;
                using (var path = GetPillShapePath(textBoxRect.X, textBoxRect.Y, textBoxRect.Width, textBoxRect.Height))
                using (var fillBrush = new SolidBrush(backgroundColor))
                {
                    g.FillPath(fillBrush, path);
                }

                Color borderColor;
                if (!textBox.Enabled)
                    borderColor = Color.FromArgb(180, 180, 180);
                else if (textBox.Focused)
                    borderColor = focusedColor;
                else
                    borderColor = normalColor;

                using (var path = GetPillShapePath(textBoxRect.X, textBoxRect.Y, textBoxRect.Width, textBoxRect.Height))
                {
                    using (var pen = new Pen(borderColor, borderWidth))
                    {
                        pen.Alignment = PenAlignment.Center;
                        pen.LineJoin = LineJoin.Round;
                        g.DrawPath(pen, path);
                    }
                }

                if (textBox.Enabled && textBox.Focused)
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        float glowOffset = i * 0.9f;
                        using (var path = GetPillShapePath(
                            textBoxRect.X - glowOffset,
                            textBoxRect.Y - glowOffset,
                            textBoxRect.Width + (glowOffset * 2),
                            textBoxRect.Height + (glowOffset * 2)))
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

            EventHandler enabledChangedHandler = null;
            enabledChangedHandler = (s, e) => {
                containerPanel.Invalidate();
            };

            textBox.EnabledChanged += enabledChangedHandler;

            // Event handlers untuk focus dan refresh panel
            textBox.Enter += (s, e) => containerPanel.Invalidate();
            textBox.Leave += (s, e) => containerPanel.Invalidate();

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