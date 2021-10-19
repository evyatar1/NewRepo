using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Common;

namespace DBMoviesManager
{
    public partial class ConnectMovieToDirecator : Window
    {
        public ConnectMovieToDirecator()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            refreshLists();
        }

        private void cbDirector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
              UpdateMovieList((cbDirector.SelectedItem as Director));
        }

        private void UpdateMovieList(Director selectedDirector)
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    if (selectedDirector == null)
                    {
                        lbMovies.ItemsSource = null;
                        lbMovies.ItemsSource = (from m in context.Movies
                                                select m).ToList();
                        return;
                    }
                    var query = (from m in context.Movies
                                 where m.DirectorId != selectedDirector.Id
                                 select m).ToList();

                    lbMovies.ItemsSource = query;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbDirector.SelectedItem == null || lbMovies.SelectedItem == null)
            {
                MessageBox.Show("You must select a Director and at least one movie");
                return;
            }
            try
            {
                Director selectedDirector = cbDirector.SelectedItem as Director;
                ICollection<Movie> selectedMovies = new List<Movie>();

                foreach (var item in lbMovies.SelectedItems)
                {
                    Movie NewMovie = item as Movie;
                    selectedMovies.Add(NewMovie);
                }
                using (var ctx = new ManageMoviesContext())
                {
                    Director director = (from a in ctx.Directors
                                         where a.Id == selectedDirector.Id
                                         select a).First();

                    foreach (Movie selectedMovie in selectedMovies)
                    {
                        Movie movie = (from m in ctx.Movies
                                       where m.MovieSerial == selectedMovie.MovieSerial
                                       select m).First();
                        movie.Director = director;
                    }
                    ctx.SaveChanges();

                }
                UpdateMovieList(selectedDirector);
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

        private void btnAddNewDirector_Click(object sender, RoutedEventArgs e)
        {
            AddDirectorWindow window = new AddDirectorWindow();
            window.FromOscar = true;

            window.ShowDialog();
            refreshLists();
            UpdateMovieList((cbDirector.SelectedItem as Director));
        }

        private void btnAddNewMovie_Click(object sender, RoutedEventArgs e)
        {
            AddMovieWindow window = new AddMovieWindow();
            window.FromOscar = true;
            window.ShowDialog();

            refreshLists();
            UpdateMovieList((cbDirector.SelectedItem as Director));
        }

        private void refreshLists()
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    cbDirector.ItemsSource = (from a in context.Directors
                                              select a).ToList();
                    lbMovies.ItemsSource = (from c in context.Movies
                                            select c).ToList();
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
