using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class EditPersonWindow : Window
    {
        public ROLE Role { get; set; }
        public Director Director { get; set; }
        public Actor Actor { get; set; }
        public int IdPerson { get; internal set; }

        public EditPersonWindow()
        {
            InitializeComponent();
        }

        private void editActor()
        {
            if (string.IsNullOrEmpty(tbActorFirstName.Text.Trim()) ||
                string.IsNullOrEmpty(tbActorLastName.Text.Trim())  ||
                string.IsNullOrEmpty(tbActorYearBorn.Text.Trim()))
            {
                MessageBox.Show("You must provide all the data");
                return;
            }
            Actor = null;
            try
            {
                String ActorFirstName = tbActorFirstName.Text.Trim();
                if (!isNameVaild(ActorFirstName)) throw new ValidationException("First Name is not in the correct format(based on hw2)");
                String ActorLastName = tbActorLastName.Text.Trim();
                if (!isNameVaild(ActorLastName)) throw new ValidationException("Last Name is not in the correct format(based on hw2)");
                int ActorYear = int.Parse(tbActorYearBorn.Text.Trim());
                if (ActorYear > 2020) throw new ValidationException("Year is not in the correct format(less or equal to 2020)");
                Gender ActorGender = FemaleRadio.IsChecked == true ? Gender.Female : Gender.Male;


                Actor = new Actor
                {
                    Id=IdPerson,
                    FirstName = ActorFirstName,
                    LastName = ActorLastName,
                    YearBorn = ActorYear,
                    Gender = (int)ActorGender
                };
                using (var ctx = new ManageMoviesContext())
                {
                    ctx.Actors.Update(Actor);
                    ctx.SaveChanges();
                    MessageBox.Show("The " + (ActorGender == Gender.Male ? "Actor " : "Actress ") + $"{ActorFirstName} {ActorLastName} edited");
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

        private void editDirector()
        {
            if (string.IsNullOrEmpty(tbActorFirstName.Text.Trim())||
                string.IsNullOrEmpty(tbActorLastName.Text.Trim())) {
                MessageBox.Show("You must provide all the data");
                return;
            }
            Director = null;
            try
            {
                String DirectorFirstName = tbActorFirstName.Text.Trim();
                if (!isNameVaild(DirectorFirstName)) throw new ValidationException("First Name is not in the correct format(based on hw2)");
                String DirectorLastName = tbActorLastName.Text.Trim();
                if (!isNameVaild(DirectorLastName)) throw new ValidationException("Last Name is not in the correct format(based on hw2)");

                Director = new Director
                {
                    Id=IdPerson,
                    FirstName = DirectorFirstName,
                    LastName = DirectorLastName
                };
                using (var ctx = new ManageMoviesContext())
                {
                    ctx.Directors.Update(Director);
                    ctx.SaveChanges();
                    MessageBox.Show($"The director {DirectorFirstName} {DirectorLastName} edited");
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

            private void editActorButton_Click(object sender, RoutedEventArgs e)
        {
                switch (Role)
            {
                case ROLE.ActorROLE:
                    editActor();
                    break;
                case ROLE.DirectorROLE:
                    editDirector();
                    break;
            }
        }

        private static Regex nameRegex = new Regex(@"^[A-Z][A-Za-z]*(\s+[A-Z][A-Za-z]*){0,3}$");
        public static bool isNameVaild(string value)
        {
            return nameRegex.IsMatch(value);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            MessageBox.Show("For update connection between "+(Role==ROLE.ActorROLE?"Actros/Actres":"Director")+" To movie.Go to Connect tab");
        }
    }
}
