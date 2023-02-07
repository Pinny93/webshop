using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using ShopBase;
using ShopBase.Model;
using ShopBase.Persistence;
using ShopBase.Tools;

namespace WPFDesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.UpdateControlState();
        }

        private void OnMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IEnumerable<Artikel> art = Artikel.ReadAll();
            if (!art.Any())
            {
                art = Artikel.GetSample();
                foreach (var curArt in art)
                {
                    curArt.Create();
                }
            }

            _lbArtikel.Items.AddRange(art);

            _lbBestellungen.Items.AddRange(Bestellung.ReadAll());

            _cbBestStatus.Items.AddRange(Enum.GetValues<BestellStatus>());
        }

        private void OnBtNew_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ArtikelDetailDlg();
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
            {
                dlg.Artikel.Create();
                _lbArtikel.Items.Add(dlg.Artikel);
            }
        }

        private void OnLbAtikel_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.UpdateControlState();
        }

        private void UpdateControlState()
        {
            _btDelete.IsEnabled = _lbArtikel.SelectedItem != null;
            _btUpdate.IsEnabled = _lbArtikel.SelectedItem != null;

            _cbBestStatus.IsEnabled = _lbBestellungen.SelectedItem != null;
        }

        private void OnBtUpdate_Click(object sender, RoutedEventArgs e)
        {
            Artikel? selectedArt = _lbArtikel.SelectedItem as Artikel;
            if (selectedArt == null) { return; }

            var dlg = new ArtikelDetailDlg(selectedArt);
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
            {
                dlg.Artikel.Update();
                _lbArtikel.Items.Refresh();
            }
        }

        private void OnBtDelete_Click(object sender, RoutedEventArgs e)
        {
            Artikel? selectedArt = _lbArtikel.SelectedItem as Artikel;
            if (selectedArt == null) { return; }

            TaskDialog dlg = new();
            dlg.Caption = App.Current.Title;
            dlg.Text = "Möchten Sie diesen Artikel wirklich löschen? Diese Aktion kann nicht rückgängig gemacht werden.";
            dlg.StandardButtons = TaskDialogStandardButtons.None;
            dlg.InstructionText = "Sind Sie sicher?";
            dlg.Icon = TaskDialogStandardIcon.Warning;
            dlg.FooterText = "Durch die SureDelete™-Technologie werden sämtliche Überreste des Artikels feinsäuberlich entfernt.";
            dlg.FooterIcon = TaskDialogStandardIcon.Information;
            dlg.FooterCheckBoxText = "Den Artikel wirklich löschen und nicht nur so tun.";
            dlg.OwnerWindowHandle = new WindowInteropHelper(this).Handle;

            var btnJa = new TaskDialogButton("yes", "Ich kenne das Risiko");
            btnJa.Click += (sender, e) => dlg.Close(TaskDialogResult.Yes);
            dlg.Controls.Add(btnJa);

            var btnNein = new TaskDialogButton("no", "Hilfe! Sofort abbrechen!");
            btnNein.Click += (sender, e) => dlg.Close(TaskDialogResult.No);
            dlg.Controls.Add(btnNein);

            //dlg.Tick += (sender, e) => { btnJa.Enabled = dlg.FooterCheckBoxChecked == true; };

            if (dlg.Show() != TaskDialogResult.Yes) { return; }

            _lbArtikel.Items.Remove(selectedArt);

            if (dlg.FooterCheckBoxChecked == true) { selectedArt.Delete(); }
        }

        private void OnBtnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Artikel exportieren...";
            dlg.Filter = "CSV-Dateien (*.csv)|*.csv";

            if (dlg.ShowDialog(this) != true) { return; }

            var allArtikel = Artikel.ReadAll();
            CSVArtikel.Export(allArtikel, dlg.FileName);

            TaskDialog successDlg = new();
            successDlg.Caption = App.Current.Title;
            successDlg.InstructionText = "Export erfolgreich";
            successDlg.Icon = TaskDialogStandardIcon.Information;
            successDlg.OwnerWindowHandle = new WindowInteropHelper(this).Handle;
            successDlg.Show();
        }

        private void OnBtnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Artikel importieren...";
            dlg.Filter = "CSV-Dateien (*.csv)|*.csv";

            if (dlg.ShowDialog(this) != true) { return; }

            bool keepCurrent = true;
            if (_lbArtikel.Items.Count > 0)
            {
                TaskDialog successDlg = new();
                successDlg.Caption = App.Current.Title;
                successDlg.InstructionText = "Vorhandene Artikel löschen?";
                successDlg.Text = "Sollen die vorhanden Artikel gelöscht oder beibehalten werden?";
                successDlg.Icon = TaskDialogStandardIcon.Warning;
                successDlg.OwnerWindowHandle = new WindowInteropHelper(this).Handle;

                var btnJa = new TaskDialogButton("yes", "Artikel beibehalten");
                btnJa.Click += (sender, e) => successDlg.Close(TaskDialogResult.Yes);
                successDlg.Controls.Add(btnJa);

                var btnNein = new TaskDialogButton("no", "Artikel löschen");
                btnNein.Click += (sender, e) => successDlg.Close(TaskDialogResult.No);
                successDlg.Controls.Add(btnNein);

                var btnCncl = new TaskDialogButton("cancel", "Hilfe! Sofort abbrechen!");
                btnCncl.Click += (sender, e) => successDlg.Close(TaskDialogResult.Cancel);
                successDlg.Controls.Add(btnCncl);

                var res = successDlg.Show();
                if (res == TaskDialogResult.Cancel) { return; }

                keepCurrent = res == TaskDialogResult.Yes;
            }

            if (!keepCurrent)
            {
                foreach (Artikel curArtikel in _lbArtikel.Items)
                {
                    curArtikel.Delete();
                }

                _lbArtikel.Items.Clear();
            }

            foreach (var curArtikel in CSVArtikel.Import(dlg.FileName))
            {
                // Check if already in DB
                if (Artikel.TryGetById(curArtikel.Id) == null)
                {
                    curArtikel.Create();
                }

                _lbArtikel.Items.Add(curArtikel);
            }
        }

        private void OnLbArtikel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnBtUpdate_Click(sender, e);
        }

        private void OnLbBest_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _lbPositions.Items.Clear();

            Bestellung? selectedBest = _lbBestellungen.SelectedItem as Bestellung;
            if (selectedBest == null)
            {
                return;
            }

            _lbPositions.Items.AddRange(selectedBest.Positionen);
            _cbBestStatus.SelectedItem = selectedBest.Status;

            this.UpdateControlState();
        }

        private void OnCbBestStatus_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Bestellung? selectedBest = _lbBestellungen.SelectedItem as Bestellung;
            if (selectedBest == null || selectedBest.Status == (BestellStatus)_cbBestStatus.SelectedItem)
            {
                return;
            }

            // Abfrage: User soll
            bool moralpredigtGelesen = false;
            do
            {
                TaskDialog dlg = new();
                dlg.Caption = App.Current.Title;
                dlg.Text = "WARNUNG: Durch drücken auf 'Ja' wird der Status in der Datenbank geändert." + Environment.NewLine +
                    "Dies kann dazu führen, dass einem ahnungslosen User die Bestellung unter den allerwertesten weggezogen wird." + Environment.NewLine + Environment.NewLine +
                    "Möchten Sie WIRKLICH GANZ SICHER fortfahren?";
                dlg.StandardButtons = TaskDialogStandardButtons.None;
                dlg.InstructionText = "Neuen Zustand in die Datenbank schreiben?";
                dlg.Icon = TaskDialogStandardIcon.Warning;
                dlg.FooterText = "Haben Sie ein Herz und ärgern Sie bitte keine unschuldigen Kunden! Sie zahlen auch dein Gehalt!";
                dlg.FooterIcon = TaskDialogStandardIcon.Information;
                dlg.FooterCheckBoxText = "Ich habe die Moralpredigt durchgelesen";

                var btnJa = new TaskDialogButton("yes", "Ja, Feuer frei!");
                btnJa.Click += (sender, e) => dlg.Close(TaskDialogResult.Yes);
                dlg.Controls.Add(btnJa);

                var btnNein = new TaskDialogButton("no", "Oh nein, das wollte ich nicht.");
                btnNein.Click += (sender, e) => dlg.Close(TaskDialogResult.No);
                dlg.Controls.Add(btnNein);
                dlg.OwnerWindowHandle = new WindowInteropHelper(this).Handle;

                if (dlg.Show() == TaskDialogResult.No)
                {
                    _cbBestStatus.SelectedItem = selectedBest.Status;
                    return;
                }

                moralpredigtGelesen = dlg.FooterCheckBoxChecked == true;
                if (!moralpredigtGelesen)
                {
                    MessageBox.Show(this, "LESEN Sie bitte die Moralpredigt durch!", App.Current.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } while (!moralpredigtGelesen);
            selectedBest.Status = (BestellStatus)_cbBestStatus.SelectedItem;
            selectedBest.Update();

            _lbBestellungen.Items.Refresh();
        }

        private void OnBtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _lbBestellungen.Items.Clear();
            _lbBestellungen.Items.AddRange(Bestellung.ReadAll());
        }
    }
}