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
    public partial class ShowActorWindow : Window
    {
        public ShowActorWindow()
        {
            InitializeComponent();
        }

        private void lbActors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbActors.SelectedItem == null) return;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;
            Actor selectedActor = lbActors.SelectedItem as Actor;
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    var Actors = context.Actors.Include(t => t.OscarsBestActor).Include(t => t.OscarsBestActress).ToList();
                    Actor Actor = (from a in Actors
                                   where a.Id == selectedActor.Id
                                   select a).First();
                    lbOscar.ItemsSource =(Actor.Gender==0)? Actor.OscarsBestActor:Actor.OscarsBestActress;
                    lbMovies.ItemsSource = (from am in context.ActorMovie
                                            where am.ActorId == selectedActor.Id
                                            select am.Movie).ToList();
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
                    lbActors.ItemsSource = (from a in context.Actors
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
            if (lbActors.SelectedItem == null) return;
            Actor selectedActor = lbActors.SelectedItem as Actor;
            EditPersonWindow window = new EditPersonWindow();
            window.Title = "Edit Actor";
            window.Role = ROLE.ActorROLE;
            window.tbActorFirstName.Text = selectedActor.FirstName;
            window.tbActorLastName.Text = selectedActor.LastName;
            window.IdPerson = selectedActor.Id;
            window.tbActorYearBorn.Text = selectedActor.YearBorn.ToString();
            window.FemaleRadio.IsChecked = (selectedActor.Gender == 1) ? true : false;
            window.maleRadio.IsChecked = (selectedActor.Gender == 1) ? false : true;

            window.label1.Visibility = Visibility.Visible;
            window.label2.Visibility = Visibility.Visible;
            window.tbActorYearBorn.Visibility = Visibility.Visible;
            window.FemaleRadio.Visibility = Visibility.Visible;
            window.maleRadio.Visibility = Visibility.Visible;
            window.ShowDialog();
            refreshList();
        }

        private void deleteButton(object sender, RoutedEventArgs e)
        {
            if (lbActors.SelectedItem == null) return;
            Actor selectedActor = lbActors.SelectedItem as Actor;
            try
            {
                using (var context = new ManageMoviesContext())
                {

                    var Actors = context.Actors.Include(t => t.OscarsBestActor).Include(t => t.OscarsBestActress).ToList();
                    Actor Actor = (from a in Actors
                                   where a.Id == selectedActor.Id
                                   select a).First();
                    if ((Actor.Gender == (int)Gender.Male ? Actor.OscarsBestActor : Actor.OscarsBestActress).Count != 0) {
                        MessageBox.Show("This actor/actress won oscar.you can't delete him.\n" +
                            "after delete actor/actress from oscar, try to delete");
                            return; }
                        context.Actors.Remove(Actor);
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
