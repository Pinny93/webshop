using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ShopBase.Model;
using ShopBase.Tools;
using WPFDesktopApp.Misc;
using WPFDesktopApp.Tools;

namespace WPFDesktopApp
{
    /// <summary>
    /// Interaktionslogik für ArtikelDetailDlg.xaml
    /// </summary>
    public partial class ArtikelDetailDlg : Window
    {
        private Artikel _artikel;
        private BitmapImage? _articleImage;
        private bool _imageChanged;

        private ArtikelDetailDlg(Artikel art, EditMode mode)
        {
            InitializeComponent();

            _artikel = art;
            this.EditMode = mode;

            this.SetFieldsFromModel();
        }

        public ArtikelDetailDlg(Artikel art)
                    : this(art, EditMode.Edit)
        {
            this.Title = "Artikel bearbeiten";
        }

        public ArtikelDetailDlg()
            : this(new Artikel(), EditMode.Create)
        {
            this.Title = "Artikel erstellen";
        }

        public Artikel Artikel
        {
            get
            {
                this.SetModelFromFields();
                return _artikel;
            }
            set
            {
                _artikel = value;
                this.SetFieldsFromModel();
            }
        }

        private bool Validate()
        {
            if (String.IsNullOrWhiteSpace(_tbName.Text))
            {
                MessageBox.Show(this, "Bitte einen Artikelnamen eingeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (_tbPrice.Text.To<decimal>() <= 0)
            {
                MessageBox.Show(this, "Bitte einen gültigen Preis eingeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void SetModelFromFields()
        {
            _artikel.Bezeichnung = _tbName.Text;
            _artikel.Beschreibung = _tbDesc.Text;
            _artikel.Kundenbewertung = (int)_slRating.Value;
            _artikel.Preis = _tbPrice.Text.To<decimal>();

            // Convert Image to Bytes
            if (_imageChanged)
            {
                if (_articleImage != null)
                {
                    _artikel.Image = new Image()
                    {
                        ImageData = _articleImage.GetBytes(new PngBitmapEncoder()),
                        DataType = "image/png",
                        Type = DbImageType.ArticleImage,
                    };
                }
                else
                {
                    _artikel.Image = null;
                }
                _imageChanged = false;
            }
        }

        private void SetFieldsFromModel()
        {
            _tbId.Text = this.EditMode == EditMode.Create ? "(auto)" : _artikel.Id + "";
            _tbName.Text = _artikel.Bezeichnung;
            _tbDesc.Text = _artikel.Beschreibung;
            _slRating.Value = _artikel.Kundenbewertung;
            _tbPrice.Text = _artikel.Preis.ToString("N2");

            if (_artikel.Image?.ImageData != null)
            {
                _articleImage = ImageExtensions.LoadImage(_artikel.Image.ImageData);
                _imgArticleImage.Source = _articleImage;
            }
        }

        private void OnBtOk_Click(object sender, RoutedEventArgs e)
        {
            if (!this.Validate()) { return; }

            this.SetModelFromFields();
            this.DialogResult = true;
        }

        private void OnBtCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public EditMode EditMode { get; }

        private void _slRating_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _lbRating.Content = $"{_slRating.Value:0}/5";
        }

        private void OnBtUploadTest_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Title = "Bild auswählen...",
                Filter = "Unterstützte Bilder (*.png, *.jpg)|*.png;*.jpg",
            };

            if (!dlg.ShowDialog(this) == true)
            {
                return;
            }

            // TODO: Esure Picture is not too big and scale it down...

            _articleImage = new BitmapImage(new Uri(dlg.FileName, UriKind.Absolute));
            _imgArticleImage.Source = _articleImage;

            _imageChanged = true;
        }
    }
}