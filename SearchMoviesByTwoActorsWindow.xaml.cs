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
using System.Data.Common;

namespace DBMoviesManager
{
    public partial class SearchMoviesByTwoActorsWindow : Window
    {
        public SearchMoviesByTwoActorsWindow()
        {
            InitializeComponent();
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
            if (cbSecondActor.SelectedItem==null || cbFirstActor.SelectedItem == null)
            {
                MessageBox.Show("You must select two actors");
                return;
            }

            Actor actorOne = cbFirstActor.SelectedItem as Actor;
            Actor actorTwo = cbSecondActor.SelectedItem as Actor;

            if (actorOne.Id==actorTwo.Id)
            {
                lbMovies.ItemsSource = null;
                MessageBox.Show("You select the same actor,this is bad!");
                return;
            }

            try
            {
                using (var context = new ManageMoviesContext())
                {
                    var MoviesActorOne =  from am in context.ActorMovie
                                          where am.ActorId == actorOne.Id
                                          select am.Movie;

                    var MoviesActorTwo = from am in context.ActorMovie
                                         where am.ActorId == actorTwo.Id
                                         select am.Movie;

                    lbMovies.ItemsSource = MoviesActorOne.Intersect(MoviesActorTwo).ToList();
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

        private void cleanOldData()
        {
            tbDirector.Text = "";
            tbYear.Text = "";
            tbCountry.Text = "";
            tbScore.Text = "";
            lbActors.ItemsSource = null;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    cbFirstActor.ItemsSource = (from a in context.Actors
                                                select a).ToList();
                    cbSecondActor.ItemsSource = (from a in context.Actors
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
    }
}
