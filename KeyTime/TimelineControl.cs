using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Timeline
{
    public class TimelineControl : Panel
    {
        private const int TopMargin = 25;
        private const int ResizeMargin = 5;

        private bool isDragging;
        private bool isResizingLeft;
        private bool isResizingRight;
        private Point dragStart;
        private Panel? selectedClip;

        private bool isCreatingClip;
        private Point createStart;
        private Panel? previewClip;

        private const int MinClipWidth = 20;
        private int dragOffsetX;

        public TimelineControl()
        {
            AutoScroll = true;
            BackColor = Color.FromArgb(30, 30, 30);
            Dock = DockStyle.Fill;
            MouseDown += TimelineControl_MouseDown;
        }

        // Add a track
        public void AddTrack()
        {
            int trackIndex = Controls.OfType<Panel>().Count();

            Panel track = new()
            {
                Height = 60,
                Width = 2000,
                BackColor = (trackIndex % 2 == 0)
                    ? Color.FromArgb(45, 45, 45)
                    : Color.FromArgb(55, 55, 55),
                Top = TopMargin + trackIndex * 65,
                Left = 0,
                Tag = trackIndex
            };

            track.MouseDown += Track_MouseDown;
            track.MouseUp += Track_MouseUp_Delete;
            Controls.Add(track);
        }

        // Remove a specific track
        public void RemoveTrack(int trackIndex)
        {
            Panel? trackToRemove = Controls.OfType<Panel>()
                .FirstOrDefault(t => (int)t.Tag == trackIndex);
            if (trackToRemove == null) return;

            Controls.Remove(trackToRemove);

            // Re-index and recolor remaining tracks
            int i = 0;
            foreach (Panel t in Controls.OfType<Panel>().OrderBy(t => t.Top))
            {
                t.Tag = i;
                t.Top = TopMargin + i * 65;
                t.BackColor = (i % 2 == 0)
                    ? Color.FromArgb(45, 45, 45)
                    : Color.FromArgb(55, 55, 55);
                i++;
            }

            Invalidate();
        }

        // Sets up a new width
        public void SetTimelineWidth(int newWidth)
        {
            if (newWidth < 100) newWidth = 100; // safety minimum

            foreach (Panel track in Controls.OfType<Panel>())
            {
                track.Width = newWidth;

                // Clamp clip positions and sizes if they go beyond new width
                foreach (Panel clip in track.Controls.OfType<Panel>())
                {
                    if (clip.Left + clip.Width > newWidth)
                    {
                        // shrink clip if needed
                        clip.Width = Math.Max(10, newWidth - clip.Left);
                        if (clip.Tag is TimelineClip data)
                        {
                            data.EndTime = data.StartTime + clip.Width;
                        }
                    }
                }
            }

            Invalidate(); // Redraw time ruler
        }

        // Export data
        public List<TimelineData> GetTimelineData()
        {
            List<TimelineData> result = new();

            foreach (Panel track in Controls.OfType<Panel>())
            {
                TimelineData tData = new()
                {
                    TrackIndex = (int)track.Tag,
                    Clips = track.Controls.OfType<Panel>()
                        .Select(c => (TimelineClip)c.Tag)
                        .Where(c => c != null)!
                        .OrderBy(c => c!.StartTime)
                        .ThenBy(c => c!.EndTime)
                        .Select(c => new ClipData
                        {
                            Character = c.Character,
                            StartTime = c.StartTime,
                            EndTime = c.EndTime
                        })
                        .ToList()
                };
                result.Add(tData);
            }

            return result;
        }

        // Import data
        public void LoadTimelineData(List<TimelineData> tracks)
        {
            Controls.Clear();

            foreach (var trackData in tracks.OrderBy(t => t.TrackIndex))
            {
                Panel track = new()
                {
                    Height = 60,
                    Width = 2000,
                    BackColor = (trackData.TrackIndex % 2 == 0)
                        ? Color.FromArgb(45, 45, 45)
                        : Color.FromArgb(55, 55, 55),
                    Top = TopMargin + trackData.TrackIndex * 65,
                    Left = 0,
                    Tag = trackData.TrackIndex
                };

                track.MouseDown += Track_MouseDown;
                track.MouseUp += Track_MouseUp_Delete;
                Controls.Add(track);

                foreach (var clipData in trackData.Clips)
                {
                    TimelineClip data = new()
                    {
                        StartTime = clipData.StartTime,
                        EndTime = clipData.EndTime,
                        Character = clipData.Character
                    };
                    Panel clip = BuildClipPanel(data);
                    track.Controls.Add(clip);
                }
            }
        }

        // Click empty background, add track
        private void TimelineControl_MouseDown(object? sender, MouseEventArgs e)
        {
            bool clickedTrack = Controls.OfType<Panel>().Any(t => t.Bounds.Contains(e.Location));
            if (!clickedTrack) AddTrack();
        }

        // Start drawing a new clip
        private void Track_MouseDown(object? sender, MouseEventArgs e)
        {
            Panel track = (Panel)sender!;

            isCreatingClip = true;
            createStart = e.Location;

            previewClip = new Panel
            {
                Height = 40,
                BackColor = Color.FromArgb(90, 140, 180),
                BorderStyle = BorderStyle.FixedSingle,
                Top = 10,
                Left = e.X,
                Width = 1
            };

            track.Controls.Add(previewClip);
            track.MouseMove += Track_MouseMove_Create;
            track.MouseUp += Track_MouseUp_Create;
        }

        // Live resize of preview
        private void Track_MouseMove_Create(object? sender, MouseEventArgs e)
        {
            if (!isCreatingClip || previewClip == null) return;
            Panel track = (Panel)sender!;

            int startX = Math.Min(createStart.X, e.X);
            int endX = Math.Max(createStart.X, e.X);
            int width = Math.Max(1, endX - startX);

            startX = Math.Max(0, startX);
            endX = Math.Min(track.Width, endX);

            previewClip.Left = startX;
            previewClip.Width = width;
        }

        // Release mouse, commit or cancel clip
        private void Track_MouseUp_Create(object? sender, MouseEventArgs e)
        {
            if (!isCreatingClip) return;
            Panel track = (Panel)sender!;

            int startX = Math.Min(createStart.X, e.X);
            int endX = Math.Max(createStart.X, e.X);
            int width = Math.Max(10, endX - startX);

            bool overlap = track.Controls.OfType<Panel>()
                .Where(c => c != previewClip)
                .Any(c => startX < c.Left + c.Width && endX > c.Left);

            track.MouseMove -= Track_MouseMove_Create;
            track.MouseUp -= Track_MouseUp_Create;

            if (!overlap)
            {
                TimelineClip data = new()
                {
                    StartTime = startX,
                    EndTime = startX + width,
                    Character = "A"
                };

                Panel clip = BuildClipPanel(data);
                clip.Left = startX;
                clip.Width = width;

                track.Controls.Remove(previewClip);
                track.Controls.Add(clip);
            }
            else
            {
                track.Controls.Remove(previewClip);
            }

            previewClip = null;
            isCreatingClip = false;
        }

        // Right-click track, delete it
        private void Track_MouseUp_Delete(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || sender is not Panel track) return;

            int index = (int)track.Tag;
            if (MessageBox.Show($"Remove Track {index}?", "Confirm",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                RemoveTrack(index);
            }
        }

        // Build new clip panel
        private Panel BuildClipPanel(TimelineClip data)
        {
            Panel clip = new()
            {
                Width = data.Duration,
                Height = 40,
                BackColor = Color.FromArgb(70, 110, 150),
                BorderStyle = BorderStyle.FixedSingle,
                Left = Math.Max(0, data.StartTime),
                Top = 10,
                Tag = data
            };

            Label label = new()
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Enabled = false
            };
            clip.Controls.Add(label);

            clip.MouseDown += Clip_MouseDown;
            clip.MouseMove += Clip_MouseMove;
            clip.MouseUp += Clip_MouseUp;
            clip.DoubleClick += Clip_DoubleClick;
            clip.MouseUp += Clip_MouseUp_Delete;

            UpdateClipText(clip);
            return clip;
        }

        // Begin drag / resize
        private void Clip_MouseDown(object? sender, MouseEventArgs e)
        {
            selectedClip = (Panel)sender!;
            dragStart = e.Location;
            dragOffsetX = e.X;

            isDragging = false;
            isResizingLeft = false;
            isResizingRight = false;

            if (e.X <= ResizeMargin)
                isResizingLeft = true;
            else if (e.X >= selectedClip.Width - ResizeMargin)
                isResizingRight = true;
            else
                isDragging = true;

            selectedClip.BringToFront();
        }

        // Drag / resize / cursor feedback
        private void Clip_MouseMove(object? sender, MouseEventArgs e)
        {
            var clip = (Panel)sender!;

            // cursor feedback when idle
            if (!isDragging && !isResizingLeft && !isResizingRight)
            {
                clip.Cursor = (e.X <= ResizeMargin || e.X >= clip.Width - ResizeMargin)
                    ? Cursors.SizeWE
                    : Cursors.Default;
                return;
            }

            if (selectedClip == null) return;
            var track = (Panel)selectedClip.Parent!;
            if (selectedClip.Tag is not TimelineClip data) return;

            // Mouse X in TRACK coordinates (stable reference frame)
            int mouseXInTrack = track.PointToClient(Cursor.Position).X;

            if (isDragging)
            {
                // keep the mouse anchored at the same offset inside the clip
                int newLeft = mouseXInTrack - dragOffsetX;

                // clamp to track
                newLeft = Math.Clamp(newLeft, 0, track.Width - selectedClip.Width);

                // test overlap with others at the new rect
                var newRect = new Rectangle(newLeft, selectedClip.Top, selectedClip.Width, selectedClip.Height);
                if (IntersectsAny(track, newRect, selectedClip)) return;

                selectedClip.Left = newLeft;
                data.StartTime = newLeft;
                data.EndTime = newLeft + selectedClip.Width;
            }
            else if (isResizingLeft)
            {
                // left edge follows mouse, but keep ≥ MinClipWidth
                int maxLeft = selectedClip.Right - MinClipWidth;
                int newLeft = Math.Clamp(mouseXInTrack, 0, maxLeft);
                int newWidth = selectedClip.Right - newLeft;

                var newRect = new Rectangle(newLeft, selectedClip.Top, newWidth, selectedClip.Height);
                if (IntersectsAny(track, newRect, selectedClip)) return;

                selectedClip.Left = newLeft;
                selectedClip.Width = newWidth;
                data.StartTime = newLeft;
                data.EndTime = newLeft + newWidth;
            }
            else if (isResizingRight)
            {
                // right edge follows mouse, but keep ≥ MinClipWidth
                int minRight = selectedClip.Left + MinClipWidth;
                int newRight = Math.Clamp(mouseXInTrack, minRight, track.Width);
                int newWidth = newRight - selectedClip.Left;

                var newRect = new Rectangle(selectedClip.Left, selectedClip.Top, newWidth, selectedClip.Height);
                if (IntersectsAny(track, newRect, selectedClip)) return;

                selectedClip.Width = newWidth;
                data.EndTime = data.StartTime + newWidth;
            }

            UpdateClipText(selectedClip);
        }

        // End drag/resize
        private void Clip_MouseUp(object? sender, MouseEventArgs e)
        {
            isDragging = false;
            isResizingLeft = false;
            isResizingRight = false;
            selectedClip = null;
            ((Panel)sender!).Cursor = Cursors.Default;
        }

        // Double-click, edit clip
        private void Clip_DoubleClick(object? sender, EventArgs e)
        {
            if (sender is not Panel clip || clip.Tag is not TimelineClip data) return;

            using Form editor = new()
            {
                Text = "Edit Clip",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                ClientSize = new Size(220, 180)
            };

            Label lblChar = new() { Text = "Character:", Left = 10, Top = 20, Width = 70 };
            TextBox txtChar = new() { Left = 90, Top = 18, Width = 100, Text = data.Character };
            Label lblStart = new() { Text = "Start (ms):", Left = 10, Top = 55, Width = 70 };
            NumericUpDown numStart = new() { Left = 90, Top = 50, Width = 100, Minimum = 0, Maximum = 5000, Value = data.StartTime };
            Label lblEnd = new() { Text = "End (ms):", Left = 10, Top = 90, Width = 70 };
            NumericUpDown numEnd = new() { Left = 90, Top = 85, Width = 100, Minimum = 0, Maximum = 5000, Value = data.EndTime };

            Button ok = new() { Text = "OK", Left = 40, Width = 60, Top = 130, DialogResult = DialogResult.OK };
            Button cancel = new() { Text = "Cancel", Left = 120, Width = 60, Top = 130, DialogResult = DialogResult.Cancel };

            editor.Controls.AddRange(new Control[] { lblChar, txtChar, lblStart, numStart, lblEnd, numEnd, ok, cancel });
            editor.AcceptButton = ok;
            editor.CancelButton = cancel;

            if (editor.ShowDialog() == DialogResult.OK)
            {
                int newStart = (int)numStart.Value;
                int newEnd = (int)numEnd.Value;

                if (newEnd <= newStart)
                {
                    MessageBox.Show("End must be greater than Start.");
                    return;
                }

                data.Character = txtChar.Text.Trim();
                data.StartTime = newStart;
                data.EndTime = newEnd;

                clip.Left = newStart;
                clip.Width = data.Duration;
                UpdateClipText(clip);
            }
        }

        // Right-click clip, delete it
        private void Clip_MouseUp_Delete(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || sender is not Panel clip) return;
            Panel track = (Panel)clip.Parent!;
            if (MessageBox.Show("Delete this clip?", "Confirm",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                track.Controls.Remove(clip);
            }
        }

        private static bool IntersectsAny(Panel track, Rectangle rect, Panel? except = null)
        {
            foreach (var c in track.Controls.OfType<Panel>())
            {
                if (c == except) continue;
                var r = new Rectangle(c.Left, c.Top, c.Width, c.Height);
                if (r.IntersectsWith(rect)) return true;
            }
            return false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawTimeRuler(e.Graphics);
        }

        private void DrawTimeRuler(Graphics g)
        {
            int width = Math.Max(ClientSize.Width, 2000);
            using Pen pen = new(Color.LightGray);
            using Font font = new("Segoe UI", 7);
            using Brush brush = new SolidBrush(Color.White);

            for (int x = 0; x < width; x += 50)
            {
                int tickHeight = (x % 500 == 0) ? 10 : 5;
                g.DrawLine(pen, x - HorizontalScroll.Value, 0, x - HorizontalScroll.Value, tickHeight);

                if (x % 500 == 0)
                {
                    double sec = x / 1000.0;
                    g.DrawString($"{sec:F1}s", font, brush, x - HorizontalScroll.Value + 2, tickHeight + 2);
                }
            }
        }

        private void UpdateClipText(Panel clip)
        {
            if (clip.Tag is not TimelineClip d) return;
            string labelText = $"{d.Character}: {d.StartTime / 1000.0:F3}s → {d.EndTime / 1000.0:F3}s";
            if (clip.Controls.OfType<Label>().FirstOrDefault() is Label lbl)
                lbl.Text = labelText;
        }

        private class TimelineClip
        {
            public int StartTime { get; set; }
            public int EndTime { get; set; }
            public string Character { get; set; } = "A";
            public int Duration => EndTime - StartTime;
        }

        public class ClipData
        {
            public string Character { get; set; } = "A";
            public int StartTime { get; set; }
            public int EndTime { get; set; }
        }

        public class TimelineData
        {
            public int TrackIndex { get; set; }
            public List<ClipData> Clips { get; set; } = new();
        }
    }
}
