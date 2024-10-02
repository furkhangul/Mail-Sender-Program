using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace EmailSenderApp
{
    public partial class MainForm : Form
    {
        private Timer animationTimer = new Timer();
        private int animationStep = 0;

        private string emailSubject;
        private string emailBody;
        private RichTextBox logTextBox;
        private string userEmail;
        private string userPassword;

        public MainForm()
        {
            InitializeComponent();
            animationTimer.Interval = 30;
            animationTimer.Tick += AnimationTimer_Tick;

            this.Resize += Form1_Resize;
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Email Gönderme Aracı";
            this.BackColor = Color.FromArgb(30, 30, 30);
            SetupStartButton();
        }

        private void SetupStartButton()
        {
            Button startButton = CreateModernButton("Başla", button1_Click);
            this.Controls.Add(startButton);
            CenterButton(startButton);
        }

        private void CenterButton(Control button)
        {
            button.Left = (this.ClientSize.Width - button.Width) / 2;
            button.Top = (this.ClientSize.Height - button.Height) / 2;
        }

        private Button CreateModernButton(string text, EventHandler clickEvent)
        {
            Button button = new Button
            {
                Size = new Size(150, 50),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 223, 51),
                ForeColor = Color.Black,
                Text = text,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(10)
            };
            button.Click += clickEvent;
            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
            return button;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CenterControls();
        }

        private void CenterControls()
        {
            foreach (Control control in this.Controls)
            {
                control.Left = (this.ClientSize.Width - control.Width) / 2;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearControls();
            animationStep = 0;
            animationTimer.Start();
        }

        private void ClearControls()
        {
            this.Controls.Clear();
            this.BackColor = Color.Black;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (animationStep < 10)
            {
                animationStep++;
            }
            else
            {
                animationTimer.Stop();
                ShowLoginDialog();
            }
        }

        private void ShowLoginDialog()
        {
            Form loginForm = new Form
            {
                Text = "Giriş Bilgileri",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White
            };

            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(20)
            };

            Label emailLabel = new Label { Text = "E-posta:", Location = new Point(10, 20), ForeColor = Color.Black };
            TextBox emailTextBox = new TextBox { Location = new Point(10, 50), Width = 360, Height = 30, Font = new Font("Segoe UI", 12) };

            Label passwordLabel = new Label { Text = "Şifre:", Location = new Point(10, 90), ForeColor = Color.Black };
            TextBox passwordTextBox = new TextBox { Location = new Point(10, 120), Width = 360, Height = 30, Font = new Font("Segoe UI", 12), PasswordChar = '*' };

            Button okButton = CreateModernButton("Tamam", (s, ev) =>
            {
                userEmail = emailTextBox.Text;
                userPassword = passwordTextBox.Text;
                loginForm.Close();
                RequestEmailContent();
            });

            okButton.Location = new Point(10, 170);

            panel.Controls.Add(emailLabel);
            panel.Controls.Add(emailTextBox);
            panel.Controls.Add(passwordLabel);
            panel.Controls.Add(passwordTextBox);
            panel.Controls.Add(okButton);

            loginForm.Controls.Add(panel);
            loginForm.ShowDialog(this);
        }

        private void RequestEmailContent()
        {
            ClearControls();

            // Modern ve düzenli konumlandırma
            Label subjectLabel = new Label { Text = "Mail Başlığı:", Location = new Point(10, 20), ForeColor = Color.White };
            TextBox subjectTextBox = new TextBox { Location = new Point(10, 50), Width = 360, Height = 30, Font = new Font("Segoe UI", 12) };

            Label bodyLabel = new Label { Text = "Mail İçeriği:", Location = new Point(10, 90), ForeColor = Color.White };
            TextBox bodyTextBox = new TextBox { Location = new Point(10, 120), Width = 360, Height = 60, Multiline = true, Font = new Font("Segoe UI", 12) };

            Button sendButton = CreateModernButton("Gönder", (s, ev) => {
                emailSubject = subjectTextBox.Text;
                emailBody = bodyTextBox.Text;
                logTextBox.Clear();
                logTextBox.AppendText("E-posta gönderimi başladı...\n");
                SendEmail(userEmail);
            });

            sendButton.Location = new Point(10, 200);

            logTextBox = new RichTextBox
            {
                ReadOnly = true,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                Height = 100,
                Width = 360,
                Location = new Point(10, 250)
            };

            this.Controls.Add(subjectLabel);
            this.Controls.Add(subjectTextBox);
            this.Controls.Add(bodyLabel);
            this.Controls.Add(bodyTextBox);
            this.Controls.Add(sendButton);
            this.Controls.Add(logTextBox);
        }

        private void SendEmail(string email)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(userEmail, userPassword),
                    EnableSsl = true
                };

                mail.From = new MailAddress(userEmail);
                mail.To.Add(email);
                mail.Subject = emailSubject;
                mail.Body = emailBody;

                smtpClient.Send(mail);
                logTextBox.AppendText($"Email gönderildi: {email}\n");
            }
            catch (Exception ex)
            {
                logTextBox.AppendText($"E-posta gönderim hatası: {ex.Message}\n");
            }
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                button.BackColor = Color.FromArgb(255, 193, 7);
                button.ForeColor = Color.Black;
                button.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                button.BackColor = Color.FromArgb(255, 223, 51);
                button.ForeColor = Color.Black;
                button.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            }
        }
    }
}
