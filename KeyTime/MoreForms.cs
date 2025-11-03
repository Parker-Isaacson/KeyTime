using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyTime
{
    public class InputDialog : Form
    {
        private Label labelPrompt;
        private TextBox textBoxInput;
        private Button buttonOk;
        private Button buttonCancel;

        public string UserInput { get; private set; }

        public InputDialog(string prompt, string title = "Input Required")
        {
            // Basic form setup
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(300, 150);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;

            // Label
            labelPrompt = new Label()
            {
                Text = prompt,
                AutoSize = false,
                Location = new Point(10, 10),
                Size = new Size(280, 30)
            };
            this.Controls.Add(labelPrompt);

            // TextBox
            textBoxInput = new TextBox()
            {
                Location = new Point(10, 45),
                Size = new Size(280, 23)
            };
            this.Controls.Add(textBoxInput);

            // OK Button
            buttonOk = new Button()
            {
                Text = "OK",
                Location = new Point(130, 90),
                DialogResult = DialogResult.OK
            };
            buttonOk.Click += (sender, e) =>
            {
                UserInput = textBoxInput.Text;
                this.Close();
            };
            this.Controls.Add(buttonOk);

            // Cancel Button
            buttonCancel = new Button()
            {
                Text = "Cancel",
                Location = new Point(215, 90),
                DialogResult = DialogResult.Cancel
            };
            buttonCancel.Click += (sender, e) => this.Close();
            this.Controls.Add(buttonCancel);

            // Make Enter/ESC work automatically
            this.AcceptButton = buttonOk;
            this.CancelButton = buttonCancel;
        }

        public static string Show(string prompt, string title = "Input Required")
        {
            using (var dialog = new InputDialog(prompt, title))
            {
                return dialog.ShowDialog() == DialogResult.OK ? dialog.UserInput : null;
            }
        }
    }
}
