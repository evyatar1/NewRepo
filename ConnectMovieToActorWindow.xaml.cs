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

    public partial class ConnectMovieToActorWindow : Window
    {
        public ConnectMovieToActorWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            refreshLists();
        }

        private void refreshLists()
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    cbActor.ItemsSource = (from a in context.Actors
                                           where a.Gender == (int)Gender.Male
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

        private void cbActors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                    UpdateMovieList((cbActor.SelectedItem as Actor));
        }

        private void UpdateMovieList(Actor selectedActor)
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    if (selectedActor == null)
                    {
                        lbMovies.ItemsSource = null;
                        lbMovies.ItemsSource = (from m in context.Movies
                                                select m).ToList();
                        return;
                    }
                    var smallQuery = (from am in context.ActorMovie
                                      where am.ActorId == selectedActor.Id
                                      select am.Movie).ToList();
                    var BigQuery = (from m in context.Movies
                                    select m).ToList();
                    lbMovies.ItemsSource = BigQuery.Except(smallQuery).ToList();
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
            if (cbActor.SelectedItem == null || lbMovies.SelectedItem == null)
            {
                MessageBox.Show("You must select a Actor and at least one movie");
                return;
            }
            try
            {
                Actor selectedActor = cbActor.SelectedItem as Actor;
                ICollection<Movie> selectedMovies = new List<Movie>();

                foreach (var item in lbMovies.SelectedItems)
                {
                    Movie NewMovie = item as Movie;
                    selectedMovies.Add(NewMovie);
                }
                using (var ctx = new ManageMoviesContext())
                {
                    Actor actor = (from a in ctx.Actors
                                   where a.Id == selectedActor.Id
                                   select a).First();

                    foreach (Movie selectedMovie in selectedMovies)
                    {
                        Movie movie = (from m in ctx.Movies
                                       where m.MovieSerial == selectedMovie.MovieSerial
                                       select m).First();
                        ctx.ActorMovie.Add(new ActorMovie
                        {
                            Actor = actor,
                            ActorId = actor.Id,
                            Movie = movie,
                            MovieSerial = movie.MovieSerial
                        });
                    }
                    ctx.SaveChanges();

                }
                UpdateMovieList(selectedActor);
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

        private void btnAddNewMovie_Click(object sender, RoutedEventArgs e)
        {
            AddMovieWindow window = new AddMovieWindow();
            window.FromOscar = true;
            window.ShowDialog();

            refreshLists();
            UpdateMovieList((cbActor.SelectedItem as Actor));
        }

        private void btnAddNewActor_Click(object sender, RoutedEventArgs e)
        {
            AddActorWindow window = new AddActorWindow();
            window.maleRadio.IsChecked = true;

            window.maleRadio.IsEnabled = false;
            window.FemaleRadio.IsEnabled = false;
            window.FromOscar = true;

            window.ShowDialog();
            refreshLists();
            UpdateMovieList((cbActor.SelectedItem as Actor));
        }
    }
}


