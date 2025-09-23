using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Linq;
using SD = System.Drawing;        // Alias for System.Drawing
using WF = System.Windows.Forms;  // Alias for WinForms

namespace RevitAddins.UI
{
    public class FirePipeSettingsForm : WF.Form
    {
        private WF.Label elementLabel;
        private WF.Label layerLabel;
        private WF.ComboBox layerCombo;

        private WF.Label pipeTypeLabel;
        private WF.ComboBox pipeTypeCombo;

        private WF.Label systemTypeLabel;
        private WF.ComboBox systemTypeCombo;

        private WF.Label levelLabel;
        private WF.ComboBox levelCombo;

        private WF.Label diameterLabel;
        private WF.TextBox diameterTextBox;

        private WF.Label offsetLabel;
        private WF.TextBox offsetTextBox;

        private WF.Label offsetOptionsLabel;
        private WF.RadioButton offset2200;
        private WF.RadioButton offset2300;
        private WF.RadioButton offset2400;

        private WF.Button submitButton;
        private WF.Button cancelButton;

        private Document _doc;

        public string SelectedLayer => layerCombo.SelectedItem?.ToString();
        public string SelectedPipeType => pipeTypeCombo.SelectedItem?.ToString();
        public string SelectedSystemType => systemTypeCombo.SelectedItem?.ToString();
        public Level SelectedLevel => levelCombo.SelectedItem as Level;
        public double Diameter => double.TryParse(diameterTextBox.Text, out double d) ? d : 0;
        public double Offset => double.TryParse(offsetTextBox.Text, out double o) ? o : 0;

        public FirePipeSettingsForm(Document doc)
        {
            _doc = doc;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form properties
            this.Text = "Pipe Input";
            this.Size = new SD.Size(400, 550);
            this.StartPosition = WF.FormStartPosition.CenterScreen;
            this.BackColor = SD.Color.WhiteSmoke;

            int labelX = 30;
            int controlX = 200;
            int y = 20;
            int spacing = 40;

            // Selected Element ID
            elementLabel = new WF.Label
            {
                Text = "Selected Element ID: None",
                Location = new SD.Point(labelX, y),
                AutoSize = true
            };
            this.Controls.Add(elementLabel);
            y += spacing;

            // Layer
            layerLabel = new WF.Label { Text = "Select Layer:", Location = new SD.Point(labelX, y), AutoSize = true };
            this.Controls.Add(layerLabel);

            layerCombo = new WF.ComboBox { Location = new SD.Point(controlX, y), Width = 140, DropDownStyle = WF.ComboBoxStyle.DropDownList };
            // Populate layers from document
            var layers = new FilteredElementCollector(_doc).OfClass(typeof(Level)).Cast<Level>().Select(l => l.Name).ToArray();
            layerCombo.Items.AddRange(layers);
            this.Controls.Add(layerCombo);
            y += spacing;

            // Pipe Type
            pipeTypeLabel = new WF.Label { Text = "Select Pipe:", Location = new SD.Point(labelX, y), AutoSize = true };
            this.Controls.Add(pipeTypeLabel);

            pipeTypeCombo = new WF.ComboBox { Location = new SD.Point(controlX, y), Width = 140, DropDownStyle = WF.ComboBoxStyle.DropDownList };
            var pipeTypes = new FilteredElementCollector(_doc).OfClass(typeof(PipeType)).Cast<PipeType>().Select(p => p.Name).ToArray();
            pipeTypeCombo.Items.AddRange(pipeTypes);
            this.Controls.Add(pipeTypeCombo);
            y += spacing;

            // System Type
            systemTypeLabel = new WF.Label { Text = "Select System Pipe Type:", Location = new SD.Point(labelX, y), AutoSize = true };
            this.Controls.Add(systemTypeLabel);

            systemTypeCombo = new WF.ComboBox { Location = new SD.Point(controlX, y), Width = 140, DropDownStyle = WF.ComboBoxStyle.DropDownList };
            var systemTypes = new FilteredElementCollector(_doc).OfClass(typeof(PipingSystemType)).Cast<PipingSystemType>().Select(s => s.Name).ToArray();
            systemTypeCombo.Items.AddRange(systemTypes);
            this.Controls.Add(systemTypeCombo);
            y += spacing;

            // Level
            levelLabel = new WF.Label { Text = "Selected Level:", Location = new SD.Point(labelX, y), AutoSize = true };
            this.Controls.Add(levelLabel);

            levelCombo = new WF.ComboBox { Location = new SD.Point(controlX, y), Width = 140, DropDownStyle = WF.ComboBoxStyle.DropDownList };
            var levels = new FilteredElementCollector(_doc).OfClass(typeof(Level)).Cast<Level>().ToArray();
            levelCombo.Items.AddRange(levels);
            if (levels.Any()) levelCombo.SelectedItem = levels.First();
            this.Controls.Add(levelCombo);
            y += spacing;

            // Pipe Diameter
            diameterLabel = new WF.Label { Text = "Pipe Diameter (mm):", Location = new SD.Point(labelX, y), AutoSize = true };
            this.Controls.Add(diameterLabel);

            diameterTextBox = new WF.TextBox { Location = new SD.Point(controlX, y), Width = 100 };
            var diameterTip = new WF.ToolTip();
            diameterTip.SetToolTip(diameterTextBox, "Example: 80");
            this.Controls.Add(diameterTextBox);
            y += spacing;

            // Custom Offset
            offsetLabel = new WF.Label { Text = "Custom Offset (mm):", Location = new SD.Point(labelX, y), AutoSize = true };
            this.Controls.Add(offsetLabel);

            offsetTextBox = new WF.TextBox { Location = new SD.Point(controlX, y), Width = 100, Text = "2500" };
            this.Controls.Add(offsetTextBox);
            y += spacing;

            // Offset Quick Options
            offsetOptionsLabel = new WF.Label { Text = "Or Select Offset:", Location = new SD.Point(labelX, y), AutoSize = true };
            this.Controls.Add(offsetOptionsLabel);

            offset2200 = new WF.RadioButton { Text = "2200", Location = new SD.Point(controlX, y), AutoSize = true };
            offset2300 = new WF.RadioButton { Text = "2300", Location = new SD.Point(controlX + 70, y), AutoSize = true };
            offset2400 = new WF.RadioButton { Text = "2400", Location = new SD.Point(controlX + 140, y), AutoSize = true };

            offset2200.CheckedChanged += OffsetRadio_CheckedChanged;
            offset2300.CheckedChanged += OffsetRadio_CheckedChanged;
            offset2400.CheckedChanged += OffsetRadio_CheckedChanged;

            this.Controls.Add(offset2200);
            this.Controls.Add(offset2300);
            this.Controls.Add(offset2400);
            y += spacing + 10;

            // Submit Button
            submitButton = new WF.Button { Text = "Submit", Location = new SD.Point(100, y), Width = 80, BackColor = SD.Color.LightGray };
            submitButton.Click += SubmitButton_Click;
            this.Controls.Add(submitButton);

            // Cancel Button
            cancelButton = new WF.Button { Text = "Cancel", Location = new SD.Point(200, y), Width = 80, BackColor = SD.Color.LightGray };
            cancelButton.Click += (s, e) => this.Close();
        }

        private void OffsetRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (offset2200.Checked) offsetTextBox.Text = "2200";
            if (offset2300.Checked) offsetTextBox.Text = "2300";
            if (offset2400.Checked) offsetTextBox.Text = "2400";
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(diameterTextBox.Text) ||
                pipeTypeCombo.SelectedItem == null ||
                systemTypeCombo.SelectedItem == null ||
                levelCombo.SelectedItem == null)
            {
                WF.MessageBox.Show("Please fill in all required fields.", "Warning", WF.MessageBoxButtons.OK, WF.MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = WF.DialogResult.OK;
            this.Close();
        }
    }
}

