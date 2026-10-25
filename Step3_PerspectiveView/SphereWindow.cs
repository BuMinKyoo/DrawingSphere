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
        Text = "구 세 개 - 원근투영";
        ClientSize = new Size(860, 480);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        canvas.Dock = DockStyle.Fill;
        canvas.BackColor = System.Drawing.Color.Black;
        canvas.SizeMode = PictureBoxSizeMode.Zoom;
        panel.Dock = DockStyle.Right;
        panel.Width = 230;
        panel.Padding = new Padding(12);
        panel.FlowDirection = FlowDirection.TopDown;
        panel.WrapContents = false;
        panel.AutoScroll = true;
        Controls.Add(canvas);
        Controls.Add(panel);

        CheckBox perspective = new CheckBox
        {
            Text = "원근 투영",
            Checked = raytracer.UsePerspective,
            AutoSize = true
        };
        perspective.CheckedChanged += (_, _) =>
        {
            raytracer.UsePerspective = perspective.Checked;
            UpdateImage();
        };
        panel.Controls.Add(perspective);

        panel.Controls.Add(new Label
        {
            Text = "구 세 개의 반지름은 모두 0.4\nZ를 바꾸며 크기를 비교하세요.",
            AutoSize = true,
            Margin = new Padding(3, 12, 3, 12)
        });

        Sphere blue = raytracer.Spheres[0];
        Sphere green = raytracer.Spheres[1];
        Sphere red = raytracer.Spheres[2];
        AddSlider("빨간 구 Z", red.Center.Z, 0.45f, 3, v => red.Center.Z = v);
        AddSlider("초록 구 Z", green.Center.Z, 0.45f, 3, v => green.Center.Z = v);
        AddSlider("파란 구 Z", blue.Center.Z, 0.45f, 3, v => blue.Center.Z = v);

        panel.Controls.Add(new Label
        {
            Text = "Phong 조명",
            AutoSize = true,
            Margin = new Padding(3, 16, 3, 3)
        });
        AddSlider("광원 X", raytracer.Light.Position.X, -2, 2, v => raytracer.Light.Position.X = v);
        AddSlider("광원 Y", raytracer.Light.Position.Y, -2, 2, v => raytracer.Light.Position.Y = v);
        AddSlider("광원 Z", raytracer.Light.Position.Z, -2, 2, v => raytracer.Light.Position.Z = v);

        panel.Controls.Add(new Label
        {
            Text = "체크 해제: 정투영\n체크: 원근투영",
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
