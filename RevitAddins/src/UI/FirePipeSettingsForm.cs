using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Autodesk.Revit.DB;
using RevitAddins.Helpers;
using System.Collections.Generic;

namespace RevitAddins.UI
{
    public class FirePipeSettingsForm : System.Windows.Forms.Form
    {
        private Label elementLabel;
        private TextBox elementTextBox;

        private Label layerLabel;
        private ComboBox layerCombo;

        private Label pipeTypeLabel;
        private ComboBox pipeTypeCombo;

        private Label systemTypeLabel;
        private ComboBox systemTypeCombo;

        private Label levelLabel;
        private ComboBox levelCombo;

        private Label diameterLabel;
        private TextBox diameterTextBox;

        private Label offsetLabel;
        private TextBox offsetTextBox;

        private Label offsetOptionsLabel;
        private RadioButton offset2200;
        private RadioButton offset2300;
        private RadioButton offset2400;

        private Button submitButton;
        private Button cancelButton;

        private Document _doc;

        // dictionaries
        private Dictionary<string, ElementId> _levelDict;
        private Dictionary<string, ElementId> _pipeDict;

        public double DefaultDiameter { get; set; } = 80;
        public double DefaultOffset { get; set; } = 2500;

        public string SelectedLayer => layerCombo.SelectedItem?.ToString();
        public string SelectedPipeTypeName => pipeTypeCombo.SelectedItem?.ToString();
        public string SelectedSystemType => systemTypeCombo.SelectedItem?.ToString();

        public ElementId SelectedPipeTypeId
        {
            get
            {
                if (SelectedPipeTypeName != null && _pipeDict.ContainsKey(SelectedPipeTypeName))
                    return _pipeDict[SelectedPipeTypeName];
                return ElementId.InvalidElementId;
            }
        }

        public ElementId SelectedLevelId
        {
            get
            {
                string key = levelCombo.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(key) && _levelDict.ContainsKey(key))
                    return _levelDict[key];
                return ElementId.InvalidElementId;
            }
        }

        public double Diameter => double.TryParse(diameterTextBox.Text, out double d) ? d : 0;
        public double Offset => double.TryParse(offsetTextBox.Text, out double o) ? o : 0;

        /// <summary>
        /// Constructor
        /// - doc: current document
        /// - cadElementId: string name for the selected CAD (displayed in read-only textbox)
        /// - layers: array of cad layers (strings)
        /// - pipeDict: dictionary of displayName -> ElementId (from PipeUtils)
        /// - systemTypes: array of piping system type names
        /// </summary>
        public FirePipeSettingsForm(Document doc, string cadElementId, string[] layers,
                                    Dictionary<string, ElementId> pipeDict)
        {
            _doc = doc;
            InitializeComponent();

            elementTextBox.Text = cadElementId ?? "Unknown";
            elementTextBox.ReadOnly = true;

            // layers
            layerCombo.Items.AddRange(layers ?? new string[0]);
            if (layerCombo.Items.Count > 0) layerCombo.SelectedIndex = 0;

            // pipes
            _pipeDict = pipeDict ?? new Dictionary<string, ElementId>();
            pipeTypeCombo.Items.AddRange(_pipeDict.Keys.ToArray());
            if (pipeTypeCombo.Items.Count > 0) pipeTypeCombo.SelectedIndex = 0;

            string[] systemTypeNames = SystemTypeUtils.GetSystemTypeNames(_doc);
            systemTypeCombo.Items.AddRange(systemTypeNames);
            if (systemTypeCombo.Items.Count > 0) systemTypeCombo.SelectedIndex = 0;


            // levels via helper
            _levelDict = LevelUtils.GetLevelDisplayNames(_doc) ?? new Dictionary<string, ElementId>();
            levelCombo.Items.AddRange(_levelDict.Keys.ToArray());

            // preselect active view level if available
            var activeLevelId = LevelUtils.GetActiveViewLevelId(_doc);
            if (activeLevelId != ElementId.InvalidElementId)
            {
                var match = _levelDict.FirstOrDefault(kvp => kvp.Value == activeLevelId);
                if (!string.IsNullOrEmpty(match.Key))
                    levelCombo.SelectedItem = match.Key;
                else if (levelCombo.Items.Count > 0)
                    levelCombo.SelectedIndex = 0;
            }
            else if (levelCombo.Items.Count > 0)
            {
                levelCombo.SelectedIndex = 0;
            }

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
            elementLabel = new Label { Text = "Selected Element ID:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            elementTextBox = new TextBox { Location = new System.Drawing.Point(controlX, y), Width = 160, ReadOnly = true };
            this.Controls.Add(elementLabel);
            this.Controls.Add(elementTextBox);
            y += spacing;

            // Layer
            layerLabel = new Label { Text = "Select Layer:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            layerCombo = new ComboBox { Location = new System.Drawing.Point(controlX, y), Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(layerLabel);
            this.Controls.Add(layerCombo);
            y += spacing;

            // Pipe Type
            pipeTypeLabel = new Label { Text = "Select Pipe:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            pipeTypeCombo = new ComboBox { Location = new System.Drawing.Point(controlX, y), Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(pipeTypeLabel);
            this.Controls.Add(pipeTypeCombo);
            y += spacing;

            // System Type
            systemTypeLabel = new Label { Text = "Select System Pipe Type:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            systemTypeCombo = new ComboBox { Location = new System.Drawing.Point(controlX, y), Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(systemTypeLabel);
            this.Controls.Add(systemTypeCombo);
            y += spacing;

            // Level
            levelLabel = new Label { Text = "Select Level:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            levelCombo = new ComboBox { Location = new System.Drawing.Point(controlX, y), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(levelLabel);
            this.Controls.Add(levelCombo);
            y += spacing;

            // Pipe Diameter
            diameterLabel = new Label { Text = "Pipe Diameter (mm):", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            diameterTextBox = new TextBox { Location = new System.Drawing.Point(controlX, y), Width = 100 };
            var diameterTip = new ToolTip();
            diameterTip.SetToolTip(diameterTextBox, "Enter diameter in mm only");
            this.Controls.Add(diameterLabel);
            this.Controls.Add(diameterTextBox);
            y += spacing;

            // Custom Offset
            offsetLabel = new Label { Text = "Custom Offset (mm):", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            offsetTextBox = new TextBox { Location = new System.Drawing.Point(controlX, y), Width = 100 };
            this.Controls.Add(offsetLabel);
            this.Controls.Add(offsetTextBox);
            y += spacing;

            // Offset Quick Options
            offsetOptionsLabel = new Label { Text = "Or Select Offset:", Location = new System.Drawing.Point(labelX, y), AutoSize = true };
            offset2200 = new RadioButton { Text = "2200", Location = new System.Drawing.Point(controlX, y), AutoSize = true };
            offset2300 = new RadioButton { Text = "2300", Location = new System.Drawing.Point(controlX + 70, y), AutoSize = true };
            offset2400 = new RadioButton { Text = "2400", Location = new System.Drawing.Point(controlX + 140, y), AutoSize = true };

            offset2200.CheckedChanged += OffsetRadio_CheckedChanged;
            offset2300.CheckedChanged += OffsetRadio_CheckedChanged;
            offset2400.CheckedChanged += OffsetRadio_CheckedChanged;

            this.Controls.Add(offsetOptionsLabel);
            this.Controls.Add(offset2200);
            this.Controls.Add(offset2300);
            this.Controls.Add(offset2400);
            y += spacing + 10;

            // Submit & Cancel
            submitButton = new Button { Text = "Submit", Location = new System.Drawing.Point(100, y), Width = 80, BackColor = System.Drawing.Color.LightGray };
            cancelButton = new Button { Text = "Cancel", Location = new System.Drawing.Point(200, y), Width = 80, BackColor = System.Drawing.Color.LightGray };

            submitButton.Click += SubmitButton_Click;
            cancelButton.Click += (s, e) =>
            {
                if (MessageBox.Show("Cancel operation?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                MessageBox.Show("Enter valid diameter (>0 mm).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (layerCombo.SelectedItem == null || pipeTypeCombo.SelectedItem == null || systemTypeCombo.SelectedItem == null || levelCombo.SelectedItem == null)
            {
                MessageBox.Show("Please fill all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
