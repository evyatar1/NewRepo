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
    public partial class AddActorWindow : Window
    {
        public Actor Actor { get; set; }
        public bool FromOscar { get; internal set; }

        public AddActorWindow()
        {
            InitializeComponent();
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

        private void AddActorButton_Click(object sender, RoutedEventArgs e)
        {
            Actor = null;
            if (!AllTextboxesFilled())
            {
                MessageBox.Show("You must provide all the data");
                return;
            }
            try
            {
                String ActorFirstName = tbActorFirstName.Text.Trim();
                if (!isNameVaild(ActorFirstName)) throw new ValidationException("First Name is not in the correct format(based on hw2)");
                String ActorLastName = tbActorLastName.Text.Trim();
                if (!isNameVaild(ActorLastName)) throw new ValidationException("Last Name is not in the correct format(based on hw2)");
                int ActorYear = int.Parse(tbActorYearBorn.Text.Trim());
                if (ActorYear > 2021) throw new ValidationException("Year is not in the correct format(less or equal to 2021)");
                Gender ActorGender = FemaleRadio.IsChecked == true ? Gender.Female : Gender.Male;

                Actor = new Actor
                {
                    FirstName = ActorFirstName,
                    LastName = ActorLastName,
                    YearBorn = ActorYear,
                    Gender = (int)ActorGender
                };
                using (var ctx = new ManageMoviesContext())
                {
                    ctx.Actors.Add(Actor);
                    ctx.SaveChanges();
                    resetFields();
                    MessageBox.Show("The " + (ActorGender == Gender.Male ? "Actor " : "Actress ") + $"{ActorFirstName} {ActorLastName} added to database");
                    if (FromOscar) {
                        Close();
                    }
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

        private void resetFields()
        {
            tbActorFirstName.Text = "";
            tbActorLastName.Text  = "";
            tbActorYearBorn.Text  = "";
            FemaleRadio.IsChecked = true;
        }

        private static Regex nameRegex = new Regex(@"^[A-Z][A-Za-z]*(\s+[A-Z][A-Za-z]*){0,3}$");
        public static bool isNameVaild(string value)
        {
            return nameRegex.IsMatch(value);
        }
    }
}