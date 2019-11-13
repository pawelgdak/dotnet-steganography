using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

namespace Steganografia
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Otwieranie okna dialogowego, w którym
            // wybieramy obrazek

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Wybierz obraz";
            fileDialog.Filter = "Bitmapa (*.bmp)|*.bmp";

            if(fileDialog.ShowDialog() == true)
            {
                // Ustawianie źródła naszego elementu Image
                // na wybrany obrazek
                originalImage.Source = new BitmapImage(new Uri(fileDialog.FileName));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Sprawdzamy czy jest jakiś obrazek ustawiony
            if(originalImage.Source != null)
            {
                // Tworzymy bitmapę, którą będziemy modyfikować
                BitmapSource modifiedImageBmp = new BitmapImage(new Uri(originalImage.Source.ToString()));

                // Skok i rozmiar tablicy, w której będą
                // przechowywane piksele z obrazka
                int stride = modifiedImageBmp.PixelWidth * 4;
                int size = stride * modifiedImageBmp.PixelHeight;

                // Zamiana tekstu na ascii, a potem na ciąg bitów
                byte[] textAscii = Encoding.ASCII.GetBytes(textInput.Text);
                string bits = "";
                for(int i = 0; i < textAscii.Length; i++)
                {
                    bits += Convert.ToString(textAscii[i], 2).PadLeft(8, '0');
                }

                // Tablica, w której będziemy trzymać piksele
                byte[] pixels = new byte[size];

                // Kopiujemy piksele z obrazka do tablicy
                modifiedImageBmp.CopyPixels(pixels, stride, 0);

                // Dwie pętle, dzięki którym możemy poruszać się
                // po naszych pikselach
                for(int y = 0; y < modifiedImageBmp.PixelHeight; y++)
                {
                    for(int x = 0; x < modifiedImageBmp.PixelWidth; x++)
                    {
                        // Kolory w pikselu
                        int index = y * stride + 4 * x;
                        int b = pixels[index];
                        int g = pixels[index + 1];
                        int r = pixels[index + 2];

                        // Zerujemy najmniej istotny bit w każdym kolorze
                        b = b - b % 2;
                        g = g - g % 2;
                        r = r - r % 2;

                        // Wsadzamy zmodyfikowane kolory ponownie do tablicy
                        pixels[index] = (byte)b;
                        pixels[index + 1] = (byte)g;
                        pixels[index + 2] = (byte)r;
                    }
                }

                // Tworzymy nową bitmape, do której wstawimy zmodyfikowane piksele
                WriteableBitmap bmp = new WriteableBitmap(
                    modifiedImageBmp.PixelWidth,
                    modifiedImageBmp.PixelHeight,
                    modifiedImageBmp.DpiX,
                    modifiedImageBmp.DpiY,
                    modifiedImageBmp.Format, null
                );

                // Wklejamy zmodyfikowane piksele
                bmp.WritePixels(
                    new Int32Rect(0, 0, modifiedImageBmp.PixelWidth, modifiedImageBmp.PixelHeight),
                    pixels, stride, 0
                );

                // Ustawiamy źródło modyfikowanego obrazu
                // na utworzoną bitmape 
                modifiedImage.Source = bmp;
            }
        }
    }
}
