using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

// 여기부터는 구의 값을 바꾸고 그림을 보여주는 간단한 창입니다.
class SphereWindow : Form
{
    readonly Raytracer raytracer = new Raytracer();
    readonly PictureBox canvas = new PictureBox();
    readonly FlowLayoutPanel panel = new FlowLayoutPanel();

    public SphereWindow()
    {
        Text = "구 하나 그리기";
        ClientSize = new Size(860, 480);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        canvas.Dock = DockStyle.Fill;
        canvas.BackColor = System.Drawing.Color.Black;
        canvas.SizeMode = PictureBoxSizeMode.Zoom;
        panel.Dock = DockStyle.Right;
        panel.Width = 210;
        panel.Padding = new Padding(12);
        panel.FlowDirection = FlowDirection.TopDown;
        panel.WrapContents = false;
        Controls.Add(canvas);
        Controls.Add(panel);

        AddSlider("중심 X", 0, -1, 1, v => raytracer.Sphere.Center.X = v);
        AddSlider("중심 Y", 0, -1, 1, v => raytracer.Sphere.Center.Y = v);
        AddSlider("중심 Z", 0.5f, -1, 1, v => raytracer.Sphere.Center.Z = v);
        AddSlider("반지름", 0.4f, 0.01f, 1, v => raytracer.Sphere.Radius = v);

        Button colorButton = new Button { Text = "색 바꾸기", Width = 170, Height = 32 };
        colorButton.Click += (_, _) =>
        {
            Vector3 rgb = raytracer.Sphere.Color;
            using ColorDialog dialog = new ColorDialog
            {
                Color = System.Drawing.Color.FromArgb(
                    (int)(rgb.X * 255), (int)(rgb.Y * 255), (int)(rgb.Z * 255))
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                raytracer.Sphere.Color = new Vector3(
                    dialog.Color.R / 255f, dialog.Color.G / 255f, dialog.Color.B / 255f);
                UpdateImage();
            }
        };
        panel.Controls.Add(colorButton);
        panel.Controls.Add(new Label
        {
            Text = "화면 좌표 → 광선 생성\n→ 구와 교차 계산 → 색 표시",
            AutoSize = true,
            Margin = new Padding(3, 16, 3, 3)
        });
        UpdateImage();
    }

    void AddSlider(string title, float value, float min, float max, Action<float> changed)
    {
        Label label = new Label { Text = $"{title}: {value:F2}", AutoSize = true };
        panel.Controls.Add(label);

        // TrackBar는 정수만 사용하므로 100배로 설정하고, 읽을 때 100으로 나눕니다.
        TrackBar slider = new TrackBar
        {
            Width = 170,
            Height = 32,
            AutoSize = false,
            Minimum = (int)MathF.Round(min * 100f),
            Maximum = (int)MathF.Round(max * 100f),
            Value = (int)MathF.Round(value * 100f),
            SmallChange = 1,
            LargeChange = 5,
            TickStyle = TickStyle.None,
            Margin = new Padding(3, 0, 3, 12)
        };
        slider.ValueChanged += (_, _) =>
        {
            float currentValue = slider.Value / 100f;
            label.Text = $"{title}: {currentValue:F2}";
            changed(currentValue);
            UpdateImage();
        };
        panel.Controls.Add(slider);
    }

    void UpdateImage()
    {
        Image? previous = canvas.Image;
        canvas.Image = raytracer.Render(640, 320);
        previous?.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            canvas.Image?.Dispose();
        base.Dispose(disposing);
    }
}
