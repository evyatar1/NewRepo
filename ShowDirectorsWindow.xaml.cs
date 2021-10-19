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
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace DBMoviesManager
{
    public partial class ShowDirectorsWindow : Window
    {
        public ShowDirectorsWindow()
        {
            InitializeComponent();
        }

        private void lbDirectors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbDirectors.SelectedItem == null) {
                btnEdit.IsEnabled = false;
                return;
            }
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;

            Director selectedDirector = lbDirectors.SelectedItem as Director;
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    var Directors = context.Directors.Include(t => t.Movies).Include(t => t.Oscars).ToList();
                    Director Director = (from a in Directors
                                         where a.Id == selectedDirector.Id
                                   select a).First();
                    lbOscar.ItemsSource = Director.Oscars;
                    lbMovies.ItemsSource = Director.Movies;
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
            refreshList();
        }

        private void refreshList()
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    lbDirectors.ItemsSource = (from a in context.Directors
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

        private void editButton(object sender, RoutedEventArgs e)
        {
            if (lbDirectors.SelectedItem == null) {
                btnEdit.IsEnabled = false;
                return;
            }
            Director selectedDirector = lbDirectors.SelectedItem as Director;
            EditPersonWindow window = new EditPersonWindow();
            window.Title = "Edit Director";
            window.Role = ROLE.DirectorROLE;
            window.tbActorFirstName.Text = selectedDirector.FirstName;
            window.tbActorLastName.Text = selectedDirector.LastName;
            window.IdPerson = selectedDirector.Id;

            window.ShowDialog();
            refreshList();
        }

        private void deleteButton(object sender, RoutedEventArgs e)
        {
            if (lbDirectors.SelectedItem == null) return;
            Director SelectedDirector = lbDirectors.SelectedItem as Director;
            try
            {
                using (var context = new ManageMoviesContext())
                {

                    var Directors = context.Directors.Include(d => d.Oscars).Include(d => d.Movies).ToList();
                    Director Director = (from a in Directors
                                         where a.Id == SelectedDirector.Id
                                   select a).First();
                    if (Director.Oscars.Count != 0)
                    {
                        MessageBox.Show("This director won oscar.you can't delete him.\n" +
                            "after delete director from oscar, try to delete");
                        return;
                    }
                    if (Director.Movies.Count != 0) {
                        foreach (Movie movie in Director.Movies.ToList()) { 
                        Movie tempMovie = (from a in context.Movies
                                           where a.MovieSerial == movie.MovieSerial
                                           select a).First();
                            tempMovie.Director = null;
                            context.Movies.Update(tempMovie);
                            context.SaveChanges();
                        }
                        
                    }
                    context.Directors.Remove(Director);
                    context.SaveChanges();
                    refreshList();
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
