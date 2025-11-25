using System;
using System.Windows.Forms;
using System.Drawing;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.Services;

namespace DTwoMFTimerHelper.UI.Settings {
    public class GeneralSettingsControl : UserControl {
        private GroupBox? groupBoxPosition;
        private GroupBox? groupBoxLanguage;
        private RadioButton? radioTopLeft;
        private RadioButton? radioTopCenter;
        private RadioButton? radioTopRight;
        private RadioButton? radioBottomLeft;
        private RadioButton? radioBottomCenter;
        private RadioButton? radioBottomRight;
        private RadioButton? chineseRadioButton;
        private RadioButton? englishRadioButton;
        private CheckBox? alwaysOnTopCheckBox;
        private Label? alwaysOnTopLabel;

        public GeneralSettingsControl() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            groupBoxPosition = new GroupBox();
            radioTopLeft = new RadioButton();
            radioTopCenter = new RadioButton();
            radioTopRight = new RadioButton();
            radioBottomLeft = new RadioButton();
            radioBottomCenter = new RadioButton();
            radioBottomRight = new RadioButton();
            groupBoxLanguage = new GroupBox();
            chineseRadioButton = new RadioButton();
            englishRadioButton = new RadioButton();
            alwaysOnTopCheckBox = new CheckBox();
            alwaysOnTopLabel = new Label();
            groupBoxPosition.SuspendLayout();
            groupBoxLanguage.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxPosition
            // 
            groupBoxPosition.Controls.Add(radioTopLeft);
            groupBoxPosition.Controls.Add(radioTopCenter);
            groupBoxPosition.Controls.Add(radioTopRight);
            groupBoxPosition.Controls.Add(radioBottomLeft);
            groupBoxPosition.Controls.Add(radioBottomCenter);
            groupBoxPosition.Controls.Add(radioBottomRight);
            groupBoxPosition.Location = new Point(8, 8);
            groupBoxPosition.Name = "groupBoxPosition";
            groupBoxPosition.Size = new Size(300, 110);
            groupBoxPosition.TabIndex = 0;
            groupBoxPosition.TabStop = false;
            groupBoxPosition.Text = "窗口位置";
            // 
            // radioTopLeft
            // 
            radioTopLeft.AutoSize = true;
            radioTopLeft.Checked = true;
            radioTopLeft.Location = new Point(15, 25);
            radioTopLeft.Name = "radioTopLeft";
            radioTopLeft.Size = new Size(79, 32);
            radioTopLeft.TabIndex = 0;
            radioTopLeft.TabStop = true;
            radioTopLeft.Text = "左上";
            // 
            // radioTopCenter
            // 
            radioTopCenter.AutoSize = true;
            radioTopCenter.Location = new Point(110, 25);
            radioTopCenter.Name = "radioTopCenter";
            radioTopCenter.Size = new Size(79, 32);
            radioTopCenter.TabIndex = 1;
            radioTopCenter.Text = "上中";
            // 
            // radioTopRight
            // 
            radioTopRight.AutoSize = true;
            radioTopRight.Location = new Point(205, 25);
            radioTopRight.Name = "radioTopRight";
            radioTopRight.Size = new Size(79, 32);
            radioTopRight.TabIndex = 2;
            radioTopRight.Text = "右上";
            // 
            // radioBottomLeft
            // 
            radioBottomLeft.AutoSize = true;
            radioBottomLeft.Location = new Point(15, 60);
            radioBottomLeft.Name = "radioBottomLeft";
            radioBottomLeft.Size = new Size(79, 32);
            radioBottomLeft.TabIndex = 3;
            radioBottomLeft.Text = "左下";
            // 
            // radioBottomCenter
            // 
            radioBottomCenter.AutoSize = true;
            radioBottomCenter.Location = new Point(110, 60);
            radioBottomCenter.Name = "radioBottomCenter";
            radioBottomCenter.Size = new Size(79, 32);
            radioBottomCenter.TabIndex = 4;
            radioBottomCenter.Text = "下中";
            // 
            // radioBottomRight
            // 
            radioBottomRight.AutoSize = true;
            radioBottomRight.Location = new Point(205, 60);
            radioBottomRight.Name = "radioBottomRight";
            radioBottomRight.Size = new Size(79, 32);
            radioBottomRight.TabIndex = 5;
            radioBottomRight.Text = "右下";
            // 
            // groupBoxLanguage
            // 
            groupBoxLanguage.Controls.Add(chineseRadioButton);
            groupBoxLanguage.Controls.Add(englishRadioButton);
            groupBoxLanguage.Location = new Point(8, 125);
            groupBoxLanguage.Name = "groupBoxLanguage";
            groupBoxLanguage.Size = new Size(300, 70);
            groupBoxLanguage.TabIndex = 1;
            groupBoxLanguage.TabStop = false;
            groupBoxLanguage.Text = "语言";
            // 
            // chineseRadioButton
            // 
            chineseRadioButton.AutoSize = true;
            chineseRadioButton.Checked = true;
            chineseRadioButton.Location = new Point(15, 30);
            chineseRadioButton.Name = "chineseRadioButton";
            chineseRadioButton.Size = new Size(117, 32);
            chineseRadioButton.TabIndex = 0;
            chineseRadioButton.TabStop = true;
            chineseRadioButton.Text = "Chinese";
            // 
            // englishRadioButton
            // 
            englishRadioButton.AutoSize = true;
            englishRadioButton.Location = new Point(155, 30);
            englishRadioButton.Name = "englishRadioButton";
            englishRadioButton.Size = new Size(110, 32);
            englishRadioButton.TabIndex = 1;
            englishRadioButton.Text = "English";
            // 
            // alwaysOnTopCheckBox
            // 
            alwaysOnTopCheckBox.AutoSize = true;
            alwaysOnTopCheckBox.Checked = true;
            alwaysOnTopCheckBox.CheckState = CheckState.Checked;
            alwaysOnTopCheckBox.Location = new Point(134, 212);
            alwaysOnTopCheckBox.Name = "alwaysOnTopCheckBox";
            alwaysOnTopCheckBox.Size = new Size(22, 21);
            alwaysOnTopCheckBox.TabIndex = 3;
            // 
            // alwaysOnTopLabel
            // 
            alwaysOnTopLabel.AutoSize = true;
            alwaysOnTopLabel.Location = new Point(23, 208);
            alwaysOnTopLabel.Name = "alwaysOnTopLabel";
            alwaysOnTopLabel.Size = new Size(96, 28);
            alwaysOnTopLabel.TabIndex = 2;
            alwaysOnTopLabel.Text = "总在最前";
            // 
            // GeneralSettingsControl
            // 
            AutoScroll = true;
            Controls.Add(groupBoxPosition);
            Controls.Add(groupBoxLanguage);
            Controls.Add(alwaysOnTopLabel);
            Controls.Add(alwaysOnTopCheckBox);
            Name = "GeneralSettingsControl";
            Size = new Size(320, 280);
            groupBoxPosition.ResumeLayout(false);
            groupBoxPosition.PerformLayout();
            groupBoxLanguage.ResumeLayout(false);
            groupBoxLanguage.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        public void LoadSettings(Services.IAppSettings settings) {
            if (this.InvokeRequired) { this.Invoke(new Action<Services.IAppSettings>(LoadSettings), settings); return; }

            // 1. 设置“总在最前”
            if (alwaysOnTopCheckBox != null) {
                alwaysOnTopCheckBox.Checked = settings.AlwaysOnTop;
            }

            // 2. 设置语言 (使用 SettingsManager 的转换逻辑)
            if (groupBoxLanguage != null) {
                var langOption = SettingsManager.StringToLanguage(settings.Language);
                if (langOption == SettingsControl.LanguageOption.English) {
                    englishRadioButton!.Checked = true;
                }
                else {
                    chineseRadioButton!.Checked = true;
                }
            }

            // 3. 设置窗口位置 (使用 SettingsManager 的转换逻辑)
            if (groupBoxPosition != null) {
                var position = SettingsManager.StringToWindowPosition(settings.WindowPosition);
                switch (position) {
                    case SettingsControl.WindowPosition.TopLeft: radioTopLeft!.Checked = true; break;
                    case SettingsControl.WindowPosition.TopCenter: radioTopCenter!.Checked = true; break;
                    case SettingsControl.WindowPosition.TopRight: radioTopRight!.Checked = true; break;
                    case SettingsControl.WindowPosition.BottomLeft: radioBottomLeft!.Checked = true; break;
                    case SettingsControl.WindowPosition.BottomCenter: radioBottomCenter!.Checked = true; break;
                    case SettingsControl.WindowPosition.BottomRight: radioBottomRight!.Checked = true; break;
                    default: radioTopLeft!.Checked = true; break;
                }
            }
        }

        public void RefreshUI() {
            // 运行时动态更新文本
            if (this.InvokeRequired) { this.Invoke(new Action(RefreshUI)); return; }

            // 再次检查以防万一
            if (groupBoxPosition == null) return;

            try {
                // 这里的 try-catch 是为了防止 Design 模式下偶然调用 LanguageManager 报错
                groupBoxPosition.Text = LanguageManager.GetString("WindowPosition");
                radioTopLeft!.Text = LanguageManager.GetString("TopLeft");
                radioTopCenter!.Text = LanguageManager.GetString("TopCenter");
                radioTopRight!.Text = LanguageManager.GetString("TopRight");
                radioBottomLeft!.Text = LanguageManager.GetString("BottomLeft");
                radioBottomCenter!.Text = LanguageManager.GetString("BottomCenter");
                radioBottomRight!.Text = LanguageManager.GetString("BottomRight");

                groupBoxLanguage!.Text = LanguageManager.GetString("Language");
                chineseRadioButton!.Text = LanguageManager.GetString("Chinese");
                englishRadioButton!.Text = LanguageManager.GetString("English");

                alwaysOnTopLabel!.Text = LanguageManager.GetString("AlwaysOnTop");
            }
            catch {
                // 如果出错（比如资源找不到），保持 InitializeComponent 中的默认文本
            }
        }

        // ... 属性部分保持不变 ...
        public SettingsControl.WindowPosition SelectedPosition {
            get {
                if (radioTopLeft?.Checked ?? false) return SettingsControl.WindowPosition.TopLeft;
                if (radioTopCenter?.Checked ?? false) return SettingsControl.WindowPosition.TopCenter;
                if (radioTopRight?.Checked ?? false) return SettingsControl.WindowPosition.TopRight;
                if (radioBottomLeft?.Checked ?? false) return SettingsControl.WindowPosition.BottomLeft;
                if (radioBottomCenter?.Checked ?? false) return SettingsControl.WindowPosition.BottomCenter;
                if (radioBottomRight?.Checked ?? false) return SettingsControl.WindowPosition.BottomRight;
                return SettingsControl.WindowPosition.TopLeft;
            }
        }

        public SettingsControl.LanguageOption SelectedLanguage {
            get {
                return (chineseRadioButton?.Checked ?? false) ? SettingsControl.LanguageOption.Chinese : SettingsControl.LanguageOption.English;
            }
        }

        public bool IsAlwaysOnTop => alwaysOnTopCheckBox?.Checked ?? false;
    }
}