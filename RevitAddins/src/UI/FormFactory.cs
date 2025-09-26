using Autodesk.Revit.DB;
using RevitAddins.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using WF = System.Windows.Forms;
using SD = System.Drawing;
using System.Runtime.InteropServices;

namespace RevitAddins.UI
{
    public static class FormFactory
    {
        // Windows API to set title bar color
        private const int DWMWA_CAPTION_COLOR = 35;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            int attr,
            ref int attrValue,
            int attrSize
        );

        public static WF.Form CreatePipeSettingsForm(Document doc, string cadName, string[] layers, Dictionary<string, ElementId> pipeDict)
        {
            var form = new WF.Form
            {
                Text = "Pipe Input",
                Size = new SD.Size(400, 550),
                StartPosition = WF.FormStartPosition.CenterScreen,
                BackColor = SD.Color.WhiteSmoke,
                FormBorderStyle = WF.FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            // Apply custom header color (#d8f3fd)
            form.Load += (s, e) =>
            {
                var headerColor = SD.ColorTranslator.FromHtml("#d8f3fd");
                int rgb = headerColor.R | (headerColor.G << 8) | (headerColor.B << 16);
                DwmSetWindowAttribute(form.Handle, DWMWA_CAPTION_COLOR, ref rgb, sizeof(int));
            };

            int labelX = 30, controlX = 200, y = 60, spacing = 40;

            // CAD Element
            AddLabel(form, "Selected Element ID:", labelX, y);
            var cadTextBox = AddTextBox(form, cadName, controlX, y, readOnly: true);
            y += spacing;

            // Layer ComboBox
            AddLabel(form, "Select Layer:", labelX, y);
            var layerCombo = AddComboBox(form, layers, controlX, y);
            y += spacing;

            // Pipe Type ComboBox
            AddLabel(form, "Select Pipe:", labelX, y);
            var pipeCombo = AddComboBox(form, pipeDict.Keys.ToArray(), controlX, y);
            y += spacing;

            // System Type ComboBox
            string[] systemTypeNames = SystemTypeUtils.GetSystemTypeNames(doc);
            AddLabel(form, "Select System Pipe Type:", labelX, y);
            var systemCombo = AddComboBox(form, systemTypeNames, controlX, y);
            y += spacing;

            // Level ComboBox
            var levels = LevelUtils.GetLevelDisplayNames(doc) ?? new Dictionary<string, ElementId>();
            AddLabel(form, "Select Level:", labelX, y);
            var levelCombo = AddComboBox(form, levels.Keys.ToArray(), controlX, y);

            var activeLevelId = LevelUtils.GetActiveViewLevelId(doc);
            var match = levels.FirstOrDefault(kvp => kvp.Value == activeLevelId);
            if (!string.IsNullOrEmpty(match.Key)) levelCombo.SelectedItem = match.Key;
            else if (levelCombo.Items.Count > 0) levelCombo.SelectedIndex = 0;

            y += spacing;

            // Diameter TextBox
            AddLabel(form, "Pipe Diameter (mm):", labelX, y);
            var diameterTextBox = AddTextBox(form, "80", controlX, y);
            y += spacing;

            // Offset TextBox
            AddLabel(form, "Custom Offset (mm):", labelX, y);
            var offsetTextBox = AddTextBox(form, "2500", controlX, y);
            y += spacing;

            // Quick Offset Options
            AddLabel(form, "Or Select Offset:", labelX, y);
            var offset2200 = AddRadioButton(form, "2200", controlX, y);
            var offset2300 = AddRadioButton(form, "2300", controlX + 70, y);
            var offset2400 = AddRadioButton(form, "2400", controlX + 140, y);
            y += spacing + 10;

            // 🔹 Buttons styled with #d8f3fd
            var buttonColor = SD.ColorTranslator.FromHtml("#d8f3fd");

            var submitButton = new WF.Button
            {
                Text = "Submit",
                Location = new SD.Point(100, y),
                Width = 80,
                BackColor = buttonColor,
                FlatStyle = WF.FlatStyle.Flat
            };
            submitButton.FlatAppearance.BorderColor = SD.Color.Gray;

            var cancelButton = new WF.Button
            {
                Text = "Cancel",
                Location = new SD.Point(200, y),
                Width = 80,
                BackColor = buttonColor,
                FlatStyle = WF.FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderColor = SD.Color.Gray;

            // Button logic
            submitButton.Click += (s, e) =>
            {
                if (!double.TryParse(diameterTextBox.Text, out double d) || d <= 0)
                {
                    WF.MessageBox.Show("Enter valid diameter (>0 mm).", "Warning",
                        WF.MessageBoxButtons.OK, WF.MessageBoxIcon.Warning);
                    return;
                }
                if (layerCombo.SelectedItem == null || pipeCombo.SelectedItem == null ||
                    systemCombo.SelectedItem == null || levelCombo.SelectedItem == null)
                {
                    WF.MessageBox.Show("Please fill all required fields.", "Warning",
                        WF.MessageBoxButtons.OK, WF.MessageBoxIcon.Warning);
                    return;
                }
                form.DialogResult = WF.DialogResult.OK;
                form.Close();
            };

            cancelButton.Click += (s, e) =>
            {
                if (WF.MessageBox.Show("Cancel operation?", "Confirm",
                    WF.MessageBoxButtons.YesNo) == WF.DialogResult.Yes)
                    form.Close();
            };

            form.Controls.Add(submitButton);
            form.Controls.Add(cancelButton);

            return form;
        }

        // ---------------- Helper Methods ----------------
        private static WF.Label AddLabel(WF.Form form, string text, int x, int y)
        {
            var label = new WF.Label { Text = text, Location = new SD.Point(x, y), AutoSize = true };
            form.Controls.Add(label);
            return label;
        }

        private static WF.TextBox AddTextBox(WF.Form form, string text, int x, int y, bool readOnly = false)
        {
            var tb = new WF.TextBox { Text = text, Location = new SD.Point(x, y), Width = 100, ReadOnly = readOnly };
            form.Controls.Add(tb);
            return tb;
        }

        private static WF.ComboBox AddComboBox(WF.Form form, string[] items, int x, int y)
        {
            var combo = new WF.ComboBox { Location = new SD.Point(x, y), Width = 160, DropDownStyle = WF.ComboBoxStyle.DropDownList };
            combo.Items.AddRange(items ?? new string[0]);
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
            form.Controls.Add(combo);
            return combo;
        }

        private static WF.RadioButton AddRadioButton(WF.Form form, string text, int x, int y)
        {
            var radio = new WF.RadioButton { Text = text, Location = new SD.Point(x, y), AutoSize = true };
            form.Controls.Add(radio);
            return radio;
        }
    }
}
