var okno = new Form();
okno.Text = "Moje okno";
okno.Size = new System.Drawing.Size(600, 400);
okno.StartPosition = FormStartPosition.CenterScreen;
okno.Visible = true;

okno.Paint += Okno_Paint;

void Okno_Paint(object? sender, PaintEventArgs e)
{
	e.Graphics.DrawLine(Pens.Black, 0f, 0f, 300f, 200f);
}

Application.Run(okno);