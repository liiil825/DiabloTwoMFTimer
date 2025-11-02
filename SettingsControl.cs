using System;
using System.Windows.Forms;
using System.Drawing;
using WinFormsDemo.Resources;

namespace WinFormsDemo
{
    public partial class SettingsControl : UserControl
    {
        // 窗口位置枚举
        public enum WindowPosition
        {
            TopLeft,
            TopCenter,
            TopRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }

        // 事件
        public event EventHandler<WindowPositionChangedEventArgs>? WindowPositionChanged;
        public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;
        public event EventHandler<AlwaysOnTopChangedEventArgs>? AlwaysOnTopChanged;
        
        // 语言枚举
        public enum LanguageOption
        {
            Chinese,
            English
        }

        public SettingsControl()
        {
            InitializeComponent();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            this.btnConfirmSettings = new System.Windows.Forms.Button();
            this.radioBottomLeft = new System.Windows.Forms.RadioButton();
            this.radioBottomCenter = new System.Windows.Forms.RadioButton();
            this.radioBottomRight = new System.Windows.Forms.RadioButton();
            this.radioTopRight = new System.Windows.Forms.RadioButton();
            this.radioTopCenter = new System.Windows.Forms.RadioButton();
            this.radioTopLeft = new System.Windows.Forms.RadioButton();
            this.groupBoxPosition = new System.Windows.Forms.GroupBox();
            this.chineseRadioButton = new System.Windows.Forms.RadioButton();
            this.englishRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBoxLanguage = new System.Windows.Forms.GroupBox();
            this.alwaysOnTopCheckBox = new System.Windows.Forms.CheckBox();
            this.alwaysOnTopLabel = new System.Windows.Forms.Label();
            this.groupBoxPosition.SuspendLayout();
            this.groupBoxLanguage.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConfirmSettings
            // 
            this.btnConfirmSettings.Location = new System.Drawing.Point(76, 350);
            this.btnConfirmSettings.Name = "btnConfirmSettings";
            this.btnConfirmSettings.Size = new System.Drawing.Size(140, 30);
            this.btnConfirmSettings.TabIndex = 1;
            this.btnConfirmSettings.UseVisualStyleBackColor = true;
            this.btnConfirmSettings.Click += new System.EventHandler(this.btnConfirmSettings_Click);
            // 
            // radioBottomLeft
            // 
            this.radioBottomLeft.AutoSize = true;
            this.radioBottomLeft.Location = new System.Drawing.Point(20, 100);
            this.radioBottomLeft.Name = "radioBottomLeft";
            this.radioBottomLeft.Size = new System.Drawing.Size(47, 19);
            this.radioBottomLeft.TabIndex = 5;
            this.radioBottomLeft.UseVisualStyleBackColor = true;
            // 
            // radioBottomCenter
            // 
            this.radioBottomCenter.AutoSize = true;
            this.radioBottomCenter.Location = new System.Drawing.Point(100, 100);
            this.radioBottomCenter.Name = "radioBottomCenter";
            this.radioBottomCenter.Size = new System.Drawing.Size(47, 19);
            this.radioBottomCenter.TabIndex = 4;
            this.radioBottomCenter.UseVisualStyleBackColor = true;
            // 
            // radioBottomRight
            // 
            this.radioBottomRight.AutoSize = true;
            this.radioBottomRight.Location = new System.Drawing.Point(180, 100);
            this.radioBottomRight.Name = "radioBottomRight";
            this.radioBottomRight.Size = new System.Drawing.Size(47, 19);
            this.radioBottomRight.TabIndex = 3;
            this.radioBottomRight.UseVisualStyleBackColor = true;
            // 
            // radioTopRight
            // 
            this.radioTopRight.AutoSize = true;
            this.radioTopRight.Location = new System.Drawing.Point(180, 40);
            this.radioTopRight.Name = "radioTopRight";
            this.radioTopRight.Size = new System.Drawing.Size(47, 19);
            this.radioTopRight.TabIndex = 2;
            this.radioTopRight.UseVisualStyleBackColor = true;
            // 
            // radioTopCenter
            // 
            this.radioTopCenter.AutoSize = true;
            this.radioTopCenter.Location = new System.Drawing.Point(100, 40);
            this.radioTopCenter.Name = "radioTopCenter";
            this.radioTopCenter.Size = new System.Drawing.Size(47, 19);
            this.radioTopCenter.TabIndex = 1;
            this.radioTopCenter.UseVisualStyleBackColor = true;
            // 
            // radioTopLeft
            // 
            this.radioTopLeft.AutoSize = true;
            this.radioTopLeft.Checked = true;
            this.radioTopLeft.Location = new System.Drawing.Point(20, 40);
            this.radioTopLeft.Name = "radioTopLeft";
            this.radioTopLeft.Size = new System.Drawing.Size(47, 19);
            this.radioTopLeft.TabIndex = 0;
            this.radioTopLeft.TabStop = true;
            this.radioTopLeft.UseVisualStyleBackColor = true;
            // 
            // groupBoxPosition
            // 
            this.groupBoxPosition.Controls.Add(this.radioBottomLeft);
            this.groupBoxPosition.Controls.Add(this.radioBottomCenter);
            this.groupBoxPosition.Controls.Add(this.radioBottomRight);
            this.groupBoxPosition.Controls.Add(this.radioTopRight);
            this.groupBoxPosition.Controls.Add(this.radioTopCenter);
            this.groupBoxPosition.Controls.Add(this.radioTopLeft);
            this.groupBoxPosition.Location = new System.Drawing.Point(30, 20);
            this.groupBoxPosition.Name = "groupBoxPosition";
            this.groupBoxPosition.Size = new System.Drawing.Size(230, 140);
            this.groupBoxPosition.TabIndex = 0;
            this.groupBoxPosition.TabStop = false;
            // 
            // chineseRadioButton
            // 
            this.chineseRadioButton.AutoSize = true;
            this.chineseRadioButton.Location = new System.Drawing.Point(30, 30);
            this.chineseRadioButton.Name = "chineseRadioButton";
            this.chineseRadioButton.Size = new System.Drawing.Size(47, 19);
            this.chineseRadioButton.TabIndex = 0;
            this.chineseRadioButton.Checked = true;
            this.chineseRadioButton.TabStop = true;
            this.chineseRadioButton.UseVisualStyleBackColor = true;
            // 
            // englishRadioButton
            // 
            this.englishRadioButton.AutoSize = true;
            this.englishRadioButton.Location = new System.Drawing.Point(100, 30);
            this.englishRadioButton.Name = "englishRadioButton";
            this.englishRadioButton.Size = new System.Drawing.Size(47, 19);
            this.englishRadioButton.TabIndex = 1;
            this.englishRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBoxLanguage
            // 
            this.groupBoxLanguage.Controls.Add(this.chineseRadioButton);
            this.groupBoxLanguage.Controls.Add(this.englishRadioButton);
            this.groupBoxLanguage.Location = new System.Drawing.Point(30, 180);
            this.groupBoxLanguage.Name = "groupBoxLanguage";
            this.groupBoxLanguage.Size = new System.Drawing.Size(230, 70);
            this.groupBoxLanguage.TabIndex = 2;
            this.groupBoxLanguage.TabStop = false;
            // 
            // alwaysOnTopCheckBox
            // 
            this.alwaysOnTopCheckBox.AutoSize = true;
            this.alwaysOnTopCheckBox.Location = new System.Drawing.Point(120, 280);
            this.alwaysOnTopCheckBox.Name = "alwaysOnTopCheckBox";
            this.alwaysOnTopCheckBox.Size = new System.Drawing.Size(15, 14);
            this.alwaysOnTopCheckBox.TabIndex = 3;
            this.alwaysOnTopCheckBox.UseVisualStyleBackColor = true;
            this.alwaysOnTopCheckBox.Checked = true;
            // 
            // alwaysOnTopLabel
            // 
            this.alwaysOnTopLabel.AutoSize = true;
            this.alwaysOnTopLabel.Location = new System.Drawing.Point(30, 280);
            this.alwaysOnTopLabel.Name = "alwaysOnTopLabel";
            this.alwaysOnTopLabel.Size = new System.Drawing.Size(70, 15);
            this.alwaysOnTopLabel.TabIndex = 4;
            // 
            // SettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.alwaysOnTopLabel);
            this.Controls.Add(this.alwaysOnTopCheckBox);
            this.Controls.Add(this.groupBoxLanguage);
            this.Controls.Add(this.btnConfirmSettings);
            this.Controls.Add(this.groupBoxPosition);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(292, 400);
            this.groupBoxPosition.ResumeLayout(false);
            this.groupBoxPosition.PerformLayout();
            this.groupBoxLanguage.ResumeLayout(false);
            this.groupBoxLanguage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void UpdateUI()
        {
            // 更新设置页面文本
            btnConfirmSettings!.Text = LanguageManager.GetString("ConfirmSettings");
            groupBoxPosition!.Text = LanguageManager.GetString("WindowPosition");
            radioTopLeft!.Text = LanguageManager.GetString("TopLeft");
            radioTopCenter!.Text = LanguageManager.GetString("TopCenter");
            radioTopRight!.Text = LanguageManager.GetString("TopRight");
            radioBottomLeft!.Text = LanguageManager.GetString("BottomLeft");
            radioBottomCenter!.Text = LanguageManager.GetString("BottomCenter");
            radioBottomRight!.Text = LanguageManager.GetString("BottomRight");
            
            // 更新语言选择文本
            groupBoxLanguage!.Text = LanguageManager.GetString("Language");
            chineseRadioButton!.Text = LanguageManager.GetString("Chinese");
            englishRadioButton!.Text = LanguageManager.GetString("English");
            
            // 更新窗口置顶文本
            alwaysOnTopLabel!.Text = LanguageManager.GetString("AlwaysOnTop");
        }

        public void ApplyWindowPosition(Form form)
        {
            WindowPosition position = GetSelectedPosition();
            MoveWindowToPosition(form, position);
        }

        private WindowPosition GetSelectedPosition()
        {
            if (radioTopLeft?.Checked ?? false) return WindowPosition.TopLeft;
            if (radioTopCenter?.Checked ?? false) return WindowPosition.TopCenter;
            if (radioTopRight?.Checked ?? false) return WindowPosition.TopRight;
            if (radioBottomLeft?.Checked ?? false) return WindowPosition.BottomLeft;
            if (radioBottomCenter?.Checked ?? false) return WindowPosition.BottomCenter;
            if (radioBottomRight?.Checked ?? false) return WindowPosition.BottomRight;
            return WindowPosition.TopLeft; // 默认左上
        }

        public void MoveWindowToPosition(Form form, WindowPosition position)
        {
            // 获取屏幕工作区域
            Rectangle screenBounds = Screen.GetWorkingArea(form);
            
            int x, y;
            
            switch (position)
            {
                case WindowPosition.TopLeft:
                    x = screenBounds.Left;
                    y = screenBounds.Top;
                    break;
                case WindowPosition.TopCenter:
                    x = screenBounds.Left + (screenBounds.Width - form.Width) / 2;
                    y = screenBounds.Top;
                    break;
                case WindowPosition.TopRight:
                    x = screenBounds.Right - form.Width;
                    y = screenBounds.Top;
                    break;
                case WindowPosition.BottomLeft:
                    x = screenBounds.Left;
                    y = screenBounds.Bottom - form.Height;
                    break;
                case WindowPosition.BottomCenter:
                    x = screenBounds.Left + (screenBounds.Width - form.Width) / 2;
                    y = screenBounds.Bottom - form.Height;
                    break;
                case WindowPosition.BottomRight:
                    x = screenBounds.Right - form.Width;
                    y = screenBounds.Bottom - form.Height;
                    break;
                default:
                    return;
            }
            
            // 设置窗口位置
            form.Location = new Point(x, y);
        }

        private void btnConfirmSettings_Click(object? sender, EventArgs e)
        {
            // 触发位置变更事件
            WindowPosition position = GetSelectedPosition();
            WindowPositionChanged?.Invoke(this, new WindowPositionChangedEventArgs(position));
            
            // 触发语言变更事件
            LanguageOption language = GetSelectedLanguage();
            LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(language));
            
            // 触发窗口置顶变更事件
            bool isAlwaysOnTop = alwaysOnTopCheckBox?.Checked ?? false;
            AlwaysOnTopChanged?.Invoke(this, new AlwaysOnTopChangedEventArgs(isAlwaysOnTop));
        }
        
        private LanguageOption GetSelectedLanguage()
        {
            if (chineseRadioButton?.Checked ?? false) return LanguageOption.Chinese;
            if (englishRadioButton?.Checked ?? false) return LanguageOption.English;
            return LanguageOption.Chinese; // 默认中文
        }

        // 位置变更事件参数
        public class WindowPositionChangedEventArgs : EventArgs
        {
            public WindowPosition Position { get; }
            
            public WindowPositionChangedEventArgs(WindowPosition position)
            {
                Position = position;
            }
        }
        
        // 语言变更事件参数
        public class LanguageChangedEventArgs : EventArgs
        {
            public LanguageOption Language { get; }
            
            public LanguageChangedEventArgs(LanguageOption language)
            {
                Language = language;
            }
        }
        
        // 窗口置顶变更事件参数
        public class AlwaysOnTopChangedEventArgs : EventArgs
        {
            public bool IsAlwaysOnTop { get; }
            
            public AlwaysOnTopChangedEventArgs(bool isAlwaysOnTop)
            {
                IsAlwaysOnTop = isAlwaysOnTop;
            }
        }

        // 控件字段
        private Button? btnConfirmSettings;
        private RadioButton? radioBottomLeft;
        private RadioButton? radioBottomCenter;
        private RadioButton? radioBottomRight;
        private RadioButton? radioTopRight;
        private RadioButton? radioTopCenter;
        private RadioButton? radioTopLeft;
        private GroupBox? groupBoxPosition;
        private RadioButton? chineseRadioButton;
        private RadioButton? englishRadioButton;
        private GroupBox? groupBoxLanguage;
        private CheckBox? alwaysOnTopCheckBox;
        private Label? alwaysOnTopLabel;
    }
}