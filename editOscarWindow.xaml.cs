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
using Microsoft.EntityFrameworkCore;

namespace DBMoviesManager
{
    public partial class editOscarWindow : Window
    {
        public editOscarWindow()
        {
            InitializeComponent();
        }

        private void EditOscarButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AllTextboxesFilled())
            {
                MessageBox.Show("You must provide all the data");
                return;
            }
            try
            {
                int OscarYear = int.Parse(tbOscarYear.Text.Trim());

                Oscar Oscar;
                using (var ctx = new ManageMoviesContext())
                {
                    Oscar = (from o in ctx.Oscars
                             where (o.Year == OscarYear)
                             select o).First();
                    
                        //add Movie Oscar
                        Movie movie = MovieInDatabase(ctx, tbOscarMovieName.Text.Trim(), OscarYear - 1);
                        if (movie == null) throw new ValidationException("movie", "Problem to find or add movie in database");
                        tbOscarMovieName.Text = movie.Title;

                        //add Actor Oscar
                        if (tbOscarActorName.Text.Trim().Split(' ').Length == 1) throw new ValidationException("actor", "Name must include first and last");
                        Actor actor = ActorInDatabase(ctx, tbOscarActorName.Text.Trim(), Gender.Male);
                        if (actor == null) throw new ValidationException("actor", "Problem to find or add actor in database");
                        tbOscarActorName.Text = actor.FirstName + " " + actor.LastName;

                        //add Actress Oscar
                        if (tbOscarActressName.Text.Trim().Split(' ').Length == 1) throw new ValidationException("actress", "Name must include first and last");
                        Actor actress = ActorInDatabase(ctx, tbOscarActressName.Text.Trim(), Gender.Female);
                        if (actress == null) throw new ValidationException("actress", "Problem to find or add actress in database");
                        tbOscarActressName.Text = actress.FirstName + " " + actress.LastName;

                        //add Director Oscar
                        if (tbOscarDirectorName.Text.Trim().Split(' ').Length == 1) throw new ValidationException("director", "Name must include first and last");
                        Director director = DirectorInDatabase(ctx, tbOscarDirectorName.Text.Trim());
                        if (director == null) throw new ValidationException("director", "Problem to find or add director in database");
                        tbOscarDirectorName.Text = director.FirstName + " " + director.LastName;

                        Oscar.BestDirector = director;
                        Oscar.BestActress = actress;
                        Oscar.BestActor = actor;
                        Oscar.MovieSerialNavigation = movie;
                        ctx.Oscars.Update(Oscar);
                        ctx.SaveChanges();
                        
                        
                        MessageBox.Show($"The Oscar edited:\n" +
                                        $"Year:{Oscar.Year}\n" +
                                        $"Actor:{Oscar.BestActor.FirstName} {Oscar.BestActor.LastName}\n" +
                                        $"Actress:{Oscar.BestActress.FirstName} {Oscar.BestActress.FirstName}\n" +
                                        $"Director:{Oscar.BestDirector.FirstName} {Oscar.BestDirector.FirstName}\n" +
                                        $"Movie:{Oscar.MovieSerialNavigation.Title}\n");
                    Close();
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

        private Movie MovieInDatabase(ManageMoviesContext ctx, string tbOscarMovieName, int searchYear)
        {
            ICollection<Movie> movies = (from m in ctx.Movies
                                         where (m.Title == tbOscarMovieName) && m.Year == searchYear
                                         select m).ToList();
            switch (movies.Count())
            {
                case 0:
                    MessageBox.Show("There is no movie in database with this name " + tbOscarMovieName + " and the year " + searchYear + "\n.You can add this movie to database");
                    AddMovieWindow addWindow = new AddMovieWindow();
                    addWindow.tbMovieTitle.Text = tbOscarMovieName.Trim();
                    addWindow.FromOscar = true;
                    addWindow.tbMovieYear.Text = searchYear.ToString();

                    addWindow.tbMovieTitle.IsEnabled = false;

                    addWindow.ShowDialog();
                    if (addWindow.Movie != null)
                        return (from m in ctx.Movies
                                where m.MovieSerial == addWindow.Movie.MovieSerial
                                select m).First();
                    break;
                case 1:
                    return movies.First();
                default:
                    ChoiceFromDuplicationWindow window = new ChoiceFromDuplicationWindow();
                    window.lsChoices.ItemsSource = movies;
                    window.role = ROLE.MovieROLE;
                    window.ShowDialog();
                    if (window.movie != null)
                        return (from m in ctx.Movies
                                where m.MovieSerial == window.movie.MovieSerial
                                select m).First();
                    break;
            }
            return null;
        }

        private Director DirectorInDatabase(ManageMoviesContext ctx, string tbOscarDirectorName)
        {
            ICollection<Director> Directors = (from a in ctx.Directors
                                               where (a.FirstName + " " + a.LastName == tbOscarDirectorName)
                                               select a).ToList();
            switch (Directors.Count())
            {
                case 0:
                    MessageBox.Show("There is no director in database with this name " + tbOscarDirectorName + ".\nYou can add this director to database");
                    AddDirectorWindow addWindow = new AddDirectorWindow();
                    addWindow.tbDirectorFirstName.Text = tbOscarDirectorName.Trim().Split(' ')[0];
                    addWindow.tbDirectorLastName.Text = tbOscarDirectorName.Trim().Substring(addWindow.tbDirectorFirstName.Text.Length + 1);

                    addWindow.FromOscar = true;
                    addWindow.ShowDialog();
                    if (addWindow.Director != null)
                        return (from a in ctx.Directors
                                where a.Id == addWindow.Director.Id
                                select a).First();
                    break;
                case 1: return Directors.First();
                default:
                    ChoiceFromDuplicationWindow window = new ChoiceFromDuplicationWindow();
                    window.lsChoices.ItemsSource = Directors;
                    window.role = ROLE.DirectorROLE;
                    window.ShowDialog();
                    if (window.director != null)
                        return (from a in ctx.Directors
                                where a.Id == window.director.Id
                                select a).First();
                    break;
            }
            return null;
        }

        private Actor ActorInDatabase(ManageMoviesContext ctx, string OscarActorName, Gender gender)
        {
            ICollection<Actor> actors = (from a in ctx.Actors
                                         where (a.FirstName + " " + a.LastName == OscarActorName) & a.Gender == (int)gender
                                         select a).ToList();
            switch (actors.Count())
            {
                case 0:
                    MessageBox.Show("There is no " + (gender == Gender.Female ? "Actress" : "Actor") + " in database with this name " + OscarActorName + ".\nYou can add this " + (gender == Gender.Female ? "Actress" : "Actor") + " to database");
                    AddActorWindow addWindow = new AddActorWindow();
                    addWindow.tbActorFirstName.Text = OscarActorName.Trim().Split(' ')[0];
                    addWindow.tbActorLastName.Text = OscarActorName.Trim().Substring(addWindow.tbActorFirstName.Text.Length + 1);
                    addWindow.maleRadio.IsChecked = (gender == Gender.Male);
                    addWindow.FemaleRadio.IsChecked = (gender == Gender.Female);

                    addWindow.maleRadio.IsEnabled = false;
                    addWindow.FemaleRadio.IsEnabled = false;

                    addWindow.FromOscar = true;
                    addWindow.ShowDialog();
                    if (addWindow.Actor != null)
                        return (from a in ctx.Actors
                                where a.Id == addWindow.Actor.Id
                                select a).First();
                    break;
                case 1: return actors.First();
                default:
                    ChoiceFromDuplicationWindow window = new ChoiceFromDuplicationWindow();
                    window.lsChoices.ItemsSource = actors;
                    window.role = ROLE.ActorROLE;
                    window.ShowDialog();
                    if (window.actor != null)
                        return (from a in ctx.Actors
                                where a.Id == window.actor.Id
                                select a).First();
                    break;
            }
            return null;
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
