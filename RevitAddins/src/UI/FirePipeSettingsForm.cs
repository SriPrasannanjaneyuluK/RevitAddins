using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing; // for GUI
using Autodesk.Revit.DB;

namespace RevitAddins.UI
{
    public class FirePipeSettingsForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label elementLabel;
        private System.Windows.Forms.TextBox elementTextBox;

        private System.Windows.Forms.Label layerLabel;
        private System.Windows.Forms.ComboBox layerCombo;

        private System.Windows.Forms.Label pipeTypeLabel;
        private System.Windows.Forms.ComboBox pipeTypeCombo;

        private System.Windows.Forms.Label systemTypeLabel;
        private System.Windows.Forms.ComboBox systemTypeCombo;

        private System.Windows.Forms.Label levelLabel;
        private System.Windows.Forms.ComboBox levelCombo;

        private System.Windows.Forms.Label diameterLabel;
        private System.Windows.Forms.TextBox diameterTextBox;

        private System.Windows.Forms.Label offsetLabel;
        private System.Windows.Forms.TextBox offsetTextBox;

        private System.Windows.Forms.Label offsetOptionsLabel;
        private System.Windows.Forms.RadioButton offset2200;
        private System.Windows.Forms.RadioButton offset2300;
        private System.Windows.Forms.RadioButton offset2400;

        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.Button cancelButton;

        private Document _doc;

        public double DefaultDiameter { get; set; } = 80;
        public double DefaultOffset { get; set; } = 2500;

        public string SelectedLayer => layerCombo.SelectedItem?.ToString();
        public string SelectedPipeType => pipeTypeCombo.SelectedItem?.ToString();
        public string SelectedSystemType => systemTypeCombo.SelectedItem?.ToString();
        public Level SelectedLevel => levelCombo.SelectedItem as Level;
        public double Diameter => double.TryParse(diameterTextBox.Text, out double d) ? d : 0;
        public double Offset => double.TryParse(offsetTextBox.Text, out double o) ? o : 0;

        public FirePipeSettingsForm(Document doc, string cadElementId, string[] layers, string[] pipeTypes, string[] systemTypes, Level[] levels)
        {
            _doc = doc;
            InitializeComponent();

            elementTextBox.Text = cadElementId;
            elementTextBox.ReadOnly = true;

            layerCombo.Items.AddRange(layers);
            if (layers.Any()) layerCombo.SelectedIndex = 0;

            pipeTypeCombo.Items.AddRange(pipeTypes);
            if (pipeTypes.Any()) pipeTypeCombo.SelectedIndex = 0;

            systemTypeCombo.Items.AddRange(systemTypes);
            if (systemTypes.Any()) systemTypeCombo.SelectedIndex = 0;

            levelCombo.Items.AddRange(levels);
            if (levels.Any()) levelCombo.SelectedItem = levels.First();

            diameterTextBox.Text = DefaultDiameter.ToString();
            offsetTextBox.Text = DefaultOffset.ToString();
        }

        private void InitializeComponent()
        {
            this.Text = "Pipe Input";
            this.Size = new System.Drawing.Size(400, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.WhiteSmoke;

            int labelX = 30;
            int controlX = 200;
            int y = 20;
            int spacing = 40;

            // CAD Element ID
            elementLabel = new System.Windows.Forms.Label { Text = "Selected Element ID:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            elementTextBox = new System.Windows.Forms.TextBox { Location = new System.Drawing.Point(controlX, y), Width = 140, ReadOnly = true };
            this.Controls.Add(elementLabel);
            this.Controls.Add(elementTextBox);
            y += spacing;

            // Layer
            layerLabel = new System.Windows.Forms.Label { Text = "Select Layer:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            layerCombo = new System.Windows.Forms.ComboBox { Location = new System.Drawing.Point(controlX, y), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(layerLabel);
            this.Controls.Add(layerCombo);
            y += spacing;

            // Pipe Type
            pipeTypeLabel = new System.Windows.Forms.Label { Text = "Select Pipe:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            pipeTypeCombo = new System.Windows.Forms.ComboBox { Location = new System.Drawing.Point(controlX, y), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(pipeTypeLabel);
            this.Controls.Add(pipeTypeCombo);
            y += spacing;

            // System Type
            systemTypeLabel = new System.Windows.Forms.Label { Text = "Select System Pipe Type:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            systemTypeCombo = new System.Windows.Forms.ComboBox { Location = new System.Drawing.Point(controlX, y), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(systemTypeLabel);
            this.Controls.Add(systemTypeCombo);
            y += spacing;

            // Level
            levelLabel = new System.Windows.Forms.Label { Text = "Selected Level:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            levelCombo = new System.Windows.Forms.ComboBox { Location = new System.Drawing.Point(controlX, y), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(levelLabel);
            this.Controls.Add(levelCombo);
            y += spacing;

            // Pipe Diameter
            diameterLabel = new System.Windows.Forms.Label { Text = "Pipe Diameter (mm):", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            diameterTextBox = new System.Windows.Forms.TextBox { Location = new System.Drawing.Point(controlX, y), Width = 100 };
            var diameterTip = new System.Windows.Forms.ToolTip();
            diameterTip.SetToolTip(diameterTextBox, "Enter diameter in mm only");
            this.Controls.Add(diameterLabel);
            this.Controls.Add(diameterTextBox);
            y += spacing;

            // Custom Offset
            offsetLabel = new System.Windows.Forms.Label { Text = "Custom Offset (mm):", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            offsetTextBox = new System.Windows.Forms.TextBox { Location = new System.Drawing.Point(controlX, y), Width = 100 };
            this.Controls.Add(offsetLabel);
            this.Controls.Add(offsetTextBox);
            y += spacing;

            // Offset Quick Options
            offsetOptionsLabel = new System.Windows.Forms.Label { Text = "Or Select Offset:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            offset2200 = new System.Windows.Forms.RadioButton { Text = "2200", Location = new System.Drawing.Point(controlX, y), AutoSize = true };
            offset2300 = new System.Windows.Forms.RadioButton { Text = "2300", Location = new System.Drawing.Point(controlX + 70, y), AutoSize = true };
            offset2400 = new System.Windows.Forms.RadioButton { Text = "2400", Location = new System.Drawing.Point(controlX + 140, y), AutoSize = true };

            offset2200.CheckedChanged += OffsetRadio_CheckedChanged;
            offset2300.CheckedChanged += OffsetRadio_CheckedChanged;
            offset2400.CheckedChanged += OffsetRadio_CheckedChanged;

            this.Controls.Add(offsetOptionsLabel);
            this.Controls.Add(offset2200);
            this.Controls.Add(offset2300);
            this.Controls.Add(offset2400);
            y += spacing + 10;

            // Submit & Cancel
            submitButton = new System.Windows.Forms.Button { Text = "Submit", Location = new System.Drawing.Point(100, y), Width = 80, BackColor = System.Drawing.Color.LightGray };
            cancelButton = new System.Windows.Forms.Button { Text = "Cancel", Location = new System.Drawing.Point(200, y), Width = 80, BackColor = System.Drawing.Color.LightGray };

            submitButton.Click += SubmitButton_Click;
            cancelButton.Click += (s, e) =>
            {
                if (System.Windows.Forms.MessageBox.Show("Cancel operation?", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    this.Close();
            };

            this.Controls.Add(submitButton);
            this.Controls.Add(cancelButton);
        }

        private void OffsetRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (offset2200.Checked) offsetTextBox.Text = "2200";
            if (offset2300.Checked) offsetTextBox.Text = "2300";
            if (offset2400.Checked) offsetTextBox.Text = "2400";
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(diameterTextBox.Text, out double d) || d <= 0)
            {
                System.Windows.Forms.MessageBox.Show("Enter valid diameter (>0 mm).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (layerCombo.SelectedItem == null || pipeTypeCombo.SelectedItem == null || systemTypeCombo.SelectedItem == null || levelCombo.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Please fill all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
