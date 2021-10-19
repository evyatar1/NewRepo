using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

namespace DBMoviesManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            refreshList();
        }

        private void addActorButton(object sender, RoutedEventArgs e)
        {
            AddActorWindow window = new AddActorWindow();
            window.ShowDialog();
            refreshList();
        }

        private void MiAddDirector_Click(object sender, RoutedEventArgs e)
        {
            AddDirectorWindow window = new AddDirectorWindow();
            window.ShowDialog();
            refreshList();
        }

        private void MiAddMovie_Click(object sender, RoutedEventArgs e)
        {
            AddMovieWindow window = new AddMovieWindow();
            window.ShowDialog();
            refreshList();
        }

        private void MiAddOscar_Click(object sender, RoutedEventArgs e)
        {
            AddOscarWindow window = new AddOscarWindow();
            window.ShowDialog();
            refreshList();
        }


        private void miShowActor_Click(object sender, RoutedEventArgs e)
        {
            ShowActorWindow window = new ShowActorWindow();
            window.ShowDialog();
            refreshList();
        }


        private void miShowDirector_Click(object sender, RoutedEventArgs e)
        {
            ShowDirectorsWindow window = new ShowDirectorsWindow();
            window.ShowDialog();
            refreshList();
        }

        private void miShowOscar_Click(object sender, RoutedEventArgs e)
        {
            ShowOscarsWindow window = new ShowOscarsWindow();
            window.ShowDialog();
            refreshList();
        }

        private void miMovieToActorClick(object sender, RoutedEventArgs e)
        {
            ConnectMovieToActorWindow window = new ConnectMovieToActorWindow();
            window.ShowDialog();
            refreshList();
        }

        private void miMovieToActressClick(object sender, RoutedEventArgs e)
        {
            ConnectMovieToActressWindow window = new ConnectMovieToActressWindow();
            window.ShowDialog();
            refreshList();
        }

        private void miMovieToDirectorClick(object sender, RoutedEventArgs e)
        {
            ConnectMovieToDirecator window = new ConnectMovieToDirecator();
            window.ShowDialog();
            refreshList();
        }

        private void miActorToMovieClick(object sender, RoutedEventArgs e)
        {
            ConnectActorToMovieWindow window = new ConnectActorToMovieWindow();
            window.ShowDialog();
            refreshList();
        }

        private void miDirectorToMovieClick(object sender, RoutedEventArgs e)
        {
            ConnectDirectorToMovieWindow window = new ConnectDirectorToMovieWindow();
            window.ShowDialog();
            refreshList();
        }

        private void miActressToMovieClick(object sender, RoutedEventArgs e)
        {
            ConnectActressToMovieWindow window = new ConnectActressToMovieWindow();
            window.ShowDialog();
            refreshList();
        }

        private void refreshList()
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    lbMovies.ItemsSource = (from m in context.Movies
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

        private void lbMovies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbMovies.SelectedItem == null)
                return;
            Movie selectedMovie = lbMovies.SelectedItem as Movie;
            try
            {
                using (var ctx = new ManageMoviesContext())
                {
                    var Movies = ctx.Movies.Include(m => m.Oscar).Include(m=>m.Director).ToList();
                    Movie movie = (from m in Movies
                                   where m.MovieSerial == selectedMovie.MovieSerial
                                   select m).First();
                    tbYear.Text = movie.Year.ToString();
                    tbScore.Text = movie.ImdbScore.ToString()+"/10";
                    tbDirector.Text = (movie.Director!= null)?(movie.Director.FirstName +" "+ movie.Director.LastName):"Not Fill";
                    tbCountry.Text = movie.Country!=null? movie.Country:"Not Fill";
                    var Actors = (from am in ctx.ActorMovie
                                  where am.MovieSerial == movie.MovieSerial
                                  select am.Actor).ToList();
                    lbActors.ItemsSource = Actors;
                    tbOscar.Text = (movie.Oscar != null) ? "Yes" : "No";
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

        private void miMovieYearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchMoviesByYearWindow window = new SearchMoviesByYearWindow();
            window.ShowDialog();
        }

        private void miTwoActorsSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchMoviesByTwoActorsWindow window = new SearchMoviesByTwoActorsWindow();
            window.ShowDialog();
        }

        private void miMovieOscarSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchOscarMovieYear window = new SearchOscarMovieYear();
            window.ShowDialog();
        }
    }
}
