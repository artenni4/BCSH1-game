using System.Windows.Forms;

using var game = new MyGame.MyGame();
var form = (Form)Control.FromHandle(game.Window.Handle);
form.WindowState = FormWindowState.Maximized;
game.Run();
