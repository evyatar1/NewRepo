using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace DBMoviesManager
{
    public partial class SearchOscarMovieYear : Window
    {
        public SearchOscarMovieYear()
        {
            InitializeComponent();
        }
        private void cleanOldData()
        {
            tbDirector.Text = "";
            tbYear.Text = "";
            tbCountry.Text = "";
            tbScore.Text = "";
            lbActors.ItemsSource = null;
            lbMovies.ItemsSource = null;
        }

        private void lbMovies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbMovies.SelectedItem == null)
                return;
            Movie selectedMovie = lbMovies.SelectedItem as Movie;
            try
            {
                using (var ctx = new ManageMoviesContext())
                {
                    Movie movie = (from m in ctx.Movies
                                   where m.MovieSerial == selectedMovie.MovieSerial
                                   select m).First();
                    tbYear.Text = movie.Year.ToString();
                    tbScore.Text = movie.ImdbScore.ToString() + "/10";
                    ICollection<Director> director = (from d in ctx.Directors
                                                      where d.Id == movie.DirectorId
                                                      select d).ToArray();
                    tbDirector.Text = (director.Count != 0) ? (director.First().FirstName + " " + director.First().LastName) : "Not Fill";
                    tbCountry.Text = movie.Country != null ? movie.Country : "Not Fill";
                    var Actors = (from am in ctx.ActorMovie
                                  where am.MovieSerial == movie.MovieSerial
                                  select am.Actor).ToList();
                    lbActors.ItemsSource = Actors;
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            cleanOldData();
            if (string.IsNullOrEmpty(tbYearSearch.Text.Trim()) || int.TryParse(tbYearSearch.Text.Trim(), out int wantedYear) == false) {
                MessageBox.Show("must fill a year");
                return;
            }
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    lbMovies.ItemsSource = (from o in context.Oscars
                                            where o.Year == wantedYear
                                            select o.MovieSerialNavigation).ToList();
             
                }
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType().ToString());
            }
        }
    }
}
