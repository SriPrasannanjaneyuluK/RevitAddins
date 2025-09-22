using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Linq;
using System.Windows.Forms; // WinForms namespace

namespace RevitAddins.UI
{
    public class FirePipeSettingsForm : System.Windows.Forms.Form
    {
        public string SelectedPipeType { get; private set; }
        public string SelectedSystemType { get; private set; }
        public Level SelectedLevel { get; private set; }
        public double Diameter { get; private set; }

        public ComboBox PipeTypeCombo;
        public ComboBox SystemTypeCombo;
        public ComboBox LevelCombo;
        public TextBox DiameterText;
        private Button OkButton;

        public FirePipeSettingsForm(Document doc)
        {
            this.Text = "Fire Pipe Settings";
            this.Width = 350;
            this.Height = 250;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Pipe Type
            Label label1 = new Label() { Text = "Pipe Type:", Left = 20, Top = 20, Width = 100 };
            PipeTypeCombo = new ComboBox() { Left = 120, Top = 20, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            PipeTypeCombo.Items.AddRange(
                new FilteredElementCollector(doc).OfClass(typeof(PipeType))
                .Cast<PipeType>().Select(x => x.Name).ToArray()
            );

            // System Type
            Label label2 = new Label() { Text = "System Type:", Left = 20, Top = 60, Width = 100 };
            SystemTypeCombo = new ComboBox() { Left = 120, Top = 60, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            SystemTypeCombo.Items.AddRange(
                new FilteredElementCollector(doc).OfClass(typeof(PipingSystemType))
                .Cast<PipingSystemType>().Select(x => x.Name).ToArray()
            );

            // Level
            Label label3 = new Label() { Text = "Level:", Left = 20, Top = 100, Width = 100 };
            LevelCombo = new ComboBox() { Left = 120, Top = 100, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            LevelCombo.Items.AddRange(
                new FilteredElementCollector(doc).OfClass(typeof(Level))
                .Cast<Level>().Select(x => x.Name).ToArray()
            );

            // Diameter
            Label label4 = new Label() { Text = "Diameter (mm):", Left = 20, Top = 140, Width = 100 };
            DiameterText = new TextBox() { Left = 120, Top = 140, Width = 180, Text = "100" };

            // OK button
            OkButton = new Button() { Text = "OK", Left = 120, Top = 180, Width = 80 };
            OkButton.Click += (s, e) =>
            {
                if (PipeTypeCombo.SelectedItem == null)
                {
                    MessageBox.Show("Please select a Pipe Type.");
                    return;
                }
                if (SystemTypeCombo.SelectedItem == null)
                {
                    MessageBox.Show("Please select a System Type.");
                    return;
                }
                if (LevelCombo.SelectedItem == null)
                {
                    MessageBox.Show("Please select a Level.");
                    return;
                }
                if (!double.TryParse(DiameterText.Text, out double dia) || dia <= 0)
                {
                    MessageBox.Show("Please enter a valid Diameter greater than 0.");
                    return;
                }

                SelectedPipeType = PipeTypeCombo.SelectedItem.ToString();
                SelectedSystemType = SystemTypeCombo.SelectedItem.ToString();
                SelectedLevel = new FilteredElementCollector(doc).OfClass(typeof(Level))
                    .Cast<Level>().First(x => x.Name == LevelCombo.SelectedItem.ToString());
                Diameter = dia;

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(label1);
            this.Controls.Add(PipeTypeCombo);
            this.Controls.Add(label2);
            this.Controls.Add(SystemTypeCombo);
            this.Controls.Add(label3);
            this.Controls.Add(LevelCombo);
            this.Controls.Add(label4);
            this.Controls.Add(DiameterText);
            this.Controls.Add(OkButton);
        }
    }
}
