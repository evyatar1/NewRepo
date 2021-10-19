using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DBMoviesManager
{
    /// <summary>
    /// Interaction logic for AddMovieWindow.xaml
    /// </summary>
    public partial class AddMovieWindow : Window
    {
        public Movie Movie { get; set; }
        public bool FromOscar { get; internal set; }

        public AddMovieWindow()
        {
            InitializeComponent();
        }

        private void AddMovieButton_Click(object sender, RoutedEventArgs e)
        {
            Movie = null;
            if (!AllTextboxesFilled())
            {
                MessageBox.Show("You must provide all the data");
                return;
            }
            try
            {
                String MovieTitle = tbMovieTitle.Text.Trim();
                if (!isTitleVaild(MovieTitle)) throw new ValidationException("Title is not in the correct format.Regex:^[A-Z][a-zA-Z,0-9 ]*$");
                int MovieYear = int.Parse(tbMovieYear.Text.Trim());
                if (!isYearVaild(MovieYear)) throw new ValidationException("Year is not in the correct format(1900 - 2020)");
                String MovieCountry = tbMovieCountry.Text.Trim();
                if (!isNameVaild(MovieCountry)) throw new ValidationException("Country is not in the correct format.Regex:^[A-Z][A-Za-z]*(\\s+[A-Z][A-Za-z]*){0,3}$");
                decimal imdbScore = Decimal.Parse(tbImdbRate.Text.Trim());
                if (!isIMDBScoreVaild(imdbScore)) throw new ValidationException("imdb score is not in the correct format.positive and less then 10");


                Movie = new Movie
                {
                    Title = MovieTitle,
                    Year = MovieYear,
                    Country = MovieCountry,
                    ImdbScore = imdbScore
                };
                using (var ctx = new ManageMoviesContext())
                {
                    ctx.Movies.Add(Movie);
                    ctx.SaveChanges();
                    resetFields();
                    MessageBox.Show($"The movie {MovieTitle} added to database");
                    MessageBox.Show($"You can go to Connect tab to add director and actors to movie");

                    if (FromOscar) Close();

                }
            }
            catch (FormatException)
            {
                MessageBox.Show("data is not in the correct format");
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType().ToString());
            }
        }

        private void resetFields()
        {
            tbMovieTitle.Text = "";
            tbMovieYear.Text = "";
            tbMovieCountry.Text = "";
            tbImdbRate.Text = "";
            
        }

        private static Regex iMDBScoreRegex = new Regex(@"^((10)|(\d{1}\.\d{1})|(\d{1}))?$");
        private static Regex yearRegex = new Regex(@"^19\d{2}|20[01]\d{1}|2020|2021$");
        private static Regex titleRegex = new Regex(@"^[A-Z][a-zA-Z,0-9 ]*$");
        private static Regex nameRegex = new Regex(@"^[A-Z][A-Za-z]*(\s+[A-Z][A-Za-z]*){0,3}$");
        public static bool isNameVaild(string value)
        {
            return nameRegex.IsMatch(value);
        }
        public static bool isTitleVaild(string value)
        {
            return titleRegex.IsMatch(value);
        }
        public static bool isYearVaild(int value)
        {
            return yearRegex.IsMatch(value.ToString());
        }
        public static bool isIMDBScoreVaild(decimal value)
        {
            return iMDBScoreRegex.IsMatch(value.ToString());
        }

        private bool AllTextboxesFilled()
        {
            foreach (var item in mainGrid.Children)
            {
                if (item is TextBox)
                {
                    if (string.IsNullOrEmpty((item as TextBox).Text))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
