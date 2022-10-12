using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

class GameForm : Form
{
    public readonly Bitmap Bitmap;
    public readonly Queue<Keys> Keys;

    public GameForm(Bitmap bitmap, Queue<Keys> keys) {
        Bitmap = bitmap;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        ClientSize = new Size(4 * bitmap.Width, 4 * bitmap.Height);
        Keys = keys;
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
    }

    protected override void OnKeyDown(KeyEventArgs args) {
        Keys.Enqueue(args.KeyCode);
        base.OnKeyDown(args);
    }

    protected override void OnPaint(PaintEventArgs args) {
        var g = args.Graphics;
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.DrawImage(Bitmap, 0, 0, ClientSize.Width, ClientSize.Height);
        base.OnPaint(args);
    }

    public void UpdateAndWait() {
        Application.DoEvents();
        Refresh();
        Thread.Sleep(10);
    }
}
