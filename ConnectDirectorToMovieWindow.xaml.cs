using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
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
    public partial class ConnectDirectorToMovieWindow : Window
    {
        public ConnectDirectorToMovieWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            refreshLists();
        }


        private void cbMovie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMovieList((cbMovie.SelectedItem as Movie));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbMovie.SelectedItem == null || lbDirectors.SelectedItem == null)
            {
                MessageBox.Show("You must select a Director and a Movie");
                return;
            }
            try
            {
                Director selectedDirector = lbDirectors.SelectedItem as Director;
                Movie selectedMovie = cbMovie.SelectedItem as Movie;
                Movie movie;
                using (var ctx = new ManageMoviesContext())
                {
                    Director director = (from a in ctx.Directors
                                         where a.Id == selectedDirector.Id
                                         select a).First();

                    movie = (from m in ctx.Movies
                             where m.MovieSerial == selectedMovie.MovieSerial
                             select m).First();
                    movie.Director = director;

                    ctx.SaveChanges();

                }
                UpdateMovieList(movie);
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

        private void UpdateMovieList(Movie selectedMovie)
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    if (selectedMovie == null || selectedMovie.Director == null)
                    {
                        lbDirectors.ItemsSource = null;
                        lbDirectors.ItemsSource = (from d in context.Directors
                                                   select d).ToList();
                        return;
                    }
                    var query = (from d in context.Directors
                                 where d.Id != selectedMovie.DirectorId
                                 select d).ToList();

                    lbDirectors.ItemsSource = query;
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

        private void btnAddNewDirector_Click(object sender, RoutedEventArgs e)
        {
            AddDirectorWindow window = new AddDirectorWindow();
            window.FromOscar = true;

            window.ShowDialog();
            refreshLists();
            UpdateMovieList((cbMovie.SelectedItem as Movie));
        }

        private void btnAddNewMovie_Click(object sender, RoutedEventArgs e)
        {
            AddMovieWindow window = new AddMovieWindow();
            window.FromOscar = true;

            window.ShowDialog();
            refreshLists();
            UpdateMovieList((cbMovie.SelectedItem as Movie));
        }

        private void refreshLists()
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    cbMovie.ItemsSource = (from m in context.Movies
                                           select m).ToList();
                    lbDirectors.ItemsSource = (from d in context.Directors
                                               select d).ToList();
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
