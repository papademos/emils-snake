using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

//
Bitmap bitmap = new Bitmap(160, 120, PixelFormat.Format32bppArgb);
Queue<Keys> keys = new();
GameForm form = new GameForm(bitmap, keys);
Graphics graphics = Graphics.FromImage(bitmap);
Font font = new Font("Courier New", 8);

//
restart:
graphics.Clear(Color.Black);
keys.Clear();

//
int xHead = 60;
int yHead = 60;
int dx = 1;
int dy = 0;
int growth = 20;
int appleCount = 0;
int score = 0;
bool alive = true;
bool gameOver = false;
long tick = 0;
Queue<(int x, int y)> snake = new();
Random random = new(0);
Rectangle stageBounds = new(2, 2, 116, 116);

//
graphics.DrawRectangle(Pens.White, stageBounds);
void CreateRandomApple() {
    while (true) {
        int x = stageBounds.X + random.Next(stageBounds.Width);
        int y = stageBounds.Y + random.Next(stageBounds.Height);
        if (bitmap.GetPixel(x, y).ToArgb() == Color.Black.ToArgb()) {
            bitmap.SetPixel(x, y, Color.LightGreen);
            ++appleCount;
            return;
        }
    }
}
for (int i = 0; i < 10; i++) {
    CreateRandomApple();
}

//
form.Show();
while (gameOver == false) {
    if (appleCount < 10 && random.Next(0, 10*60) == 0) {
        CreateRandomApple();
    }

    if (alive) {
        // Update direction.
        keys.TryDequeue(out Keys key);
        if (key == Keys.Left) {
            (dx, dy) = (dy, -dx);
        }
        else if (key == Keys.Right) {
            (dx, dy) = (-dy, dx);
        };

        // Update position.
        xHead += dx;
        yHead += dy;

        // Check for collision.
        var c = bitmap.GetPixel(xHead, yHead).ToArgb();
        if (c == Color.Black.ToArgb()) {
            // Do nothing.
        } else if (c == Color.LightGreen.ToArgb()) {
            growth += 20;
            score += 100;
            --appleCount;
        } else {
            alive = false;
        }
    }

    if (alive) {
        // Draw head.
        bitmap.SetPixel(xHead, yHead, Color.Orange);

        // Add head position to the snakes body.
        snake.Enqueue((xHead, yHead));
    }

    if (growth > 0) {
        growth--;
    } else {
        // Remove tail from the snakes body.
        var (xTail, yTail) = snake.Dequeue();

        // Draw a black pixel where the tail used to be.
        bitmap.SetPixel(xTail, yTail, Color.Black);
    }

    // When the snake has no body, the game is over.
    if (snake.Count == 0) {
        gameOver = true;
    }

    // Increase score every 20th tick.
    if (tick % 20 == 0) {
        score++;
    }

    // Print score.
    graphics.FillRectangle(Brushes.Black, Rectangle.FromLTRB(stageBounds.Right+1, stageBounds.Top, 160, 40));
    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
    graphics.DrawString($"{score, 5}", font, Brushes.White, stageBounds.Right + 2, stageBounds.Top);

    //
    form.UpdateAndWait();
    if (form.IsDisposed) {
        gameOver = true;
    }
    tick++;
}

//
if (!form.IsDisposed) {
    if (MessageBox.Show("Play again?", "Game Over!", MessageBoxButtons.YesNo) == DialogResult.Yes) {
        goto restart;
    }
}