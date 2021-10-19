using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Data.Common;

namespace DBMoviesManager
{
    public partial class SearchMoviesByYearWindow : Window
    {
        public SearchMoviesByYearWindow()
        {
            InitializeComponent();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!AllTextboxesFilled())
            {
                MessageBox.Show("You must provide at least one year");
                return;
            }
  
            if (int.TryParse(tblowerYear.Text.Trim(), out int lowerYear) == false && tblowerYear.Text.Trim() == "") lowerYear = 1900;
            if (int.TryParse(tbUpperYear.Text.Trim(), out int UpperYear) == false && tbUpperYear.Text.Trim() == "") UpperYear = 2020;
            
            if(lowerYear==0 || UpperYear==0 || lowerYear> UpperYear)
            {
                lbMovies.ItemsSource = null;
                MessageBox.Show("Upper or lower contain a wrong year");
                return;
            }

            try
            {
                using (var context = new ManageMoviesContext())
                {
                    lbMovies.ItemsSource = (from m in context.Movies
                                            where m.Year>= lowerYear &&m.Year<= UpperYear
                                            select m).ToList();
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
    }
}
