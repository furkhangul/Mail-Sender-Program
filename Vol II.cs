using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using Excel = Microsoft.Office.Interop.Excel;

namespace denem
{
    public partial class Form1 : Form
    {
        Timer animationTimer = new Timer();
        private int animationStep = 0;

        private string emailSubject;
        private string emailBody;

        public Form1()
        {
            InitializeComponent();
            animationTimer.Interval = 30;
            animationTimer.Tick += AnimationTimer_Tick;

            this.Resize += new EventHandler(Form1_Resize);
            this.Load += new EventHandler(Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Email Gönderme Aracı";
            this.BackColor = Color.FromArgb(30, 30, 30);

            // Sadece "Start" butonunu ekle
            Button startButton = CreateButton("Start", new EventHandler(button1_Click));
            this.Controls.Add(startButton);
            CenterButton(startButton); // Start butonunu ortala
        }

        private void CenterButton(Control button)
        {
            button.Left = (this.ClientSize.Width - button.Width) / 2;
            button.Top = (this.ClientSize.Height - button.Height) / 2;
        }

        private Button CreateButton(string text, EventHandler clickEvent)
        {
            Button button = new Button
            {
                Size = new Size(150, 50),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Text = text,
                Font = new Font("Arial", 14, FontStyle.Bold)
            };
            button.Click += clickEvent;
            button.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, button.Width, button.Height, 20, 20));
            return button;
        }

        private TextBox CreateTextBox(string placeholder, Point location)
        {
            TextBox textBox = new TextBox
            {
                Text = placeholder,
                ForeColor = Color.Gray,
                Location = location,
                Width = 250 // Genişliği ayarlayın
            };
            textBox.Enter += (s, ev) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };
            textBox.Leave += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
            return textBox;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CenterControls();
        }

        private void CenterControls()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    control.Left = (this.ClientSize.Width - control.Width) / 2;
                    control.Top = (this.ClientSize.Height - control.Height) / 2;
                }
                else if (control is TextBox)
                {
                    control.Left = (this.ClientSize.Width - control.Width) / 2; // Metin kutularını ortala
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Controls.Clear(); // Tüm bileşenleri temizle
            animationStep = 0;
            animationTimer.Start();
            BackgroundImage = null;
            BackColor = Color.Black;

            // "Geri" butonunu ekle
            Button backButton = CreateButton("Geri", new EventHandler(BackButton_Click));
            this.Controls.Add(backButton);
            backButton.Location = new Point(10, 10);

            // "Dosya Ekle", "Mail Başlığı" ve "Mail İçeriği" bileşenlerini ekle ve görünür hale getir
            Button addButton = CreateButton("Dosya Ekle", new EventHandler(addButton_Click));
            this.Controls.Add(addButton);
            addButton.Top = (this.ClientSize.Height - addButton.Height) / 2 + 50; // Yüksekliği ortala

            TextBox subjectTextBox = CreateTextBox("Mail Başlığı", new Point(10, 60));
            this.Controls.Add(subjectTextBox);
            subjectTextBox.Top = (this.ClientSize.Height - subjectTextBox.Height) / 2 - 30; // Yüksekliği ortala

            TextBox bodyTextBox = CreateTextBox("Mail İçeriği", new Point(10, 120));
            bodyTextBox.Multiline = true;
            bodyTextBox.Height = 100;
            this.Controls.Add(bodyTextBox);
            bodyTextBox.Top = (this.ClientSize.Height - bodyTextBox.Height) / 2 + 10; // Yüksekliği ortala
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            // Ekranı temizle ve başlangıca dön
            this.Controls.Clear();
            Form1_Load(sender, e);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                if (filePath.EndsWith(".xls") || filePath.EndsWith(".xlsx"))
                {
                    ReadExcelFile(filePath);
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir Excel dosyası seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ReadExcelFile(string filePath)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Open(filePath);
            Excel.Worksheet worksheet = workbook.Sheets[1];

            Excel.Range range = worksheet.UsedRange;

            for (int row = 1; row <= range.Rows.Count; row++)
            {
                string email = range.Cells[row, 1].Value2.ToString();
                string name = range.Cells[row, 2].Value2.ToString();
                SendEmail(email, name);
            }

            workbook.Close(false);
            excelApp.Quit();
        }

        private void SendEmail(string email, string name)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

                smtpClient.Credentials = new NetworkCredential("your-email@gmail.com", "your-password");
                smtpClient.EnableSsl = true;

                mail.From = new MailAddress("your-email@gmail.com");
                mail.To.Add(email);
                mail.Subject = emailSubject;
                mail.Body = $"Değerli {name},\n\n{emailBody}";

                smtpClient.Send(mail);
                MessageBox.Show("Email sent to " + email);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message);
            }
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
                // Animation bitince "Start" butonunu tekrar göster
                Form1_Load(sender, e);
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
    }
}
