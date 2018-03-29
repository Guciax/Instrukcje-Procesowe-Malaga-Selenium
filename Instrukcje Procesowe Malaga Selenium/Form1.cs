using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;
using System.Runtime.InteropServices;



namespace Instrukcje_Procesowe_Malaga_Selenium
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string RootDir = "";
        string ActiveProductDirectory = "";
        string PathUnderButton = "";
        List <Button> ButtonsList = new List<Button>();
        List<string> ListaFolderowMontaz = new List<string>();
        
        int CurrentImageIndex1 = 0;
        int CurrentImageIndex2 = 0;
        List<Image> ImagList = new List<Image>();
        string[] Okablowanie = new string[] { null };
        string[] Inne = new string[] { null };
        string[] Pakowanie = new string[] { null };
      //  [DllImport("User32.dll", EntryPoint = "MessageBox",
      //  CharSet = CharSet.Auto)]
      //  public static extern int MsgBox(int hWnd, string text, string caption,
      //uint type);

        private void ButtonPressed(string ButtonName)
        {
            PathUnderButton = RootDir + "\\" + ButtonName;
            ListaFolderowMontaz.Clear();
            foreach (var button in ButtonsList)
            {
                button.Visible = false;
            }
            Exitbutton.Visible = false;
            Shutdownbutton.Visible = false;

            label1.Visible = true;
            label1.Text = ButtonName + " 12NC:";
            textBox1.Visible = true;
            button9.Visible = true;
            DirectoryInfo FolderyMontazu = new DirectoryInfo(RootDir + "\\" + ButtonName + "\\MONTAZ");
            DirectoryInfo FolderyPodPrzyciskiem = new DirectoryInfo(RootDir + "\\" + ButtonName);

            try
            {
                listBox1.Items.Clear();
                ActiveProductDirectory = FolderyMontazu.FullName;
                DirectoryInfo[] DirListMontaz = FolderyMontazu.GetDirectories();

                foreach (var dir in DirListMontaz)
                {
                    ListaFolderowMontaz.Add(dir.Name);
                    listBox1.Items.Add(dir.Name);
                    //Debug.WriteLine(dir.Name);
                }
            } catch(Exception e) { TopMostMessageBox.Show( (e.Message)); }

            string InnePath = FolderyPodPrzyciskiem.FullName + "\\INNE";
            string PakowaniePath = FolderyPodPrzyciskiem.FullName + "\\PAKOWANIE";
            string OkablowaniePath = FolderyPodPrzyciskiem.FullName + "\\OKABLOWANIE";

            try
            {
                if (File.Exists(OkablowaniePath + "\\konfiguracja okablowania.csv"))
                    Okablowanie = File.ReadAllLines(OkablowaniePath + "\\konfiguracja okablowania.csv");
            } catch(Exception e) { TopMostMessageBox.Show(e.Message); }

            try { 
                if (File.Exists(InnePath + "\\Konfiguracja inne.csv"))
                    Inne = File.ReadAllLines(InnePath + "\\Konfiguracja inne.csv");
            }
            catch (Exception e) { TopMostMessageBox.Show(e.Message); }

            try { 
            if (File.Exists(PakowaniePath + "\\Konfiguracja pakowania.csv"))
                    Pakowanie = File.ReadAllLines(PakowaniePath + "\\Konfiguracja pakowania.csv");
            }
            catch (Exception e) { TopMostMessageBox.Show(e.Message); }

            textBox1.Text = "";
            textBox1.Focus();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            panel1.Parent = pictureBox1;
            ButtonsList.Add(button1);
            ButtonsList.Add(button2);
            ButtonsList.Add(button3);
            ButtonsList.Add(button4);
            ButtonsList.Add(button5);
            ButtonsList.Add(button6);
            ButtonsList.Add(button7);
            ButtonsList.Add(button8);
            ButtonsList.Add(button11);
            ButtonsList.Add(button12);
            ButtonsList.Add(button13);
            ButtonsList.Add(button14);
            RootDir =ConfigurationManager.AppSettings["Sciezka OPRAWY"].ToString();
            string FullScreen = ConfigurationManager.AppSettings["Pelny Ekran (1 lub 0)"].ToString();

            if (FullScreen=="0")
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = false;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.TopMost = true;
                this.Height = SystemInformation.VirtualScreen.Height;
                progressBar1.Location = new Point(0, this.Height - 30);
            }

            OrganiseButtons();
            this.KeyPreview = true;

            //Mirror interface
            string MirrorInterface = ConfigurationManager.AppSettings["Odwrocenie interfejsu (wartosc 1 lub 0)"].ToString();
            Int32 MirrorInt = 0;
            if (MirrorInterface == "1") MirrorInt = 1;

            if (MirrorInt == 1)
            {
                pictureBox1.Location = new System.Drawing.Point(1920, 0);
                pictureBox2.Location = new System.Drawing.Point(0, 0);
            }

            //Wybór monitora
            string ScreenNo = ConfigurationManager.AppSettings["Ekran startowy (1 lub 2)"].ToString();
            Int32 ScreenNoInt = 0;
            Int32.TryParse(ScreenNo, out ScreenNoInt);
            if (ScreenNoInt > 0) ScreenNoInt = ScreenNoInt - 1;
            //this.Location = Screen.AllScreens[ScreenNoInt].WorkingArea.Location;

            
            //this.StartPosition = FormStartPosition.Manual;
            //this.Location = new Point(0, 0);

            //Okablowanie = System.IO.File.ReadAllLines(RootDir + "\\OKABLOWANIE\\Konfiguracja okablowania.csv");
            //Inne = System.IO.File.ReadAllLines(RootDir + "\\INNE\\Konfiguracja inne.csv");

            pictureBox2.Image = Instrukcje_Procesowe_Malaga_Selenium.Properties.Resources.mst_logo;
            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;

            

            /*ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);*/
        }

        private string[] FilterArray(string[] ArrayToFilter)
        {
            string[] OutputArray = null;

            if (ArrayToFilter!=null)
            {
                foreach (var line in ArrayToFilter)
                {
                    if (line != null)
                        if (line.Contains(";"))
                            if (line.Split(';')[0].Replace(" ", "") == textBox1.Text.Replace(" ", ""))
                            {
                                OutputArray = line.Replace(line.Split(';')[0] + ";", "").Split(';');
                                break;
                            }
                }
            }

            return OutputArray;
        }

        private string FilterDirectories(string DirectoryToFilter, string Numer12NC)
        {
            string outputDir = "";

            DirectoryInfo PathDirInfo = new DirectoryInfo(DirectoryToFilter);
            DirectoryInfo[] AllFolders = PathDirInfo.GetDirectories();

            foreach (DirectoryInfo folder in AllFolders)
            {
                if (folder.Name.Replace(" ", "").Contains(Numer12NC.Replace(" ", "")))
                {
                    outputDir=folder.FullName;
                    break;
                }
            }

            return outputDir;
        }

        int progress = 0;
        private void LoadImages(string FolderName)
        {

            progress = 0;
            ImagList.Clear();
            
            string ImagesPath = ActiveProductDirectory + "\\" + FolderName;
            DirectoryInfo ImageDir = new DirectoryInfo(ImagesPath);
            FileInfo[] Images = ImageDir.GetFiles("*.png");
            int tick = (Int32)Math.Round((Decimal)35 / Images.Length, 0);
            foreach (FileInfo ImgFile in Images) //35% progress 
            {
                ImagList.Add(Image.FromFile(ImgFile.FullName));
                progress += tick;
            }

            string[] wlasciewe_okablowanie = FilterArray(Okablowanie); //40% progress (+5%)
            progress += 2;
            string[] wlasciewe_inne = FilterArray(Inne);
            progress += 1;
            string[] wlasciewe_pakowanie = FilterArray(Pakowanie);
            progress =40;

            if (wlasciewe_okablowanie != null) //60% (+20%)
            {
                tick = (Int32)Math.Round((Decimal)20 / wlasciewe_okablowanie.Length, 0);
                foreach (var item in wlasciewe_okablowanie)
                {
                    if (item != null)
                        if (item.Length > 2)
                        {
                            string OkablowanieImagesPath = FilterDirectories(PathUnderButton + "\\OKABLOWANIE", item);
                            if (OkablowanieImagesPath != "")
                            {
                                DirectoryInfo OkablowanieImageDir = new DirectoryInfo(OkablowanieImagesPath);
                                FileInfo[] OkablowanieImages = OkablowanieImageDir.GetFiles("*.png");
                                foreach (FileInfo ImgFile in OkablowanieImages)
                                {
                                    ImagList.Add(Image.FromFile(ImgFile.FullName));
                                    Debug.WriteLine("Added Ok " + ImgFile.FullName);
                                }
                            }
                            else
                            {
                                TopMostMessageBox.Show("Brak folderu OKABLOWANIE dla: " + item);
                                zapisz_braki(" Brak folderu OKABLOWANIE dla: " + item);
                            }
                            }
                    progress += tick;
                }
            }
            else
                progress = 60;
            if (wlasciewe_inne != null)//80%
            {
                tick = (Int32)Math.Round((Decimal)20 / wlasciewe_inne.Length, 0);
                foreach (var item in wlasciewe_inne)
                {
                    if (item != null)
                        if (item.Length > 2)
                        {
                            string inneImagesPath = FilterDirectories(PathUnderButton + "\\INNE", item);
                            if (inneImagesPath != "")
                            {
                                DirectoryInfo inneImageDir = new DirectoryInfo(inneImagesPath);
                                FileInfo[] inneImages = inneImageDir.GetFiles("*.png");
                                foreach (FileInfo ImgFile in inneImages)
                                {
                                    ImagList.Add(Image.FromFile(ImgFile.FullName));
                                    Debug.WriteLine("Added In " + ImgFile.FullName);
                                }
                            }
                            else
                            {
                                TopMostMessageBox.Show("Brak folderu INNE dla: " + item);
                                zapisz_braki(" Brak folderu OKABLOWANIE dla: " + item);
                            }
                        }
                    progress += tick;
                }
            }
            else
                progress = 80;

            if (wlasciewe_pakowanie != null)//100%
            {
                tick = (Int32)Math.Round((Decimal)20 / wlasciewe_pakowanie.Length, 0);
                foreach (var item in wlasciewe_pakowanie)
                {
                    if (item != null)
                        if (item.Length > 3)
                        {
                            string pakowanieImagesPath = FilterDirectories(PathUnderButton + "\\PAKOWANIE", item);
                            if (pakowanieImagesPath != "")
                            {
                                DirectoryInfo pakowanieImageDir = new DirectoryInfo(pakowanieImagesPath);
                                FileInfo[] pakowanieImages = pakowanieImageDir.GetFiles("*.png");
                                foreach (FileInfo ImgFile in pakowanieImages)
                                {
                                    ImagList.Add(Image.FromFile(ImgFile.FullName));
                                    Debug.WriteLine("Added Pa " + ImgFile.FullName);
                                }
                            }
                            else
                            {
                                TopMostMessageBox.Show("Brak folderu PAKOWANIE dla: " + item);
                                zapisz_braki(" Brak folderu PAKOWANIE dla: " + item);
                            }
                        }
                    if (progress + tick >= 100) progress = 100; else progress+= tick;
                }
            }
            else
                progress = 100;
            ThreadFinished = true;

        }

        private void zapisz_braki(string msg)
        {
            try
            {
                using (FileStream fs = new FileStream("ListaBrakow.txt", FileMode.Append, FileAccess.Write))
                using (var writer = new StreamWriter(fs))
                {

                    writer.WriteLine(System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToShortTimeString() + " " + msg + ", Oprawa:" + textBox1.Text);
                }
            }
            catch (Exception e) { }
        }

        private void OrganiseButtons()
        {
            DirectoryInfo RootDirInfo = new DirectoryInfo(RootDir);
            DirectoryInfo[] Foldery = RootDirInfo.GetDirectories();

            //foreach (DirectoryInfo Dir in Foldery)
            //{
            //    ButtonsList[0].Text = Dir.Name;
            //}
            int Buttonindex = 0;
            for (int i = 0; i < Foldery.Count(); i++)
            {
                if (Foldery[i].Name != "OKABLOWANIE" & Foldery[i].Name != "INNE")
                {
                    ButtonsList[Buttonindex].Visible = true;
                    ButtonsList[Buttonindex].Text = Foldery[i].Name;
                    Buttonindex++;
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            panel_pass.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonPressed(button1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ButtonPressed(button2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ButtonPressed(button3.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ButtonPressed(button4.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ButtonPressed(button5.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ButtonPressed(button6.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ButtonPressed(button7.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ButtonPressed(button8.Text);
        }

        private void ShowButtons()
        {
            button9.Visible = false;
            if (panel1.Visible)
            {
                foreach (var button in ButtonsList)
                {
                    if (button.Text != "") button.Visible = true;
                    label1.Visible = false;
                    textBox1.Visible = false;
                    Exitbutton.Visible = true;
                    Shutdownbutton.Visible = true;
                }
                pictureBox1.Image = null;
                pictureBox2.Image = null;
            }
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            ShowButtons();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine(e.KeyCode.ToString());
            if (panel1.Visible == false)
            {
                switch (e.KeyCode)
                {
                    case Keys.ControlKey:
                        {
                            if (CurrentImageIndex1 == 0)
                                CurrentImageIndex1 = ImagList.Count - 1;
                            else
                                CurrentImageIndex1--;
                            pictureBox1.Image = ImagList[CurrentImageIndex1];
                            label2.Text = (CurrentImageIndex1 + 1).ToString() + "/" + ImagList.Count.ToString();
                            break;
                        }
                
                    case Keys.ShiftKey:
                        {
                            if (CurrentImageIndex1 < ImagList.Count - 1)
                            {
                                CurrentImageIndex1++;
                            }
                            else CurrentImageIndex1 = 0;
                            pictureBox1.Image = ImagList[CurrentImageIndex1];
                            label2.Text = (CurrentImageIndex1 + 1).ToString() + "/" + ImagList.Count.ToString();
                            break;
                        }

                    case Keys.Subtract:
                        {
                            if (CurrentImageIndex2 == 0)
                                CurrentImageIndex2 = ImagList.Count - 1;
                            else
                                CurrentImageIndex2--;
                            pictureBox2.Image = ImagList[CurrentImageIndex2];
                            label3.Text = (CurrentImageIndex2 + 1).ToString() + "/" + ImagList.Count.ToString();
                            break;
                        }

                    case Keys.Add:
                        {
                            if (CurrentImageIndex2 < ImagList.Count - 1)
                            {
                                CurrentImageIndex2++;
                            }
                            else CurrentImageIndex2 = 0;
                            pictureBox2.Image = ImagList[CurrentImageIndex2];
                            label3.Text = (CurrentImageIndex2 + 1).ToString() + "/" + ImagList.Count.ToString();
                            break;
                        }
                    case Keys.Escape:
                        {
                            panel1.Visible = true;
                            ShowButtons();
                            pictureBox1.Image = null;
                            pictureBox2.Image = null;
                            ImagList.Clear();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            break;
                        }
                }

            }
            else
            {
                if (e.KeyCode == Keys.Home)
                {
                    if (listBox1.Visible) listBox1.Visible = false; else listBox1.Visible = true;
                }
            }
        }

        Boolean ThreadFinished = false;
        private void maskedTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (panel1.Visible)
            {
                if (e.KeyCode == Keys.Return & textBox1.Text.Length > 0)
                {
                    if (ListaFolderowMontaz.Contains(textBox1.Text))
                    {
                        progressBar1.Visible = true;
                        panel1.Visible = false;
                        ThreadFinished = false;
                        ThreadTimer.Enabled = true;
                        pictureBox1.Image = Properties.Resources.hexLoader;
                        pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                        pictureBox2.Image = Properties.Resources.hexLoader;
                        pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
                        label12NC.Text = textBox1.Text;
                        new Thread(delegate ()
                        {
                            LoadImages(textBox1.Text);
                            
                        }).Start();
                    }
                    else TopMostMessageBox.Show("Nieprawidłowe 12NC");
                }
                if (e.KeyCode== Keys.Escape)
                    ShowButtons();
            }
        }

        private void ThreadTimer_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = progress;
            
            if (ThreadFinished)
            {
                CurrentImageIndex1 = 0;
                CurrentImageIndex2 = 0;
                progressBar1.Visible = false;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = ImagList[CurrentImageIndex1];
                label2.Text = (CurrentImageIndex1 + 1).ToString() + "/" + ImagList.Count.ToString();
                pictureBox2.Image = ImagList[CurrentImageIndex2];
                label3.Text = (CurrentImageIndex2 + 1).ToString() + "/" + ImagList.Count.ToString();
                ThreadTimer.Enabled = false;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = listBox1.SelectedItem.ToString();
            listBox1.Visible = false;
            textBox1.Focus();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            pictureBox1.Image = Properties.Resources.spinner;
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textBox_pass.Text!= "typefoud")
                e.Cancel = true;
            else
            {
                panel_pass.Visible = false;
                textBox_pass.Text = "";
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            panel_pass.Visible = false;
        }

        private void textBox_pass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.Close();

        }

        private void button11_Click(object sender, EventArgs e)
        {
            panel_pass.Visible = false;
            textBox_pass.Text = "typefoud";

            var psi = new ProcessStartInfo("shutdown", "/s /t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            DialogResult dialres = TopMostMessageBox.Show("Nastąpi zamknięcie komputera.", "Ostrzeżenie!", MessageBoxButtons.YesNo);

            if (dialres == DialogResult.Yes)
                Process.Start(psi);
            else
                textBox_pass.Text = "";

        }

        private void panel_pass_VisibleChanged(object sender, EventArgs e)
        {
            if (panel_pass.Visible) textBox_pass.Focus();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }
        //System level functions to be used for hook and unhook keyboard input  
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);
        //Declaring Global objects     
        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess;

        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                // Disabling Windows keys 

                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin || objKeyInfo.key == Keys.Tab && HasAltModifier(objKeyInfo.flags) || objKeyInfo.key == Keys.Escape && (ModifierKeys & Keys.Control) == Keys.Control)
                {
                    return (IntPtr)1; // if 0 is returned then All the above keys will be enabled
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }

        bool HasAltModifier(int flags)
        {
            return (flags & 0x20) == 0x20;
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            ButtonPressed(button11.Text);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ButtonPressed(button14.Text);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            ButtonPressed(button13.Text);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ButtonPressed(button14.Text);
        }
    }
}
