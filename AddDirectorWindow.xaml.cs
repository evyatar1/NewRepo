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
    public partial class AddDirectorWindow : Window
    {
        public Director Director { get; set; }
        public bool FromOscar { get; internal set; }

        public AddDirectorWindow()
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

        private void AddDirectorButton_Click(object sender, RoutedEventArgs e)
        {
            Director = null;
            if (!AllTextboxesFilled())
            {
                MessageBox.Show("You must provide all the data");
                return;
            }
            try
            {
               String DirectorFirstName = tbDirectorFirstName.Text.Trim();
                if (!isNameVaild(DirectorFirstName)) throw new ValidationException("First Name is not in the correct format(based on hw2)");
                String DirectorLastName = tbDirectorLastName.Text.Trim();
                if (!isNameVaild(DirectorLastName)) throw new ValidationException("Last Name is not in the correct format(based on hw2)");


                Director = new Director
                {
                    
                    FirstName = DirectorFirstName,
                    LastName = DirectorLastName
                };
                using (var ctx = new ManageMoviesContext())
                {
                    ctx.Directors.Add(Director);
                    ctx.SaveChanges();
                    resetFields();
                    MessageBox.Show($"The director {DirectorFirstName} {DirectorLastName} added to database");
                    if (FromOscar) Close();
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
            
            tbDirectorFirstName.Text = "";
            tbDirectorLastName.Text = "";
        }

        private static Regex nameRegex = new Regex(@"^[A-Z][A-Za-z]*(\s+[A-Z][A-Za-z]*){0,3}$");
        public static bool isNameVaild(string value)
        {
            return nameRegex.IsMatch(value);
        }
    }
}
