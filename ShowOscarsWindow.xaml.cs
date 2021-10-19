using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
    public partial class ShowOscarsWindow : Window
    {
        public ShowOscarsWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void RefreshList()
        {
            try
            {
                using (var context = new ManageMoviesContext())
                {
                    var Oscars = context.Oscars.Include(o => o.BestActor)
                                               .Include(o => o.BestActress)
                                               .Include(o => o.BestDirector)
                                               .Include(o => o.MovieSerialNavigation).ToList();
                    lbOscars.ItemsSource = (from a in Oscars
                                            select new { Year = a.Year,
                                                BestActor = a.BestActor,
                                                BestActress=a.BestActress,
                                                BestDirector=a.BestDirector, 
                                                BestMotionPicture=a.MovieSerialNavigation }).ToList();


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

        private void lbOscars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbOscars.SelectedItem == null) return;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;

        }

        private void editButton(object sender, RoutedEventArgs e)
        {
            editOscarWindow editWindow = new editOscarWindow();

            var x = new
            {
                Year = 1990,
                BestActor = new Actor(),
                BestActress = new Actor(),
                BestDirector = new Director(),
                BestMotionPicture = new Movie()
            };
            var selectedOscar = Cast(x, lbOscars.SelectedItem);


            editWindow.tbOscarYear.Text = selectedOscar.Year.ToString();
            editWindow.tbOscarYear.IsEnabled = false;

            editWindow.tbOscarActorName.Text = $"{selectedOscar.BestActor.FirstName} {selectedOscar.BestActor.LastName}";
            editWindow.tbOscarActressName.Text = $"{selectedOscar.BestActress.FirstName} {selectedOscar.BestActress.LastName}";
            editWindow.tbOscarDirectorName.Text = $"{selectedOscar.BestDirector.FirstName} {selectedOscar.BestDirector.LastName}";
            editWindow.tbOscarMovieName.Text = $"{selectedOscar.BestMotionPicture.Title}";
            editWindow.ShowDialog();
            RefreshList();
        }

        private void deleteButton(object sender, RoutedEventArgs e)
        {
            if (lbOscars.SelectedItem == null) return;

            var x = new
            {
                Year = 1990,
                BestActor = new Actor(),
                BestActress = new Actor(),
                BestDirector = new Director(),
                BestMotionPicture = new Movie()
            };
            var selectedOscar = Cast(x, lbOscars.SelectedItem);

            try
            {
                using (var context = new ManageMoviesContext())
                {

                    var Oscars = context.Oscars.Include(d => d.BestActor)
                        .Include(d => d.BestActress)
                        .Include(d => d.BestDirector)
                        .Include(d => d.MovieSerialNavigation).ToList();

                    Oscar Oscar = (from a in Oscars
                                   where a.Year == selectedOscar.Year
                                   select a).First();
                    
                    context.Oscars.Remove(Oscar);
                    context.SaveChanges();
                    RefreshList();
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

        private static T Cast<T>(T typetoConvert, Object objectToConvert) {
            return (T)objectToConvert;
        }
    }
}
