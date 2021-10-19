using System;
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
using System.Linq;
using System.Data.Common;

namespace DBMoviesManager
{
    public partial class ConnectActressToMovieWindow : Window
    {
        public ConnectActressToMovieWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbMovie.SelectedItem == null || lbActresses.SelectedItem == null)
            {
                MessageBox.Show("You must select a Movie and at least one actress");
                return;
            }
            try
            {
                Movie selectedMovie = cbMovie.SelectedItem as Movie;
                ICollection<Actor> selectedActors = new List<Actor>();

                foreach (var item in lbActresses.SelectedItems)
                {
                    Actor NewActor = item as Actor;
                    selectedActors.Add(NewActor);
                }
                using (var ctx = new ManageMoviesContext())
                {
                    Movie movie = (from m in ctx.Movies
                                   where m.MovieSerial == selectedMovie.MovieSerial
                                   select m).First();

                    foreach (Actor selectedActor in selectedActors)
                    {
                        Actor actor = (from a in ctx.Actors
                                       where a.Id == selectedActor.Id
                                       select a).First();
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
                UpdateMovieList(selectedMovie);
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

        private void cbMovie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    UpdateMovieList((cbMovie.SelectedItem as Movie));
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

        private void UpdateMovieList(Movie selectedMovie)
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    if (selectedMovie == null)
                    {
                        lbActresses.ItemsSource = null;
                        lbActresses.ItemsSource = (from a in context.Actors
                                                   where a.Gender == (int)Gender.Female
                                                   select a).ToList();
                        return;
                    }
                    var smallQuery = (from am in context.ActorMovie
                                      where am.MovieSerial == selectedMovie.MovieSerial
                                      && am.Actor.Gender == (int)Gender.Female
                                      select am.Actor).ToList();
                    var BigQuery = (from a in context.Actors
                                    where a.Gender == (int)Gender.Female
                                    select a).ToList();
                    var result = BigQuery.Except(smallQuery).ToList();
                    lbActresses.ItemsSource = result;
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

        private void Window_Initialized(object sender, EventArgs e)
        {
            refreshLists();
        }

        private void btnAddNewActress_Click(object sender, RoutedEventArgs e)
        {
            AddActorWindow window = new AddActorWindow();
            window.maleRadio.IsChecked = false;

            window.maleRadio.IsEnabled = false;
            window.FemaleRadio.IsEnabled = false;
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
                    lbActresses.ItemsSource = (from a in context.Actors
                                               where a.Gender == (int)Gender.Female
                                               select a).ToList();
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

        private void btnAddNewMovie_Click(object sender, RoutedEventArgs e)
        {
            AddMovieWindow window = new AddMovieWindow();
            window.FromOscar = true;
            window.ShowDialog();
            refreshLists();
            UpdateMovieList((cbMovie.SelectedItem as Movie));

        }
    }
}
